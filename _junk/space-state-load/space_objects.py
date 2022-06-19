import astropy.units as u
from astropy.time import Time
from astropy.coordinates import solar_system_ephemeris, EarthLocation
from astropy.coordinates import get_body_barycentric, get_body, get_moon, SkyCoord
from sunpy.coordinates import get_body_heliographic_stonyhurst

from core.views.space_views import PlanetView

planet_list = ['earth', 'venus', 'mars', 'mercury', 'saturn', 'jupiter', 'neptune', 'uranus', 'sun']


def get_view(name, p):
    return PlanetView(
        name=name.title(),
        x=p.cartesian.x.to_value(u.au),
        y=p.cartesian.y.to_value(u.au),
        z=p.cartesian.z.to_value(u.au),

        lon=p.spherical.lon.to_value(),
        lat=p.spherical.lat.to_value(),
        radius=p.spherical.distance.to_value(),
    )


def list_planets(obstime = None):
    if obstime is None:
        obstime = Time.now()

    planets = [get_view(planet_name, get_body_heliographic_stonyhurst(planet_name, time=obstime)) for planet_name in planet_list]

    return obstime, planets
