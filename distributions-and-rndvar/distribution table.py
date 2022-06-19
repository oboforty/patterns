def distribution(dist, sorted=False):
    u = random.uniform(0, 1)
    s = 0
    if not sorted:
        dist.sort(key=operator.itemgetter(1))

    for item, prob in dist:
      s += prob
      if s >= u:
          return item
    return item  # Might occur because of floating point inaccuracies
