import uuid

from sqlalchemy import Column, TIMESTAMP, func, SmallInteger, String
from eme.data_access import GUID


class WorldMixin:
    # Worlds module
    wid = Column(GUID(), primary_key=True, default=uuid.uuid4)
    created_at = Column(TIMESTAMP, server_default=func.now())
    name = Column(String(20))
    map = Column(String(20))
    max_players = Column(SmallInteger)

    last_update = Column(TIMESTAMP, server_default=func.now(), onupdate=func.current_timestamp())
    invlink = Column(String(6))


    def __repr__(self):
        return "World({})".format(self.invlink)


class TurnMixin():
    turn_time = Column(SmallInteger)
    turns = Column(SmallInteger)
    rounds = Column(SmallInteger)
    current = Column(SmallInteger)
    max_rounds = Column(SmallInteger)
    #isos = Column(SmallInteger)
