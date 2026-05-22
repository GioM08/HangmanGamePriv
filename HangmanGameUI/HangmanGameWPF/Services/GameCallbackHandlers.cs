using HangmanGameWPF.Services;

namespace HangmanGameWPF
{
    // Stub callback used when duplex connection is needed only for one-way calls
    internal class DummyGameCallback : IGameCallback
    {
        public void OnGameStateChanged(GameStateDto gameState) { }
        public void OnPlayerJoined(GameDto game) { }
        public void OnGameEnded(GameStateDto gameState) { }
    }
}
