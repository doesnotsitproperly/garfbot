#!/usr/bin/env python3

import os, nextcord, sys
from garf_data import GarfData
from nextcord.ext import commands
from nextcord.ext.commands.context import Context
from util import get_int, get_range, random_number
from youtube_dl import YoutubeDL

current_dir = os.path.dirname(os.path.realpath(__file__))
cah_dir = os.path.join(current_dir, "cards_against_humanity")

if not os.path.exists(GarfData.file):
    print("garf_data.json was not found, exiting...")
    sys.exit()
data = GarfData()

bot = commands.Bot(command_prefix = "garf ")

@bot.event
async def on_ready():
    print("< GARFBOT ACTIVATED >")

@bot.event
async def on_message(ctx: Context):
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
        await ctx.channel.send(data.jokes[random_number(0, len(data.jokes) - 1)])

    await bot.process_commands(ctx)

# Commands

# Add a joke to garf_data
@bot.command()
async def add(ctx: Context, joke: str):
    data = GarfData()

    if joke in data.jokes:
        await ctx.reply("i already know that one!")
        return

    data.jokes.append(joke)
    data.overwrite()

    await ctx.reply(f"added joke: \"{joke}\"")

# Remove a joke from garf_data
@bot.command()
async def remove(ctx: Context, joke: str):
    data = GarfData()

    if not joke in data.jokes:
        await ctx.reply("i don't know what one")
        return

    data.jokes.remove(joke)
    data.overwrite()

    await ctx.reply(f"removed joke: \"{joke}\"")

# List all jokes from garf_data
@bot.command()
async def jokes(ctx: Context):
    data = GarfData()

    jokes = "```\n"
    for joke in data.jokes:
        jokes += f"{joke}\n"
    jokes += "```"

    await ctx.reply(jokes)

# Roll some dice
@bot.command()
async def roll(ctx: Context, arg_1: str, arg_2: str):
    amount = get_int(arg_1)
    dice = get_int(arg_2)

    final_amount = 0
    final_message = "you rolled: "

    first_roll = random_number(1, dice)
    final_amount += first_roll
    final_message += str(first_roll)

    for _ in get_range(2, amount):
        roll = random_number(1, dice)
        final_amount += roll
        final_message += f" + {roll}"

    if amount > 1:
        final_message += f" = {final_amount}"

    await ctx.reply(final_message)

# @bot.command()
# async def start_game(ctx: Context, *players: nextcord.User):
#     with open(os.path.join(cah_dir, "cards.json"), "r") as f:
#         cards_dict = json.loads(f.read())
# 
#     game_file = os.path.join(cah_dir, "game.json")
# 
#     game_dict = {
#         "players": []
#     }
#     for player in players:
#         pass

# Join voice
@bot.command()
async def join(ctx: Context):
    channel = ctx.author.voice.channel

    if channel != None:
        if ctx.voice_client != None:
            await ctx.voice_client.move_to(channel)
        else:
            await channel.connect()

# Leave voice
@bot.command()
async def leave(ctx: Context):
    if ctx.voice_client != None:
        await ctx.voice_client.disconnect()

# Play audio from a YouTube link
@bot.command()
async def play(ctx: Context, link: str):
    if ctx.voice_client == None:
        await ctx.reply("i'm not in a voice channel!")
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

bot.run(data.token)
