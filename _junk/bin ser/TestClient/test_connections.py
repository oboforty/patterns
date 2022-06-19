import time
import signal
import sys

#ip_as_bytes = bytes(map(int, addr[0].split('.')))
from tcp_client import TcpClient
from udp_client import UdpClient


def handledc(tcp, udp):
    def dcfunc(err=None):
        if tcp:
            tcp.close()
        if udp:
            udp.close()

        if not err:
            print("Disconnected!")
        else:
            print("Disconnected! Reason:", err)

        sys.exit(0)

    return dcfunc


if __name__ == "__main__":
    addr = ("127.0.0.1", 1337)
    player_id = 1

    # create TCP & UDP handshakes
    tcp = TcpClient(addr, player_id)
    udp = UdpClient(addr, player_id)

    disconnect = handledc(tcp, udp)

    # handle disconnects
    def signal_handler(signal, frame):
        disconnect()
    tcp.ondisconnect += [disconnect]
    udp.ondisconnect += [disconnect]
    signal.signal(signal.SIGINT, signal_handler)

    # connect to services
    udp.conn_hash = tcp.connect()
    time.sleep(0.1)
    udp.connect()
