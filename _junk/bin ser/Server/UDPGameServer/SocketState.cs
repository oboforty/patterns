using System;
using System.Net.Sockets;

namespace UDPGameServer
{
    public class SocketState
    {
        // Client  socket.
        public Socket WorkSocket = null;
        // Size of receive buffer.
        public const int BUFFER_SIZE = 5242880;
        // Receive buffer.
        public byte[] Buffer = new byte[BUFFER_SIZE];
        // Received data string.
        //public StringBuilder Sb = new StringBuilder();

    }
}
