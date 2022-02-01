﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

public class Program {
    static void Main() {
        MainAsync().GetAwaiter().GetResult();
    }

    static async Task MainAsync() {
        string currentDir = Directory.GetCurrentDirectory();
        string garfDataFile = Path.Combine(currentDir, "GarfData.json");
        string tokenFile = Path.Combine(currentDir, "TOKEN");

        // Check if files exist
        if (!File.Exists(tokenFile)) {
            Console.WriteLine("TOKEN file not found, exiting...");
            Environment.Exit(-1);
        }
        if (!File.Exists(garfDataFile)) {
            Console.WriteLine("GarfData.json not found; a new one will be created...");
            JsonObject jsonObject = new JsonObject {
                ["jokes"] = new JsonArray(),
                ["triggerWords"] = new JsonArray()
            };
            string jsonString = jsonObject.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
            using (StreamWriter sw = new StreamWriter(garfDataFile)) {
                await sw.WriteAsync(jsonString);
            }
        }

        // Get token from TOKEN file
        string token;
        using (StreamReader sr = new StreamReader(tokenFile)) {
            token = await sr.ReadLineAsync();
        }

        // Init Discord stuff
        DiscordClient discord = new DiscordClient(new DiscordConfiguration() {
            Token = token,
            TokenType = TokenType.Bot
        });
        discord.UseVoiceNext();
        CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration() {
            StringPrefixes = new string[] { "garf", "garf " }
        });
        commands.RegisterCommands<CommandModule>();
        commands.RegisterCommands<CahCommandModule>();
        Console.WriteLine("< GARFBOT ACTIVATED >");

        discord.MessageCreated += async (dClient, dEvent) => {
            string msg = dEvent.Message.Content.ToLower();

            // Don't do anything if the message author is GarfBot
            if (dEvent.Author.Id == dClient.CurrentUser.Id) {
                return;
            }

            // React w/ :eyes: if someone mentions lasagna
            if (msg.Contains("lasagna")) {
                await dEvent.Message.CreateReactionAsync(DiscordEmoji.FromName(dClient, ":eyes:"));
            }

            // React w/ :rage: if someone mentions mondays
            if (msg.Contains("monday")) {
                await dEvent.Message.CreateReactionAsync(DiscordEmoji.FromName(dClient, ":rage:"));
            }

            // Say a joke if someone says a trigger word
            GarfData data = new GarfData();
            if (data.triggerWords.Any(msg.Contains)) {
                DiscordMessage discordMessage = await new DiscordMessageBuilder()
                    .WithContent(data.jokes[new Random().Next(0, data.jokes.Count)])
                    .SendAsync(dEvent.Channel);
            }
        };

        await discord.ConnectAsync();
        await Task.Delay(-1);
    }
}
