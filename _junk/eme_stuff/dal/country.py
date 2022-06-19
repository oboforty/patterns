from core.ctx import EntityBase
from core.dal.base.childmixin import ChildMixin, InstanceRepositoryBase
from core.dal.worlds import World


class Country(ChildMixin, EntityBase):
    __tablename__ = 'countries'

    __world__ = World

    def __init__(self, **kwargs):
        self.iso = kwargs.get("iso")
        self.wid = kwargs.get("wid")

        self.name = kwargs.get("name")

    @property
    def view(self):
        return {
            "iso": self.iso,
            "name": self.name,
            "wid": str(self.wid),
        }

    def __hash__(self):
        return hash(self.iso)

    def __repr__(self):
        return "Country #{}".format(self.iso)


from eme.data_access import Repository
from core.dal.users import User
from sqlalchemy import and_, or_


@Repository(Country)
class CountryRepository(InstanceRepositoryBase):
    def list_with_players_both(self, wid, wid2):
        """
        Lists all countries and player uids in both worlds, paired
        """
        return self.session.query(Country, User.uid) \
            .filter(or_(Country.wid == wid, Country.wid == wid2)) \
            .outerjoin(User, and_(User.iso == Country.iso, Country.wid == User.wid))

    def list_with_players(self, wid):
        """
        Lists countries that are associated with a user
        """
        return self.session.query(User.username, Country) \
            .filter(User.wid == wid) \
            .join(Country, and_(User.iso == Country.iso, User.wid == Country.wid))

    def list_without_players(self, wid):
        """
        lists countries that are empty
        """
        return self.session.query(Country) \
            .filter(Country.wid == wid) \
            .outerjoin(User, and_(User.iso == Country.iso, User.wid == Country.wid)) \
            .filter(User.iso == None)
