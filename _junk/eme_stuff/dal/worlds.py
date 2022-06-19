from eme.data_access import GUID, Repository, RepositoryBase
from sqlalchemy import Column, ForeignKey
from sqlalchemy.orm import relationship

from core.ctx import EntityBase
from core.dal.base.worldmixin import WorldMixin


class World(WorldMixin, EntityBase):
    __tablename__ = 'worlds'

    owner_id = Column(GUID(), ForeignKey('users.uid', ondelete='CASCADE'))
    #owner = relationship(User, foreign_keys=[owner_id])

    def __init__(self, **kwargs):
        self.wid = kwargs.get('wid')
        self.name = kwargs.get('name')
        self.map = kwargs.get('map')
        self.max_players = kwargs.get('max_players')
        self.invlink = kwargs.get('invlink')
        self.owner_id = kwargs.get('owner_id')

    @property
    def view(self):
        return {
            "wid": self.wid,
            "name": self.name,
            "map": self.map,
            "max_players": self.max_players,
            "invlink": self.invlink,
            "owner_id": self.owner_id,
            "created_at": self.created_at,
            "last_update": self.last_update,
        }


@Repository(World)
class WorldRepository(RepositoryBase):

    def get_first(self):
        return self.session.query(World).first()

    def find_by_invite(self, invlink):
        return self.session.query(World)\
            .filter(World.invlink == invlink)\
        .first()
