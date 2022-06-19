from collections import defaultdict
from os.path import dirname, realpath, join

from eme.entities import load_settings
from eme.websocket import WebsocketApp
from chatapp.services import auth, startup



class GeopolyChat(WebsocketApp):

    def __init__(self):
        # eme/examples/simple_website is the working directory.
        script_path = dirname(realpath(__file__))
        conf = load_settings(join(script_path, 'config.ini'))

        super().__init__(conf, script_path)

        startup.init(self)
        auth.init(self, conf['auth'])

        self.world_clients = defaultdict(set)

    def get_clients_at(self, wid: str):
        for client in self.world_clients[str(wid)]:
            yield client

    async def send_to_world(self, wid: str, rws: dict, route=None, msid=None, isos=None):
        clients = self.world_clients.get(str(wid))

        if clients:
            if isos is not None:
                for client in clients:
                    if client.user and client.user.iso in isos:
                        await self.send(rws, client)
            else:
                for client in clients:
                    await self.send(rws, client, route=route, msid=msid)

    # start threads
    # for tname, tcontent in self.threads.items():
    #     thread = threading.Thread(target=tcontent.run)
    #     thread.start()

    # def do_reconnect(self, client):
    #     if not client.user:
    #         return
    #
    #     # remove redundant old clients by the same user
    #     if client.user.wid:
    #         clients = self.onlineMatches[str(client.user.wid)]
    #
    #         for cli in list(clients):
    #             if cli == client:
    #                 # my current client, skip
    #                 continue
    #
    #             if cli.user and cli.user.uid == client.user.uid:
    #                 # client has the same uid, but is not my current client
    #                 # -> remove it
    #                 #print("Reconnect: ", cli.id, '->', client.id)
    #                 clients.remove(cli)
    #
    #     if client.user.wid:
    #         self.client_enter_world(client)
    #     else:
    #         self.onlineMatches[str(client.user.wid)].add(client)



if __name__ == "__main__":
    app = GeopolyChat()
    app.start()
