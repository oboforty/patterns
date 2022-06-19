using System.Net;
using System.Threading.Tasks;

namespace GameServer
{
    class Program
    {
        static int fps = 30;
        static int delay;

        static UDPSocket simu;

        static void Main(string[] args)
        {
            delay = 1000 / fps;

            var mt2 = new MetinServer(fps);
            simu = new UDPSocket(mt2);
            var sgnServer = new TCPSocket(mt2);
            mt2.SetServers(simu, sgnServer);

            simu.StartServer(new IPEndPoint(IPAddress.Any, 1337));
            StartFrameContext();
            sgnServer.StartServer(new IPEndPoint(IPAddress.Any, 1338));
        }

        static async Task StartFrameContext()
        {
            while (true)
            {
                FrameContext();
                await Task.Delay(delay);
            }
        }

        static async Task FrameContext()
        {
            simu.SimulateFrame();
        }
    }
}
