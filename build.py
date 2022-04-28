#!/usr/bin/env python3

import json, os, shutil, subprocess, sys

# This script will build a zipapp with built-in dependencies

current_dir = os.getcwd()
build_dir = os.path.join(current_dir, "build")
garfbot_dir = os.path.join(build_dir, "garfbot")

if os.path.isdir(build_dir):
    shutil.rmtree(build_dir)
os.mkdir(build_dir)
os.mkdir(garfbot_dir)

shutil.copytree(
    os.path.join(current_dir, "cards_against_humanity"),
    os.path.join(build_dir, "cards_against_humanity")
)
shutil.copy(os.path.join(current_dir, "garf_data.json"), build_dir)

with open(os.path.join(build_dir, "garf_data.json"), "r") as f:
    data = json.loads(f.read())
    data["path_to_ffmpeg"] = ""
with open(os.path.join(build_dir, "garf_data.json"), "w") as f:
    f.write(json.dumps(data, indent = 4) + "\n")

shutil.copy(os.path.join(current_dir, "__main__.py"), garfbot_dir)
shutil.copy(os.path.join(current_dir, "garf_data.py"), garfbot_dir)
shutil.copy(os.path.join(current_dir, "util.py"), garfbot_dir)

subprocess.check_call([sys.executable, "-m", "pip", "install", "nextcord[voice]", "youtube-dl", "--target", garfbot_dir])
subprocess.check_call([sys.executable, "-m", "zipapp", garfbot_dir, "--output", os.path.join(build_dir, "garfbot.pyz")])

shutil.rmtree(garfbot_dir)
