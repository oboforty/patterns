import html
import json

from core.ctx import get_redis


class ChatHandler:
    tpl_room = "World:{}:Chat:{}:{}"
    KEEP_TEXTS = 10

    def __init__(self):
        self.conn = get_redis()

    def join(self, user, room_id):
        wrid = self.tpl_room.format(user.wid, room_id, 'sub')
        self.conn.sadd(wrid, user.iso)

    def leave(self, user, room_id):
        wrid = self.tpl_room.format(user.wid, room_id, 'sub')
        self.conn.srem(wrid, user.iso)

    def get_messages(self, wid, room_id):
        wmsgid = self.tpl_room.format(wid, room_id, 'msg')
        messages = self.conn.lrange(wmsgid, 0, 10)

        return [json.loads(s) for s in messages]

    def add_message(self, user, room_id, msg):
        isos = self.get_members(user.wid, room_id)

        if not user.iso in isos:
            return None

        wmsgid = self.tpl_room.format(user.wid, room_id, 'msg')
        if self.conn.llen(wmsgid) > self.KEEP_TEXTS:
            self.conn.lpop(wmsgid)

        msg_obj = {
            "username": user.username,
            "iso": user.iso,
            "msg": html.escape(msg)
        }

        self.conn.rpush(wmsgid, json.dumps(msg_obj))

        return msg_obj

    def get_members(self, wid, room_id):
        wrid = self.tpl_room.format(wid, room_id, 'sub')
        isos = [iso.decode('utf-8') for iso in self.conn.smembers(wrid)]

        return isos

    def clear(self, wid, room_id):
        wrid = self.tpl_room.format(wid, room_id, 'sub')

        wmsgid = self.tpl_room.format(wid, room_id, 'msg')

        self.conn.delete(wrid)
        self.conn.delete(wmsgid)
