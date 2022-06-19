from time import time

from core.ctx import EntityBase
from core.dal.base.usermixin import UserMixin
from eme.data_access import Repository, RepositoryBase, GUID
from sqlalchemy.orm import relationship
from sqlalchemy import Column, TIMESTAMP, func, Boolean, SmallInteger, String, ForeignKey, and_

from core.dal.worlds import World


class User(UserMixin, EntityBase):
    __tablename__ = 'users'

    iso = Column(String(5))
    wid = Column(GUID(), ForeignKey('worlds.wid', ondelete='CASCADE'))
    world = relationship(World, foreign_keys=[wid])

    def __init__(self, **kwargs):
        self.uid = kwargs.get('uid')
        self.username = kwargs.get('username')
        self.admin = kwargs.get('admin')
        self.face = kwargs.get('face')
        self.points = kwargs.get('points')
        self.last_active = kwargs.get('last_active')
        self.access_token = kwargs.get('access_token')
        self.refresh_token = kwargs.get('refresh_token')
        self.issued_at = kwargs.get('issued_at')
        self.expires_in = kwargs.get('expires_in')

        self.wid = kwargs.get("wid")
        self.iso = kwargs.get("iso")

        if not isinstance(self.admin, bool):
            self.admin = int(self.admin) == 1

    @property
    def expires_at(self):
        return self.issued_at + self.expires_in

    @property
    def view(self):
        return {
            "uid": self.uid,
            "username": self.username,
            "admin": self.admin,
            "face": self.face,
            "points": self.points,
            "last_active": self.last_active,
            "access_token": self.access_token,
            "refresh_token": self.refresh_token,
            "issued_at": self.issued_at,
            "expires_in": self.expires_in,
            "wid": self.wid,
            "iso": self.iso,
        }


from core.dal.country import Country


@Repository(User)
class UserRepository(RepositoryBase):

    def set_world(self, uid, wid, iso, commit=True):
        self.session.query(User)\
            .filter(User.uid == uid)\
            .update({User.wid: wid, User.iso: iso})

        if commit:
            self.session.commit()

    def find_by_iso(self, iso, wid):
        return self.session.query(User)\
            .filter(User.wid == wid, User.iso == iso)\
        .first()

    def list_all(self, wid=None, towns=False):
        if wid:
            return self.session.query(User) \
                .filter(User.wid == wid) \
                .all()
        else:
            if towns:
                return self.session.query(User, Country)\
                    .outerjoin(Country, and_(User.iso == Country.iso, User.wid == Country.wid))\
                .all()
            else:
                return self.session.query(User).all()

    def list_names(self, wid=None):
        return self.session.query(User.iso, User.username) \
            .filter(User.wid == wid) \
            .all()

    def transfer_to(self, wid1, wid2, commit=True):
        """
        Transfer users from one world into another
        :param wid1: from world
        :param wid2: to world
        :param commit: wether to commit automatically
        """

        self.session.query(User)\
            .filter(User.wid == wid1)\
            .update({User.wid: wid2})

        if commit:
            self.session.commit()

    def list_some(self, N):
        return self.session.query(User)\
            .limit(N)\
        .all()

    def find_by_token(self, token):
        return self.session.query(User)\
            .filter(User.access_token == token)\
        .first()

    def delete_inactive(self):
        self.session.query(User)\
            .filter(User.last_active + 14*24*3600 < time())\
        .delete(synchronize_session=False)
        self.session.commit()

    def create(self, ent, commit=True):
        user = self.session.query(User).filter(User.uid == ent.uid).first()
        if user is not None:
            self.session.delete(user)
            self.session.commit()

        super().create(ent, commit)
