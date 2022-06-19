import datetime



def start_timeout(world):
    time_left = settings.get('turns.user_timeout', int, 60)
    dt = datetime.timedelta(seconds=time_left)
    now = datetime.datetime.utcnow()

    world.user_timeout = now + dt

    return dt


def check_timeout(world, current_iso):

    if current_iso is None or world.current != current_iso:
        return False

    if world.user_timeout is None:
        return False

    now = datetime.datetime.utcnow()

    # see if current user has exceeded its timeout date
    return now >= world.user_timeout
