import socket
import event
import asyncio


class UdpEndPoint(asyncio.BaseProtocol):
    MAX_ALLOWED_FRAME_SKIP = 10
    onconnect = None
    ondisconnect = None
    connected = False
    bufferSize = 1024 * 8

    def __init__(self, addr, player_id, conn_hash):
        self.addr = addr
        self.handshake = bytes([0x01, player_id, conn_hash])
        self.transport = None

    def connection_made(self, transport):
        self.transport = transport
        self.transport.sendto(self.handshake, self.addr)

    def datagram_received(self, data, addr):
        # todo: UDP sends to itself


        if data == b'\x0F\x1F\x2F':
            print('-UDP Connected')
            self.connected = True
            return self.onconnect()
        elif not self.connected:
            self.ondisconnect("UDP failed handshake")
            return

        print('UDP Received', addr, ''.join('\\x'+format(x, '02x') for x in data))

        #print('Send %r to %s' % (message, addr))
        #self.transport.sendto(data, addr)


class UdpClient:
    so: socket.socket
    addr: tuple
    connected: bool
    conn_hash: bytes
    player_id: int
    transport = None
    loop = None

    onconnect = event.Event()
    ondisconnect = event.Event()

    def __init__(self, addr, player_id):
        self.addr = addr
        self.connected = False
        self.player_id = player_id

    def connect(self):
        self.loop = asyncio.get_event_loop()

        local_addr = ('127.0.0.7', 12007)

        # One protocol instance will be created to serve all client requests
        listen = self.loop.create_datagram_endpoint(self.create_socket, local_addr=local_addr)
        self.transport, protocol = self.loop.run_until_complete(listen)

        try:
            self.loop.run_forever()
        except KeyboardInterrupt:
            self.close()

    def create_socket(self):
        ep = UdpEndPoint(self.addr, self.player_id, self.conn_hash)
        ep.onconnect = self.onconnect
        ep.ondisconnect = self.ondisconnect

        return ep

    def close(self):
        print("UDP Disconnected")
        if self.transport:
            self.transport.close()
            if self.loop:
                self.loop.close()
