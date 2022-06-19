import uuid

from sqlalchemy import Column, TIMESTAMP, func, SmallInteger, String, ForeignKey
from eme.data_access import GUID, RepositoryBase
from sqlalchemy.ext.declarative.base import declared_attr
from sqlalchemy.orm import relationship

from core.dal.worlds import World


class ChildMixin:
    iso = Column(String(3), primary_key=True)
    name = Column(String(20))

    @declared_attr
    def wid(self):
        return Column(GUID(), ForeignKey(self.__world__.wid), primary_key=True)




class InstanceRepositoryBase(RepositoryBase):

    def count(self, wid=None):
        if wid:
            return self.session.query(self.T)\
                .filter(self.T.wid == wid)\
            .count()
        else:
            return self.session.query(self.T).count()

    def get(self, eid, wid):
        return super().get([eid, wid])

    def list_all(self, wid=None):
        if wid:
            return self.session.query(self.T)\
                .filter(self.T.wid == wid)\
            .all()
        else:
            return self.session.query(self.T).all()

    def delete_all(self, wid=None, commit=True):
        if wid:
            self.session.query(self.T)\
                .filter(self.T.wid == wid)\
            .delete()
        else:
            self.session.query(self.T).delete()

        if commit:
            self.session.commit()
