import random
from collections import defaultdict

from eme.data_access import get_repo

from core.dal.country import Country, CountryRepository
from core.dal.users import User, UserRepository
from core.dal.worlds import World


world_repo = get_repo(World)
user_repo: UserRepository = get_repo(User)
country_repo: CountryRepository = get_repo(Country)


class JoinException(Exception):
    def __init__(self, reason, par1=None):
        self.reason = reason
        self.par1 = par1

    def __str__(self):
        return str(self.reason)


class CountryPair:
    def __init__(self):
        self.country1 = None
        self.country2 = None
        self.uid1 = None
        self.uid2 = None

    def __iter__(self):
        return iter((self.country1, self.country2, self.uid1, self.uid2))


def merge_worlds(world: World, world2: World):
    players = country_repo.list_with_players_both(world.wid, world2.wid)
    country_pairs = defaultdict(CountryPair)

    # Assign country pairs of both countries in worlds
    for country, uid in players:
        p: CountryPair = country_pairs[country.iso]

        if country.wid == world.wid:
            p.country1 = country
            p.uid1 = uid
        else:
            p.country2 = country
            p.uid2 = uid

    redundants = []
    transfers = []

    # Go through each pair and evaluate
    for iso, (country1, country2, uid1, uid2) in country_pairs.items():
        if uid2:
            if uid1:
                # both countries are occupied; the worlds can't be merged
                redundants.append(country1.name)
                continue
            else:
                # we delete country1 first so that it's committed before transferring the new countries
                transfers.append(country2)
                country_repo.delete(country1, commit=False)

    # if we can't merge certain countries
    if redundants:
        # Redo changes and return with an error
        world_repo.session.rollback()
        raise JoinException("cant_merge", redundants)

    # Save deletes
    country_repo.save()

    # save transfers
    for country in transfers:
        country.wid = world.wid

    # Save transfers
    country_repo.save()

    user_repo.transfer_to(world2.wid, world.wid)
    world_repo.delete(world2)

    return True


def join_world(world, user, iso=None):
    # check if world is above limit
    if world.max_players <= len(list(country_repo.list_with_players(world.wid))):
        raise JoinException("max_players")

    # check if user is already joined
    # todo: check already connected from players...

    # check if spot is taken
    if iso:
        # player has selected a country
        player = user_repo.find_by_iso(iso, world.wid)

        if player:
            raise JoinException("country_taken")

        user.iso = iso
    else:
        # give random country to player
        _countries = list(country_repo.list_without_players(world.wid))

        user.iso = random.choice(_countries).iso

    # join world
    user.wid = world.wid
    user_repo.set_world(user.uid, user.wid, user.iso)
