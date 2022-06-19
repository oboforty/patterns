using QServerSDK;
using QServerSDK.Mapping;
using QServerSDK.ServerHandler;
using System;
using System.Net;

namespace GameServer
{
    public class MetinServer : QServerHandler<IPEndPoint>
    {
        IServer simuServer;
        IServer sgnServer;

        public MetinServer(int fps = 30)
            : base(new Metin2Arena(), fps)
        {
            defaultBitOffset = 0;
        }

        public void SetServers(IServer simu, IServer sgnServer)
        {
            this.simuServer = simu;
            this.sgnServer = sgnServer;
        }

        public override void Send(int playerId, QSnapshot snapshot)
        {
            byte[] packet = sm.Serialize(snapshot, out int bitOffset);
            int byteLength = (bitOffset - 1) / 8 + 1;
            SendPacket(playerId, packet, byteLength);
        }

        public override void SendPacket(int playerId, byte[] packet, int nbytes)
        {
            simuServer.Send(playerId, packet);
        }
    }
}
