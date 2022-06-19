from math import floor

from game.entities import Hero
from game.services.heroes import change_face_slightly


def expected_death(health):
    # returns expected age of death based on current health
    """ Health is based on a geometric distribution [0, 1]
    """

    healthyiest = 0.012
    sickest = 0.04

    sick = 1-health

    # use geometric distribution with variable p_sick
    p_sick = sick*(sickest-healthyiest) + healthyiest
    exp_death_age = int(1/p_sick)

    return exp_death_age


def increment_age(hero):
    hero.age += 1

    exp_death = expected_death(hero.health)

    if exp_death >= hero.age:
        # todo: create custom exception for this
        raise Exception("Hoplite has died!")

    return hero.age


def simulate_age(hero: Hero, dt):
    elapsed_days = dt / 3600.0 / 24.0

    if elapsed_days < 1:
        return None

    # Hoplite's age has increased

    hero.age += floor(elapsed_days)
    death_age = expected_death(hero.health)

    # by aging your face changes too
    change_face_slightly(hero)

    if hero.age >= death_age:
        # Hoplite has died
        hero.health = 0

        return True

    return False
