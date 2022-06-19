import json
import string
import uuid
import random

from core.dal.country import Country
from core.dal.users import User
from core.dal.worlds import World


with open('core/content/countries.json') as fh:
    default_countries: dict = json.load(fh)

    country_template = default_countries.pop('__template__')


def create_world(owner: User, invlink=None, name=None, iso=None, max_players=None, map=None):
    """
    Creates world and makes the owner join

    :param owner:
    :param invlink:
    :param name:
    :param iso:
    :param max_players:
    :return:
    """

    if name is None:
        if owner.admin:
            # admins do not formally own worlds
            name = "Geopoly and chill"
        else:
            name = "{}'s world".format(owner.username)

    if invlink is None:
        invlink = create_invlink()

    if max_players is None:
        max_players = len(default_countries)

    world = World(name=name, invlink=invlink, max_players=max_players, map=map)
    world.wid = create_wid()
    world.invlink = invlink

    # xrefs
    world.owner_id = owner.uid
    owner.wid = world.wid

    # create countries
    countries = [Country(wid=world.wid, iso=iso, **cd, **country_template) for iso, cd in default_countries.items()]

    if iso is None:
        owner.iso = random.choice(countries).iso
    else:
        owner.iso = iso

    return world, countries


def create_invlink():
    letters = string.ascii_uppercase
    return ''.join(random.choices(letters, k=6))


def create_wid():
    return uuid.uuid4()
