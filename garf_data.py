import json, os

class GarfData:
    garf_data_file = os.path.join(
        os.path.dirname(
            os.path.realpath(__file__)
        ),
        "garf_data.json"
    )

    def __init__(self):
        with open(self.garf_data_file, "r") as f:
            json_dict = json.loads(f.read())

        self.path_to_ffmpeg = json_dict["path_to_ffmpeg"]
        self.jokes = json_dict["jokes"]
        self.trigger_words = json_dict["trigger_words"]

    def create_new(self):
        json_dict = {
            "path_to_ffmpeg": "",
            "jokes": [],
            "trigger_words": []
        }
        with open(self.garf_data_file, "w") as f:
            f.write(json.dumps(json_dict, indent = 4) + "\n")

    def overwrite(self):
        json_dict = {
            "path_to_ffmpeg": self.path_to_ffmpeg,
            "jokes": self.jokes,
            "trigger_words": self.trigger_words
        }
        with open(self.garf_data_file, "w") as f:
            f.write(json.dumps(json_dict, indent = 4) + "\n")
