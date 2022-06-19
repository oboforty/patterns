import operator
from random import random, uniform, randint, sample, shuffle
from math import sqrt, log, cos, pi, sin, ceil, exp, floor


def normal(mean=0, variance=1):
    U1 = random()
    U2 = random()

    Z = sqrt(-2 * log(U1)) * cos(2*pi*U2)
    return mean + variance * Z

def normalPoint(mean=0, variance=1):
    U1 = random()
    U2 = random()

    Z1 = sqrt(-2 * log(U1)) * cos(2*pi*U2)
    Z2 = sqrt(-2 * log(U1)) * sin(2*pi*U2)
    return mean + variance * Z1, mean + variance * Z2
