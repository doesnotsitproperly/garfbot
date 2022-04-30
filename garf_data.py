import json, os

class GarfData:
    file = os.path.join(os.getcwd(), "garf_data.json")

    token: str
    path_to_ffmpeg: str
    jokes: list
    trigger_words: list

    def __init__(self):
        with open(self.file, "r") as f:
            json_dict = json.loads(f.read())

        self.token = json_dict["token"]
        self.path_to_ffmpeg = json_dict["path_to_ffmpeg"]
        self.jokes = json_dict["jokes"]
        self.trigger_words = json_dict["trigger_words"]

    def overwrite(self):
        json_dict = {
            "token": self.token,
            "path_to_ffmpeg": self.path_to_ffmpeg,
            "jokes": self.jokes,
            "trigger_words": self.trigger_words
        }
        with open(self.file, "w") as f:
            f.write(json.dumps(json_dict, indent = 4) + "\n")
