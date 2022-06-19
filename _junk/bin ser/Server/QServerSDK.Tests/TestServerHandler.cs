
using System.Collections.Generic;


namespace QServerSDK.ServerHandler.Tests
{
    public class TestServerHandler : QServerHandler<string>
    {
        Queue<(int, int, byte[])> arr;

        public TestServerHandler(IArenaHandler arena, Queue<(int, int, byte[])> arr)
            :base(arena)
        {
            this.arr = arr;
        }

        public override void SendPacket(int playerId, byte[] packet, int nbytes)
        {
            // aid in sending packets to a queue
            arr.Enqueue((playerId, nbytes, packet));
        }
    }
}
