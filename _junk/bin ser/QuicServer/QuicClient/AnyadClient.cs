using System;
using System.Text;
using QuicNet;
using QuicNet.Connections;
using QuicNet.Streams;
using System.Threading;


namespace Anyad.Gecije
{
    class Program
    {
        static protected Thread m_RequestThread;
        static QuicStream streamdown;
        static QuicStream streamup;

        static void Main(string[] args)
        {
            Thread.Sleep(300);

            // Connection
            QuicClient client = new QuicClient();
            QuicConnection connection = client.Connect("127.0.0.1", 11000);

            // Stream UP
            streamup = connection.CreateStream(QuickNet.Utilities.StreamType.ClientBidirectional);
            streamup.OnStreamDataReceived += StreamDataReceived;

            streamup.Send(Encoding.UTF8.GetBytes("Hello from Client!"));
            streamup.Receive();

            Thread.Sleep(50);

            // Stream Down
            streamdown = connection.CreateStream(QuickNet.Utilities.StreamType.ClientBidirectional);
            streamdown.OnStreamDataReceived += StreamDataReceived;

            streamup.Send(Encoding.UTF8.GetBytes("Hello"));
            streamup.Receive();

            Thread.Sleep(50);


            Console.ReadKey();
        }


        static void StreamDataReceived(QuicStream stream, byte[] data)
        {
            // Fired when a stream received full batch of data
            string decoded = Encoding.UTF8.GetString(data);
            Console.WriteLine("[C] Recv:" + decoded);
        }

        private static void DownStreamOpened(QuicStream stream)
        {
            Console.WriteLine("Kaki");
            streamdown = stream;
            stream.OnStreamDataReceived += StreamDataReceived;

            return;


            // Wait reponse back from the server
            byte[] data = streamup.Receive();
        }

        static bool isRunning = true;

        static void t_RunRequestThread()
        {
            while (true)
            {
                if (streamdown is null)
                    Thread.Sleep(200);
                else
                    break;
            }
            Console.WriteLine("Starting thread");

            while (isRunning)
            {
                byte[] data = streamdown.Receive();
                //Console.WriteLine(Encoding.UTF8.GetString(data));
            }
        }

    }
}
