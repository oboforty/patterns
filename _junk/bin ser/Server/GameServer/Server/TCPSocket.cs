using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using QServerSDK.ServerHandler;


namespace GameServer
{
    public class TCPSocket : IServer
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);

        QServerHandler<IPEndPoint> handler;

        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public class State
        {
            public Socket conn;

            public byte[] buffer = new byte[bufSize];
        }

        public TCPSocket(QServerHandler<IPEndPoint> handler)
        {
            this.handler = handler;
        }

        public void StartServer(IPEndPoint endPoint)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(endPoint);
            _socket.Listen(10);

            Console.WriteLine("(TCP) Online");

            Receive();
        }

        const byte OP_NEWPLAYER = 0b0000_0001;

        public void Send(int playerId, byte[] data)
        {
            // @TODO: TCP send
            Console.WriteLine("(TCP) Fake SEND {0} {1}", playerId, data);
        }

        private void Receive()
        {
            try
            {
                while (true)
                {
                    allDone.Reset();
                    _socket.BeginAccept(new AsyncCallback((on)=> {
                        State so = (State) on.AsyncState;
                        Socket clientSocket = _socket.EndAccept(on);

                        // Create the state object.
                        State state = new State();
                        state.conn = clientSocket;
                        clientSocket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, new AsyncCallback(ReadCallback), state);
                    }), state);
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            State so = (State)ar.AsyncState;

            // Read data from the client socket.
            int bytesRead = so.conn.EndReceive(ar);

            if (bytesRead > 0)
            {
                Console.WriteLine("(TCP) RECV: {0} [{1}]", epFrom.ToString(), bytesRead);

                switch (so.buffer[0])
                {
                    case OP_NEWPLAYER:
                        // byte 0 is playerId + isSpectator + 2 reserved bits
                        bool isSpectator = (so.buffer[1] & (0x1 << 6)) > 0;
                        int playerId = (so.buffer[1] & 0b00011111);
                        int _reserved = (so.buffer[1] & 0b11000000) >> 6;
                        // byte 1 is connHash
                        int connHash = so.buffer[2];

                        if (handler.StartJoinPlayer(playerId, connHash, isSpectator))
                        {
                            Console.WriteLine("(TCP) NEW {1} #{0}", playerId, isSpectator ? "SPECTATOR" : "PLAYER");

                            // send handshake OK signal
                            so.conn.Send(new byte[] { 0x0F, 0x1F, 0x2F });
                        }
                        else
                        {
                            // @TODO: @later: json encoding for TCP
                            Console.WriteLine("(TCP) Handshake failed {0}", playerId);
                            return;
                        }

                        break;
                    default:
                        Console.WriteLine("(TCP) OPCODE NOT FOUND}");
                        break;
                }

                so.conn.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, new AsyncCallback(ReadCallback), so);
            }
        }
    }
}
