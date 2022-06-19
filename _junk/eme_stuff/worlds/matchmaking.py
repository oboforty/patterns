import html
import json
import random
import string
import time
from collections import defaultdict

from eme.entities import load_settings

conn = None

mmk_party = "Parties"
mmk_queue = "Queues"

conf = load_settings('worlds')
PMAX = conf.get('search_max_players')
party_timeouts = {a: b for a,b in zip(conf.get('search_buckets', int), conf.get('search_timeouts', int))}
final_timeout = 5*60

def init(c):
    global conn
    conn = c


def create_party(uid, username):
    # party id is a 4 letter code
    party_id = (''.join(random.choice(string.ascii_lowercase) for i in range(4))).upper()

    party = {
        "in_queue": None,
        "created": time.time(),
        "uids": {username: uid}
    }

    conn.hset(mmk_party, party_id, json.dumps(party))

    return party_id, party


def get_party(party_id):
    g = conn.hget(mmk_party, party_id)

    if g is None:
        return None

    return json.loads(g)


def remove_party(party_id):
    conn.hdel(mmk_party, party_id)


def remove_queue(party_id):
    conn.hdel(mmk_queue, party_id)


def join_party(party_id, uid, username):
    g = conn.hget(mmk_party, party_id)

    if g is None:
        return False

    party = json.loads(g)

    # join and save the party
    party['uids'][username] = str(uid)
    conn.hset(mmk_party, party_id, json.dumps(party))

    return party


def leave_party(party_id, username):
    KEY = mmk_party
    g = conn.hget(KEY, party_id)

    if g is None:
        # try loading again, this time from the Queue
        KEY = mmk_queue
        g = conn.hget(KEY, party_id)

        if g is None:
            # party not found, neither in parties nor in the queue
            return False

    party = json.loads(g)

    del party['uids'][username]

    # save or delete the party
    if not party['uids']:
        conn.hdel(KEY, party_id)
    else:
        conn.hset(KEY, party_id, json.dumps(party))

    return True


def start_party(party_id):
    g = conn.hget(mmk_party, party_id)

    if g is None:
        return False

    # place party into queue and remove from parties list
    conn.hdel(mmk_party, party_id)

    # set queue status -> is now put into the redis queue
    party = json.loads(g)
    party['in_queue'] = int(time.time())
    conn.hset(mmk_queue, party_id, json.dumps(party))

    return True


def handle_matchmaking():
    parties = conn.hgetall(mmk_queue)

    if not parties:
        return []

    merged_parties, removed_parties, starting_parties = merge_parties(parties)

    # update merged parties in Redis
    if merged_parties:
        conn.hmset(mmk_queue, merged_parties)

    # remove the other half of merged parties
    if removed_parties:
        conn.hdel(mmk_queue, *removed_parties)

    return starting_parties


def merge_parties(parties):
    small_team = []     # N = 1,2,3
    large_team = []     # N = 4,5,6,7
    removed_teams = []
    merged_teams = {}
    start_parties = []

    now = time.time()

    for party_id, party_json in parties.items():
        party = party_json if isinstance(party_json, dict) else json.loads(party_json)

        N = len(party['uids'])
        timeout = party_timeouts.get(N, 10)
        time_spent = now - party['in_queue']

        # filter out parties that had their timeout
        if time_spent > final_timeout:
            # party stayed too long in queue
            # we assume they're gone, just remove it
            removed_teams.append(party_id)
        elif time_spent > timeout:
            # start party automatically
            start_parties.append(party)
            removed_teams.append(party_id)
            continue

        # bin each party into a bin based on party size
        if N <= 3: small_team.append((party_id, party))
        else: large_team.append((party_id, party))


    NL = len(large_team); NS = len(small_team)

    # balance out datasets
    if NL < NS / 2:
        to_add = round((NS - NL)/2)
        large_team.extend(small_team[NS-to_add:NS])
    elif NS < NL:
        # rare case, but let's cover it anyway

        # randomly split the dataset into 2
        random.shuffle(small_team)
        random.shuffle(large_team)

        all_team = small_team + large_team
        small_team, large_team = all_team[0:int(len(all_team)/2)], all_team[int(len(all_team)/2):len(all_team)]

    # merge parties based on merge-sort policy
    l = 0; s = 0

    while s < NS and l < NL:
        party1_id, party1 = small_team[s]
        party2_id, party2 = large_team[l]

        N = len(party1['uids']); M = len(party2['uids'])

        if N + M > PMAX:
            # move on to the next item in one of the two lists (decided randomly)

            if random.uniform() > 0.5: l += 1
            else: s += 1
            continue

        # merge two teams - update small team and remove the large one. party inherits earlier waiting time
        party1['uids'].extend(party2['uids'])
        party1['in_queue'] = min(party1['in_queue'], party2['in_queue'])
        removed_teams.append(party2_id)

        if len(party1['uids']) > 7:
            # party is ready to start
            removed_teams.append(party1_id)
            start_parties.append(party1)
        else:
            # keep party in queue
            merged_teams[party1_id] = party1

        # skip both; they're covered
        s += 1
        l += 1

    return merged_teams, removed_teams, start_parties


def clear_parties():
    removed_parties = []
    parties = conn.hgetall(mmk_party)
    now = time.time()

    for party_id, party_json in parties.items():
        party = party_json if isinstance(party_json, dict) else json.loads(party_json)

        # 1 hour
        if 'created' not in party or now - party['created'] > 60*60:
            removed_parties.append(party_id)

    if removed_parties:
        conn.hdel(mmk_party, *removed_parties)

    return True

def clear_queues():
    removed_parties = []
    parties = conn.hgetall(mmk_queue)
    now = time.time()

    for party_id, party_json in parties.items():
        party = party_json if isinstance(party_json, dict) else json.loads(party_json)

        # 1 hour
        if 'in_queue' not in party or now - party['in_queue'] > final_timeout:
            removed_parties.append(party_id)

    if removed_parties:
        conn.hdel(mmk_queue, *removed_parties)

    return True


def delete_all():
    conn.delete(mmk_party)
    conn.delete(mmk_queue)
