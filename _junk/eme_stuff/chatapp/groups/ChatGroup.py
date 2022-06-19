from core.worlds.chat import ChatHandler


class ChatGroup:

    def __init__(self, app):
        self.app = app
        self.service = ChatHandler()


    async def subscribe(self, request, user, client):
        room_id = request.data['room_id']

        if not user.wid:
            return

        self.service.join(user, room_id)
        isos = self.service.get_members(user.wid, room_id)

        # add client
        self.app.world_clients[str(user.wid)].add(client)

        await self.app.send_to_world(user.wid, {
            "route": "Chat:subscribed",
            "username": user.username,
            "iso": user.iso,
        }, isos=isos)

        messages = self.service.get_messages(user.wid, room_id)

        return {
            "messages": messages,
            "users": isos
        }

    async def unsubscribe(self, request, user, client):
        room_id = request.data['room_id']

        if not user.wid:
            return

        self.service.leave(user, room_id)
        isos = self.service.get_members(user.wid, room_id)

        # remove client
        self.app.world_clients[str(user.wid)].remove(client)

        await self.app.send_to_world(user.wid, {
            "route": "Chat:unsubscribed",
            "username": user.username,
            "iso": user.iso,
        }, isos=isos)

        return {
            "username": user.username,
            "iso": user.iso,
        }

    async def message(self, request, user):
        room_id = request.data['room_id']
        msg = request.data['msg']

        if not user.wid:
            return

        msg_obj = self.service.add_message(user, room_id, msg)
        isos = self.service.get_members(user.wid, room_id)

        if msg_obj is not None:
            await self.app.send_to_world(user.wid, {
                "route": "Chat:message",
                "room_id": room_id,
                "msg": msg_obj,
            }, isos=isos)
