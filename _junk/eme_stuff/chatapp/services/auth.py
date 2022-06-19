from time import time

import requests
from authlib.integrations.requests_client import OAuth2Auth

from core.dal.users import User
from eme.auth import UserManager
from eme.data_access import get_repo

user_manager = None
user_repo = None
conf = None


def init(app, c):
    global user_repo, user_manager, conf

    conf = c
    user_repo = get_repo(User)
    user_manager = UserManager(user_repo)


def load_user(uid, token):
    if uid is None or uid == 'None':
        return None

    user: User = user_repo.get(uid)

    if user is None:
        user = fetch_user(token)

        if user is None:
            return None

    # validate if this access token is still valid
    if user.expires_at < time():
        # delete expired user cache
        user_repo.delete(user)
        return None

    return user


def fetch_user(access_token):

    doors_url = conf['doors_url']

    token_auth = OAuth2Auth({
        'token_type': 'bearer',
        'access_token': access_token
    })

    r = requests.get(doors_url + "/api/me", headers={
        "access_token": access_token
    }, auth=token_auth)

    if r.status_code != 200:
        return None

    user = User(**r.json())

    return user

