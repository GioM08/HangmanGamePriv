using System.ServiceModel;
using HangmanGameEntities.Dtos;

namespace HangmanGameServices.Services
{
    public interface IGameCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnGameStateChanged(GameStateDto gameState);

        [OperationContract(IsOneWay = true)]
        void OnPlayerJoined(GameDto game);

        [OperationContract(IsOneWay = true)]
        void OnGameEnded(GameStateDto gameState);
    }
}
