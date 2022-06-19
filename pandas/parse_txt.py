import re
import csv
from datetime import datetime

_sess = {}
skipped = 0
prevtime = None

dice_rgx = re.compile(r".*gets \*\*(\d)\*\* and \*\*(\d)\*\*")
msg_rgx = re.compile(r":game_die: (.*?)( bets| gets|, your|, you|, it's| rolls) (.*)$")


def parse_userline(line):
    splt = line.split('] ')
    _date = splt[0][1:]
    _date = datetime.strptime(_date, '%d-%b-%y %I:%M %p')
    _user = splt[1].split('#')[0]

    return _date, _user


def fetch_dice(text):
    match = dice_rgx.match(text)

    if match:
        d1 = match.group(1)
        d2 = match.group(2)

        return int(d1),int(d2)
    else:
        raise Exception("gec " + text)

_sess_keys = ['date','stake','user','outcome','dice11','dice12','dice21','dice22']


with open('data.csv', 'w', encoding='utf8', newline='') as fw:
    with open('botch.txt', encoding='utf8') as fh:
        i = 0

        csv_writer = csv.DictWriter(fw, delimiter='|', quotechar='"', quoting=csv.QUOTE_MINIMAL, fieldnames=_sess_keys)
        csv_writer.writeheader()

        for line in fh:
            if ']' in line:
                try:
                    prevtime, _mee6 = parse_userline(line)
                except:
                    pass

            if not line.startswith(':game_die:'):
                continue

            # in-session
            line = line.replace(':astonished: ', '')

            match = msg_rgx.search(line)
            if not match:
                print('   !', line)
                continue

            username = match.group(1)
            resp = match.group(2).lstrip(' ,')
            restline = match.group(3)

            if resp == 'bets':
                # new session
                sess = dict.fromkeys(_sess_keys)

                sess['date'] = prevtime
                sess['stake'] = int(restline.split(':existentialdread:')[0])
                sess['user'] = username
                _sess[username] = sess
            else:
                sess = _sess[username]

            if resp == 'gets':
                sess['dice11'], sess['dice12'] = fetch_dice(line)
            elif resp == 'your':
                sess['dice21'], sess['dice22'] = fetch_dice(restline)
            else:
                # outcomes:
                if '**two 6s**' in restline:
                    sess['dice11'], sess['dice12'] = 6,6
                    sess['dice21'], sess['dice22'] = 0,0
                    sess['outcome'] = 'W'
                elif '**lost**' in restline:
                    sess['outcome'] = 'L'
                elif '**won**' in restline:
                    sess['outcome'] = 'W'
                elif "it's a draw" in line:
                    sess['outcome'] = 'D'
                else:
                    if 'bets' not in line:
                        print(">>?<<", line)
                    continue

                # end of session, mark and close
                csv_writer.writerow(sess)
                del _sess[username]

            i+=1

            # if i > 50000:
            #     break
