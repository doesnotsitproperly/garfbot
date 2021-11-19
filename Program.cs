﻿using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GarfBot {
    class Program {
        static void Main() {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync() {
            DiscordClient discord = new DiscordClient(new DiscordConfiguration() {
                Token = await File.ReadAllTextAsync(Path.Combine( Directory.GetCurrentDirectory(), "token.txt" )),
                TokenType = TokenType.Bot
            });

            Console.WriteLine("< GARFBOT ACTIVATED >");

            List<string> triggerWords = new List<string> {
                "bongo",
                "cum",
                "sex"
            };

            // dClient: DSharpPlus.DiscordClient, dEvent: DSharpPlus.EventArgs.MessageCreateEventArgs
            discord.MessageCreated += async (dClient, dEvent) => {
                string msg = dEvent.Message.Content.ToLower();

                // React w/ :eyes: if someone mentions lasagna
                if (msg.Contains("lasagna")) {
                    await dEvent.Message.CreateReactionAsync(DiscordEmoji.FromName(dClient, ":eyes:"));
                }

                // React w/ :rage: if someone mentions mondays
                if (msg.Contains("monday")) {
                    await dEvent.Message.CreateReactionAsync(DiscordEmoji.FromName(dClient, ":rage:"));
                }

                // Add a joke to "jokes.txt"
                if (msg.StartsWith("garf add ")) {
                    string joke = dEvent.Message.Content.Substring(9);

                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "jokes.txt");
                    using (StreamWriter sw = File.AppendText(filePath)) {
                        await sw.WriteAsync("\n" + joke);
                    }

                    DiscordMessage discordMessage = await new DiscordMessageBuilder()
                        .WithContent("added joke: " + "\"" + joke + "\"")
                        .SendAsync(dEvent.Channel);
                }
                // List all jokes from "jokes.txt"
                else if (msg.StartsWith("garf jokes")) {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "jokes.txt");
                    string[] fileContents = await File.ReadAllLinesAsync(filePath);

                    string jokes = "";
                    for (int i = 0; i < fileContents.Length; i++) {
                        jokes += i.ToString() + " " + fileContents[i] + "\n";
                    }

                    DiscordMessage discordMessage = await new DiscordMessageBuilder()
                        .WithContent(jokes)
                        .SendAsync(dEvent.Channel);
                }
                // Say a joke if someone says a trigger word
                else if (triggerWords.Any(word => msg.Contains(word))) {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "jokes.txt");
                    string[] jokes = await File.ReadAllLinesAsync(filePath);

                    DiscordMessage discordMessage = await new DiscordMessageBuilder()
                        .WithContent(jokes[new Random().Next(0, jokes.Length)])
                        .SendAsync(dEvent.Channel);
                }
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}