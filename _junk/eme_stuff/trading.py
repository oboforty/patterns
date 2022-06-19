from game.instance import towns
from game.entities import TradeOffer
from game.services.building import MaterialException, buildings_conf


class TradeException(Exception):
    def __init__(self, reason, harbor_score_left):
        self.reason = reason
        # Note: can be negative
        self.trade_score_left = harbor_score_left

    def __str__(self):
        return self.reason + str(self.trade_score_left)


def create_offer(town, to_town_iso, material, gold, is_sell):
    """
    :param town: town making the offer
    :param to_town_iso: ISO of the town that should receive it
    :param material: {string: integer} string being name, integer being amount
    :param gold: integer of gold
    :return: current total trading rating
    """
    print(town.resources)
    harbor_attribute = 0
    if 'harbor' in town.buildings:
        harbor_attribute = buildings_conf['harbor']['attri'][town.buildings['harbor']]

    total_trading = tradeoffers.summarize_offers(town.iso, town.wid)

    if total_trading + material[1] > harbor_attribute:
        raise TradeException("Not_enough_trade_score", harbor_attribute - total_trading + material[1])

    if is_sell:
        if town.resources[material[0]] < material[1]:
            raise MaterialException({material[0]: material[1]-town.resources[material[0]]})
        print(town.resources[material[0]] - material[1])
        town.resources[material[0]] = town.resources[material[0]] - material[1]
        towns.mark_changed(town)
    else:
        if town.resources['gold'] < gold:
            raise MaterialException({'gold': gold-town.resources['gold']})
        else:
            town.resources['gold'] -= gold
            towns.mark_changed(town)

    new_trade = TradeOffer(iso=to_town_iso, wid=town.wid, from_iso=town.iso, materials_name=material[0]\
                           ,material_amount=material[1], gold=gold, is_sell=is_sell)
    tradeoffers.create(new_trade)
    print(town.resources)
    towns.save()
    return True


def cancel_offer(town, trade_id):
    trade_offer = tradeoffers.get(trade_id, town.wid)
    if trade_offer.iso != town.iso:
        raise TradeException("not_owned", None)
    if trade_offer.is_sell:
        town.resources[trade_offer.material_name] += trade_offer.material_amount
    else:
        town.resources['gold'] += trade_offer.gold
    tradeoffers.delete(trade_offer)
    towns.mark_changed(town)
    towns.save()
    return True


def accept_offer(town, trade_id):
    trade_offer = tradeoffers.get_by_id(trade_id)

    if not trade_offer or trade_offer.wid != town.wid:
        raise TradeException("Non_Existant", 0)

    if trade_offer.is_sell:
        if town.resources['gold'] < trade_offer.gold:
            raise MaterialException({'gold': trade_offer.gold-town.resources['gold']})
        material_wanting = ['gold', trade_offer.gold]
        material_giving = [trade_offer.material_name, trade_offer.material_amount]
    else:
        if town.resources[trade_offer.material_name] < trade_offer.material_amount:
            raise MaterialException({trade_offer.material_name: trade_offer.material_amount-town.resources['gold']})
        material_wanting = [trade_offer.material_name, trade_offer.material_amount]
        material_giving = ['gold', trade_offer.gold]

    town.resources[material_giving[0]] += material_giving[1]
    town.resources[material_wanting[0]] -= material_wanting[1]
    tradeoffers.delete(trade_offer)
    towns.mark_changed(town)
    towns.save()
    return True
