using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using HangmanGameBusiness.Games;
using HangmanGameEntities.Dtos;

namespace HangmanGameServices.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class GameService : IGameService
    {
        private static readonly ConcurrentDictionary<string, List<IGameCallback>> _gameSubscribers
            = new ConcurrentDictionary<string, List<IGameCallback>>();

        private readonly IGameBusiness _gameBusiness;
        private IGameCallback _callback;

        public GameService()
        {
            _gameBusiness = new GameBusiness();
        }

        public GameOperationResultDto CreateGame(CreateGameDto createGameDto)
        {
            return _gameBusiness.CreateGame(createGameDto);
        }

        public GameOperationResultDto JoinGame(int gameId, int retadorId)
        {
            var result = _gameBusiness.JoinGame(gameId, retadorId);
            if (result.Success && result.Game != null)
            {
                var game = result.Game;
                // Notificar en hilo separado para no bloquear la respuesta al cliente
                ThreadPool.QueueUserWorkItem(_ => NotifySubscribers(gameId, cb => cb.OnPlayerJoined(game)));
            }
            return result;
        }

        public GameOperationResultDto GetAvailableGames()
        {
            return _gameBusiness.GetAvailableGames();
        }

        public GameOperationResultDto GetGameState(int gameId)
        {
            return _gameBusiness.GetGameState(gameId);
        }

        public GameOperationResultDto GuessLetter(GuessLetterDto guessLetterDto)
        {
            var result = _gameBusiness.GuessLetter(guessLetterDto);
            if (result.Success && result.GameState != null)
            {
                var state = result.GameState;
                int gameId = guessLetterDto.GameId;
                // Notificar en hilo separado — evita deadlock con el cliente que hizo la llamada
                if (state.IsOver)
                    ThreadPool.QueueUserWorkItem(_ => NotifySubscribers(gameId, cb => cb.OnGameEnded(state)));
                else
                    ThreadPool.QueueUserWorkItem(_ => NotifySubscribers(gameId, cb => cb.OnGameStateChanged(state)));
            }
            return result;
        }

        public GameOperationResultDto AbandonGame(int gameId, int userId)
        {
            var result = _gameBusiness.AbandonGame(gameId, userId);
            if (result.Success)
            {
                var finalState = _gameBusiness.GetGameState(gameId);
                if (finalState.Success && finalState.GameState != null)
                {
                    var state = finalState.GameState;
                    ThreadPool.QueueUserWorkItem(_ => NotifySubscribers(gameId, cb => cb.OnGameEnded(state)));
                }
            }
            return result;
        }

        public GameOperationResultDto GetUserScore(int userId)
        {
            return _gameBusiness.GetUserScore(userId);
        }

        public void RegisterForGame(int gameId, int userId)
        {
            try
            {
                _callback = OperationContext.Current.GetCallbackChannel<IGameCallback>();
                string key = gameId.ToString();
                _gameSubscribers.AddOrUpdate(key,
                    _ => new List<IGameCallback> { _callback },
                    (_, list) => { if (!list.Contains(_callback)) list.Add(_callback); return list; });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GameService] RegisterForGame error: {ex.Message}");
            }
        }

        private static void NotifySubscribers(int gameId, Action<IGameCallback> action)
        {
            string key = gameId.ToString();
            if (!_gameSubscribers.TryGetValue(key, out var subscribers)) return;

            var dead = new List<IGameCallback>();
            lock (subscribers)
            {
                foreach (var cb in subscribers)
                {
                    try
                    {
                        if (((ICommunicationObject)cb).State == CommunicationState.Opened)
                            action(cb);
                        else
                            dead.Add(cb);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[GameService] Callback error: {ex.Message}");
                        dead.Add(cb);
                    }
                }
                foreach (var d in dead) subscribers.Remove(d);
            }
        }
    }
}
