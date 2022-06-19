using QServerSDK;
using QServerSDK.ServerHandler;
using System;

namespace GameServer
{
    public class Metin2Arena : IArenaHandler
    {
        public int NPlayers => 2;
        QSnapshot state;

        const int maxPos = 16384 - 1;
        const int speed = 300;

        public void BuildArena(QSnapshot state)
        {
            this.state = state;

            state.players[0].Pos.x = maxPos / 2;
            state.players[0].Pos.z = 1600;
            state.players[1].Pos.x = maxPos / 2;
            state.players[1].Pos.z = maxPos - 1600;
        }

        public void ReceiveCommand(int player, QCommand cmd)
        {
            // just add movement to state
            if (cmd.DesiredMove != Vector3.zero)
            {
                //Console.WriteLine("--MOVE [{0},{1},{2}]", cmd.DesiredMove.x, cmd.DesiredMove.y, cmd.DesiredMove.z);
                Console.WriteLine("Moving!!");

                state.players[player].Pos = (state.players[player].Pos + cmd.DesiredMove * speed).Clamp(0, maxPos);
            }
            else
                Console.WriteLine("");
        }

        public void Dispose()
        {

        }

    }
}
