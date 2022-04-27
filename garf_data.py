import json, os

class GarfData:
    def __init__(self):
        garf_data_file = os.path.join(
            os.path.dirname(
                os.path.realpath(__file__)
            ),
            "garf_data.json"
        )
        with open(garf_data_file, "r") as f:
            json_dict = json.loads(f.read())

        self.path_to_ffmpeg = json_dict["path_to_ffmpeg"]
        self.jokes = json_dict["jokes"]
        self.trigger_words = json_dict["trigger_words"]

    def create_new():
        garf_data_file = os.path.join(
            os.path.dirname(
                os.path.realpath(__file__)
            ),
            "garf_data.json"
        )

        json_dict = {
            "path_to_ffmpeg": "",
            "jokes": [],
            "trigger_words": []
        }
        with open(garf_data_file, "w") as f:
            f.write(json.dumps(json_dict, indent = 4) + "\n")
