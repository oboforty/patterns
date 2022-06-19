"""
Policies:

                user                            entity
admin           admin=1                         -
owner(wid)      user.wid == entity.wid          world
owner(iso)      user.iso == entity.iso          country, entity, etc
owner(entity)   user.uid == entity.owner_id     anything
member(wid)     user.wid == entity.wid          world
anyone          is_authenticated                -
"""
from core.dal.worlds import World


def verify(policy, user, entity=None):
    if user is None:
        return False
    elif user.admin:
        # admins can do everything
        return True
    elif 'king' == policy:
        return entity.iso == user.iso
    elif 'member' == policy:
        return entity.wid == user.wid
    elif 'worldless' == policy:
        return user.wid is None
    elif 'owner' == policy:
        return entity.owner_id == user.uid and entity.wid == user.wid
    elif 'anyone' == policy:
        return user.is_authenticated
    else:
        raise Exception("Policy not found: {}".format(policy))


def can_edit(world: World, attr):
    if not hasattr(world, attr):
        return False

    if attr not in ['map', 'name', 'invlink', 'max_players']:
        return False

    return True
