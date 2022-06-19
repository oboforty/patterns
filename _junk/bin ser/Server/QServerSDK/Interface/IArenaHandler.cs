using System;


namespace QServerSDK.ServerHandler
{
    public interface IArenaHandler : IDisposable
    {
        int NPlayers { get; }
        void BuildArena(QSnapshot state);

        void ReceiveCommand(int playerId, QCommand cmd);
    }
}
