using QServerSDK;
using QServerSDK.ServerHandler;
using System;


namespace QServerSDK.ServerHandler.Tests
{
    public class TestArena : IArenaHandler
    {
        public int NPlayers => 2;
        QSnapshot state;

        public void BuildArena(QSnapshot state)
        {
            this.state = state;
        }

        public void ReceiveCommand(int player, QCommand cmd)
        {
            // just add movement to state
            if (cmd.DesiredMove != Vector3.zero)
            {
                state.players[player].Pos += cmd.DesiredMove;
            }
        }

        public void Dispose()
        {

        }

    }
  
}
