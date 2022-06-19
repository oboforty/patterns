using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

using QServerSDK.ServerHandler;

namespace GameServer
{
    public class UDPSocket : IFrameSimulation, IServer
    {
        public bool Connected { get; private set; }

        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;

        bool simulate = false;
        QServerHandler<IPEndPoint> handler;

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        //IDictionary<int, IPEndPoint> playerClients;

        public UDPSocket(QServerHandler<IPEndPoint> handler)
        {
            this.handler = handler;
        }

        public void StartServer(IPEndPoint endPoint) 
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(endPoint);

            Console.WriteLine("(UDP) Online");

            Connected = true;
            Receive();
        }

        private void Receive()
        {
            // Continious receiving handler
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);

                var endPoint = (epFrom as IPEndPoint);
                var player = handler.GetPlayerInfo(endPoint);
                //Console.WriteLine("--UDP RECV {0} [{1}]", epFrom.ToString(), bytes);

                if (player == null || !player.Connected)
                {
                    // handle new connection
                    // byte 0 is OPCODE
                    if (so.buffer[0] != 0x01) // force handshake OPCODE
                        return;
                    // byte 1 is playerId + isSpectator + 2 reserved bits
                    bool isSpectator = (so.buffer[1] & (0x1 << 6)) > 0;
                    int playerId = (so.buffer[1] & 0b00011111);
                    int _reserved = (so.buffer[1] & 0b11000000) >> 6;
                    // byte 2 is connHash
                    int connHash = so.buffer[2];

                    if (handler.EndJoinPlayer(playerId, connHash, endPoint))
                    {
                        // connection flag set: address saved & client provided hash was correct
                        // Successful connection
                        Console.WriteLine("(UDP) new connection: {0}: #{1}", epFrom.ToString(), playerId);

                        Send(playerId, new byte[] { 0x0F, 0x1F, 0x2F });
                        //playerClients[playerId] = endPoint;

                        if (handler.CanStart)
                            simulate = true;
                    }
                    else
                    {
                        Console.WriteLine("(UDP) Handshake failed for {0} [{1}]", epFrom.ToString(), bytes);
                        return;
                    }
                }
                else
                {
                    // in-game packet
                    handler.ReceivePacket(player.playerId, so.buffer, bytes);
                }

                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
            }, state);
        }

        public void Send(int playerId, byte[] data)
        {
            // playerInfo class contains the IPEndPoint
            var player = handler.GetPlayerInfo(playerId);

            //Console.WriteLine("--SENT TO #{0} {1}:{2} [{3} bytes]", playerId, player.ConnInfo.Address, player.ConnInfo.Port, data.Length);

            _socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, player.ConnInfo, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);

            }, state);
        }

        public void SimulateFrame()
        {
            if (!Connected || !simulate)
                return;

            //  call simulate world
            handler.OnSimulation();
        }
    }
}
