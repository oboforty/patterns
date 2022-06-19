from eme.entities import loadConfig
from eme.website import WebsiteApp


class CeleryApi(WebsiteApp):


    def __init__(self):
        # eme/examples/simple_website is the working directory.
        conf = loadConfig('webapi/config.ini')

        super().__init__(conf)
