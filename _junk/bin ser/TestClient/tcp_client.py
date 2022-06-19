import os
import socket
import event


class TcpClient:
    so: socket.socket
    bufferSize = 1024 * 8
    tcp_addr: tuple
    connected: bool
    player_id: int

    onconnect = event.Event()
    ondisconnect = event.Event()

    def __init__(self, addr, player_id):
        self.tcp_addr = (addr[0], addr[1] + 1)
        self.connected = False
        self.player_id = player_id

    def connect(self):
        self.so = socket.socket(family=socket.AF_INET, type=socket.SOCK_STREAM)
        self.so.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

        try:
            self.so.connect(self.tcp_addr)
        except ConnectionRefusedError:
            return self.ondisconnect("tcp_server_unavailable")

        conn_hash = os.urandom(1)[0]
        handshake = bytes([1, self.player_id, conn_hash])

        try:
            self.so.sendto(handshake, self.tcp_addr)
            self.so.settimeout(1)
            data, cli_addr = self.so.recvfrom(self.bufferSize)

            assert data == b'\x0F\x1F\x2F'

            print('=TCP Connected')
            self.connected = True
            self.onconnect()

            return conn_hash
        except socket.timeout:
            return self.ondisconnect("TCP timed out")
        except AssertionError:
            return self.ondisconnect("Wrong handshake data received")

    def close(self):
        print("TCP Disconnected")
        self.so.close()
