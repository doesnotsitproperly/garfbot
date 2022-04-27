import json, os

class GarfData:
    def __init__(self) -> None:
        garf_data_file = os.path.join(
            os.path.dirname(
                os.path.realpath(__file__)
            ),
            "garf_data.json"
        )
        with open(garf_data_file, "r") as f:
            json_dict = json.loads(f.read())

        self.path_to_ffmpeg = json_dict["pathToFfmpeg"]
        self.jokes = json_dict["jokes"]
        self.trigger_words = json_dict["triggerWords"]
