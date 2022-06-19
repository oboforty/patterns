import sys
import re
import io
from collections import defaultdict

def colorize(color, text):
    return color + text + NO_COLOR

REGEX_LINE = re.compile('.*\s(.?)/Unity.*\):\s\[([a-zA-Z0-9_\-]*)\]\s?(.*)')
RED="\033[1;31m"
YELLOW="\033[1;33m"
NO_COLOR="\033[0m"

"""
Black       0;30     Dark Gray     1;30
Blue        0;34     Light Blue    1;34
Green       0;32     Light Green   1;32
Cyan        0;36     Light Cyan    1;36
Red         0;31     Light Red     1;31
Purple      0;35     Light Purple  1;35
Brown       0;33     Yellow        1;33
Light Gray  0;37     White         1;37
"""

get_color = defaultdict(lambda: NO_COLOR)
get_color.update({
    # Contexts - dark green
    "GameInstaller": "\033[1;32m",
    "SceneInstaller": "\033[1;32m",
    "ValidateTownScript": "\033[1;32m",
    "TownTaskScheduler": "\033[1;33m",

    # GUI - yellow
    # "":"1;33",

    # Scene Objects / Game Data - Blue
    "Town": "\033[1;34m",
    "World": "\033[1;34m",
    "Soldier": "\033[1;34m",

# --------------------------------------
    # Server - purple
    "ServerThread": "\033[0;35m",
    "Server Thread": "\033[0;35m",
    "FixtureAuth": "\033[1;35m",
    "FixtureServer": "\033[1;35m",
    "ServerMainThread": "\033[1;35m",
    "ServerCodeExecuter": "\033[1;35m",

    # Service - etc
    "GameManager": "\033[1;36mm",
    "DataLoader": "\033[1;36m",

    # Login related - cyan
    "ValidateLogin": "\033[1;36m",
    "Login": "\033[1;36m",
    "LoginMenuScript": "\033[1;36m",
})

_filter = {"EGL"}

steam = sys.stdin
#steam = io.StringIO(strrr)

while True:
    line = steam.readline().rstrip()

    if 'Unity' not in line:
        continue
    g = REGEX_LINE.match(line)

    if g:
        log_ind,log_group,log_msg = g.groups()

        if log_group in _filter:
            continue

        sys.stdout.write(colorize(get_color[log_group], f"{log_msg} ({log_group})\n"))
        sys.stdout.flush()
    elif "Error" in line or "Exception" in line:
        sys.stdout.write(colorize(RED, line))
        sys.stdout.flush()
