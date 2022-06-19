import json
from collections import OrderedDict, defaultdict
import os
import shutil
import re
from math import ceil, sqrt
from itertools import product

from PIL import Image


spriteDim = 9

def sorted_alphanumeric(data):
    convert = lambda text: int(text) if text.isdigit() else text.lower()
    alphanum_key = lambda key: [ convert(c) for c in re.split('([0-9]+)', key) ] 
    return sorted(data, key=alphanum_key)

for dim in [64,70,128]:
    # discover all icons in folder:
    folder = f'{dim}x{dim}'
    if not os.path.isdir(folder):
        continue
    files = sorted_alphanumeric(os.listdir(folder))

    # eric andre
    iW = dim; iH = dim
    nX = min(len(files),spriteDim)
    nY = ceil(len(files) / nX)
    new_im = Image.new('RGBA', (iW * nX, iH * nY))

    for i,file in enumerate(files):
        im = Image.open('{}/{}'.format(folder, file))
        aspect = round(im.width / im.height, 1)

        if im.width != iW or im.height != iH:
            raise Exception(f"Image size mismatch: {im.width}x{im.height} != {iW}x{iH}")

        y = i // spriteDim
        x = i % spriteDim

        new_im.paste(im, (x * iW, y * iH))

    new_im.save(f'{folder}.png')
