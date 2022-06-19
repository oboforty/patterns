def uniformEllipse(lat, lon, R):
    angle = uniform(0, pi*2)

    if isinstance(R, float) or isinstance(R, int):
        return lat + cos(angle) * R, lon + sin(angle) * R
    return lat + cos(angle) * R[0], lon + sin(angle) * R[1]
 
def uniformRing(lat, lon, R1, R2):
    t = 2*pi*random()

    dist = sqrt(random() * (R2**2 - R1**2) + R1**2)
    return lat + dist * cos(t), lon + dist * sin(t)
