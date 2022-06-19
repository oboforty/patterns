from dataclasses import dataclass


@dataclass
class PlanetView():
    name: str

    x: float
    y: float
    z: float

    lat: float
    lon: float
    radius: float
