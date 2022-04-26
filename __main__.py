#!/usr/bin/env python3

import json, os, sys
from garf_data import GarfData
from nextcord.ext import commands
from util import get_int, get_range, random_number

current_dir = os.path.dirname(os.path.realpath(__file__))
garf_data_file = os.path.join(current_dir, "garf_data.json")
token_file = os.path.join(current_dir, "TOKEN")

# Check if files exist
if not os.path.exists(token_file):
    print("TOKEN file was not found, exiting...")
    sys.exit()
elif not os.path.exists(garf_data_file):
    print("garf_data.json file was not found, exiting...")
    sys.exit()

bot = commands.Bot(command_prefix = "garf ")
data = GarfData()

@bot.event
async def on_ready():
    print("< GARFBOT ACTIVATED >")

@bot.event
async def on_message(ctx: commands.context.Context):
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
async def add(ctx: commands.context.Context, joke: str):
    data = GarfData()
    data.jokes.append(joke)

    json_dict = {
        "jokes": data.jokes,
        "triggerWords": data.trigger_words
    }
    with open(garf_data_file, "w") as f:
        f.write(json.dumps(json_dict, indent = 4) + "\n")

    data = GarfData()

    await ctx.reply(f"added joke: \"{joke}\"")

# Remove a joke from garf_data
@bot.command()
async def remove(ctx: commands.context.Context, joke: str):
    data = GarfData()
    data.jokes.remove(joke)

    json_dict = {
        "jokes": data.jokes,
        "triggerWords": data.trigger_words
    }
    with open(garf_data_file, "w") as f:
        f.write(json.dumps(json_dict, indent = 4) + "\n")

    data = GarfData()

    await ctx.reply(f"remove joke \"{joke}\"")

# List all jokes from garf_data
@bot.command()
async def jokes(ctx: commands.context.Context):
    data = GarfData()

    jokes = "```\n"
    for joke in data.jokes:
        jokes += f"{joke}\n"
    jokes += "```"

    await ctx.reply(jokes)

# Roll some dice
@bot.command()
async def roll(ctx: commands.context.Context, arg_1: str, arg_2: str):
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

with open("TOKEN", "r") as f:
    bot.run(f.readline())
