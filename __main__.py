#!/usr/bin/env python3

import json, nextcord, os, random, sys
from nextcord.ext import commands
from youtube_dl import YoutubeDL

from garf_data import GarfData
from util import inclusive_range, index_len, int_from_str, jsonc_loads

current_dir = os.getcwd()
cah_dir = os.path.join(current_dir, "cards_against_humanity")

if not os.path.exists(GarfData.file):
    print("garf_data.json was not found, exiting...")
    sys.exit()
data = GarfData()

bot = commands.Bot(command_prefix = "garf ")
context = commands.context.Context

@bot.event
async def on_ready():
    print("< GARFBOT ACTIVATED >")

@bot.event
async def on_message(ctx: context):
    msg = ctx.content.lower()

    if ctx.author.id == bot.user.id:
        return

    # React with :eyes: if someone mentions lasagna
    if "lasagna" in msg:
        await ctx.add_reaction("👀")

    # React with :rage: if someone mentions mondays
    if "monday" in msg:
        await ctx.add_reaction("😡")

    # Say a joke if someone says a trigger word
    data = GarfData()
    if any(trigger_word in msg for trigger_word in data.trigger_words):
        random.seed()
        await ctx.channel.send(data.jokes[random.randint(0, index_len(data.jokes))])

    await bot.process_commands(ctx)

# Commands

# Add a joke to garf_data
@bot.command()
async def add(ctx: context, joke: str):
    """Adds a joke to my list"""

    data = GarfData()

    if joke in data.jokes:
        await ctx.reply("i already know that one!")
        return

    data.jokes.append(joke)
    data.overwrite()

    await ctx.reply(f"added joke: \"{joke}\"")

# Remove a joke from garf_data
@bot.command()
async def remove(ctx: context, joke: str):
    """Removes a joke from my list"""

    data = GarfData()

    if not joke in data.jokes:
        await ctx.reply("i don't know that one")
        return

    data.jokes.remove(joke)
    data.overwrite()

    await ctx.reply(f"removed joke: \"{joke}\"")

# List all jokes from garf_data
@bot.command()
async def jokes(ctx: context):
    """Lists all my jokes"""

    data = GarfData()

    jokes = "```\n"
    for joke in data.jokes:
        jokes += f"{joke}\n"
    jokes += "```"

    await ctx.reply(jokes)

# Roll some dice
@bot.command()
async def roll(ctx: context, number: str, size: str):
    """Rolls (number) (size)-sided dice"""

    number = int_from_str(number)
    size = int_from_str(size)

    final_amount = 0
    final_message = "you rolled: "

    random.seed()
    first_roll = random.randint(1, size)
    final_amount += first_roll
    final_message += str(first_roll)

    for _ in inclusive_range(2, number):
        random.seed()
        roll = random.randint(1, size)
        final_amount += roll
        final_message += f" + {roll}"

    if number > 1:
        final_message += f" = {final_amount}"

    await ctx.reply(final_message)

# Join voice
@bot.command()
async def join(ctx: context):
    """Makes me join the voice channel you're in"""

    channel = ctx.author.voice.channel

    if channel is not None:
        if ctx.voice_client is not None:
            await ctx.voice_client.move_to(channel)
        else:
            await channel.connect()

# Leave voice
@bot.command()
async def leave(ctx: context):
    """Makes me leave the voice channel i'm in"""

    if ctx.voice_client is not None:
        await ctx.voice_client.disconnect()

# Play audio from a YouTube link
@bot.command()
async def play(ctx: context, link: str):
    """Plays audio from a YouTube link"""

    if ctx.voice_client is None:
        await ctx.reply("i'm not in a voice channel!")
        return

    if data.path_to_ffmpeg == "":
        await ctx.reply("you need to set my `path_to_ffmpeg` to use `play`")
        return

    youtube_dl_options = {
        "format": "bestaudio/best",
        "outtmpl": "download"
    }
    dl = YoutubeDL(youtube_dl_options)

    if os.path.exists(os.path.join(current_dir, "download")):
        os.remove(os.path.join(current_dir, "download"))

    await ctx.reply("downloading...")

    youtube_dl_data = await bot.loop.run_in_executor(None, lambda: dl.extract_info(link))
    file = dl.prepare_filename(youtube_dl_data)
    player = nextcord.FFmpegPCMAudio(file, **{ "options": "-vn" }, executable = data.path_to_ffmpeg)

    ctx.voice_client.play(player, after = lambda e: print(f'player error: {e}') if e else None)

# Start a game of Cards Against Humanity (WIP)
@bot.command()
async def start_game(ctx: context, chance: int = 15, *players: nextcord.User):
    """Start a new Cards Against Humanity game with (chance)% chance of a card being fill-in-the-blank (WIP)"""

    chance: float = chance / 100
    if chance < 0:
        chance = 0
    elif chance > 1:
        chance = 1

    await ctx.channel.send("Cards Against Humanity is made by Cards Against Humanity LLC; learn more at https://www.cardsagainsthumanity.com/")

    with open(os.path.join(cah_dir, "cards.jsonc"), "r") as f:
        cards_dict = jsonc_loads(f.read())

    game_file = os.path.join(cah_dir, "game.json")
    if os.path.exists(game_file):
        os.remove(game_file)

    game_dict = {
        "players": [],
        "black_cards_in_play": cards_dict["black_cards"],
        "white_cards_in_play": cards_dict["white_cards"]
    }
    for player in players:
        player_dict = {
            "id": player.id,
            "cards": []
        }
        for _ in inclusive_range(1, 10):
            random.seed()
            if random.random() < chance:
                white_card = "(FILLABLE)"
            else:
                random.seed()
                white_card = game_dict["white_cards_in_play"][random.randint(0, index_len(game_dict["white_cards_in_play"]))]
                game_dict["white_cards_in_play"].remove(white_card)
            player_dict["cards"].append(white_card)
        game_dict["players"].append(player_dict)

        player_message = "```"
        for i in inclusive_range(0, index_len(player_dict["cards"])):
            card = player_dict["cards"][i]
            player_message += f"{i} {card}\n"
        player_message += "```"
        await player.send(player_message)

    with open(game_file, "w") as f:
        f.write(json.dumps(game_dict, indent = 4) + "\n")

    await ctx.reply("this feature isn't fully implemented yet")

    # random.seed() ; czar = bot.get_user(game_dict["players"][random.randint(0, index_len(game_dict["players"]))]["id"])
    # random.seed() ; black_card = game_dict["black_cards_in_play"][random.randint(0, index_len(game_dict["black_cards_in_play"]))]
    # game_dict["black_cards_in_play"].remove(black_card)

    # await ctx.channel.send(f"the Card Czar is {czar.mention}, the black card is {black_card}")

bot.run(data.token)
