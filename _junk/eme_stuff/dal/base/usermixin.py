from time import time

from sqlalchemy import Column, TIMESTAMP, func, Boolean, SmallInteger, String, ForeignKey, Integer
from flask_login import UserMixin as FlaskUserMixin
from eme.data_access import GUID, JSON_GEN


class UserMixin(FlaskUserMixin):
    # login
    uid = Column(GUID(), primary_key=True)
    username = Column(String(32))

    # profile
    admin = Column(Boolean(), default=False)
    face = Column(JSON_GEN())
    points = Column(SmallInteger)
    last_active = Column(TIMESTAMP, server_default=func.now(), onupdate=func.current_timestamp())

    # oauth
    access_token = Column(String(255), unique=True, nullable=False)
    refresh_token = Column(String(255), index=True)
    issued_at = Column(Integer, nullable=False, default=lambda: int(time()))
    expires_in = Column(Integer, nullable=False, default=0)


    def get_id(self):
        return str(self.uid) if self.uid else None

    @property
    def is_active(self):
        return True

    @property
    def is_authenticated(self):
        return self.uid is not None

    @property
    def is_anonymous(self):
        return False

    def get_user_id(self):
        # used for oauth2
        return self.uid

    def __hash__(self):
        return hash(self.uid)

    def __repr__(self):
        return "{}({}..)".format(self.username, str(self.uid)[0:4])


