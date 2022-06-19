import json
from flask import request

from game.instance import towns

from game.services import building

from webapp.entities import ApiResponse
from webapp.services.login import getUser


class TradeApiController():
    def __init__(self, server):
        self.server = server
        self.group = "TownsApi"

        self.server.addUrlRule({
            'GET /api/town/offers': 'tradeapi/offers',
            'GET /api/offers': 'tradeapi/all_offers',
        })

    def get_offers(self):
        """
        Lists all incoming and outgoing offers for user
        """
        user = getUser()

        incoming = tradeoffers.list_for_me(user.iso, user.wid)
        outgoing = tradeoffers.list_in_town(user.iso, user.wid)

        return ApiResponse({
            "incoming": [of.to_dict() for of in incoming],
            "outgoing": [of.to_dict() for of in outgoing]
        })

    def get_all_offers(self):
        """
        Lists all offers in user's world
        """
        user = getUser()

        offers = tradeoffers.list_all(user.wid)

        return ApiResponse([of.to_dict() for of in offers])
