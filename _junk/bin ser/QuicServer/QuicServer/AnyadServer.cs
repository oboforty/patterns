using System;
using System.Text;
using QuicNet;
using QuicNet.Streams;
using QuicNet.Connections;
using System.Threading;

namespace Anyad.QuicServer
{
    public class Program
    {
        static QuicStream streamup = null;
        static QuicStream streamdown = null;

        // Fired when a client is connected
        static void ClientConnected(QuicConnection connection)
        {
            Console.WriteLine("new Client");
            connection.OnStreamOpened += StreamOpened;
        }

        // Fired when a new stream has been opened (It does not carry data with it)
        static void StreamOpened(QuicStream stream)
        {
            Console.WriteLine("UpStream Opened");
            if (streamup == null)
            {
                Console.WriteLine("UpStream Opened");
                stream.OnStreamDataReceived += StreamDataReceived;
                streamup = stream;
            }
            else if (streamdown == null)
            {
                Console.WriteLine("DownStream Opened");

                streamdown = stream;
            }
        }

        // Fired when a stream received full batch of data
        static void StreamDataReceived(QuicStream stream, byte[] data)
        {
            string decoded = Encoding.UTF8.GetString(data);

            Console.WriteLine("[S] Recv: " + decoded);

            // Send back data to the client on the same stream
            if (streamdown != null)
                streamdown.Send(data);
            else
                streamup.Send(data);
        }

        //// Start downlink stream
        //Thread.Sleep(300);
        //    streamdown = connection.CreateStream(QuickNet.Utilities.StreamType.ClientBidirectional);

        //    // Send data downstream
        //    Thread.Sleep(100);
        //    streamdown.Send(Encoding.UTF8.GetBytes("Ping back from server."));

        static void Main(string[] args)
        {
            QuicListener listener = new QuicListener(11000);
            listener.OnClientConnected += ClientConnected;

            listener.Start();

            //Console.ReadKey();
        }
    }
}
