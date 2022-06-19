
namespace GameServer
{
    public interface IServer
    {
        void Send(int playerId, byte[] packet);
    }
}
