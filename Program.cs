global using DSharpPlus;
global using DSharpPlus.CommandsNext;

using DSharpPlus.Entities;

class Program {
    static void Main() {
        MainAsync().GetAwaiter().GetResult();
    }

    static async Task MainAsync() {
        string tokenFile = Path.Combine(Directory.GetCurrentDirectory(), "token.txt");
        string jokesFile = Path.Combine(Directory.GetCurrentDirectory(), "jokes.txt");

        if (!File.Exists(tokenFile)) {
            Console.WriteLine("Could not find token file, exiting...");
            Environment.Exit(-1);
        }
        if (!File.Exists(jokesFile)) {
            Console.WriteLine("Jokes file not found; a new one will be created...");
            using (StreamWriter sw = File.AppendText(jokesFile)) {
                await sw.WriteAsync("");
            }
        }

        DiscordClient discord = new DiscordClient(new DiscordConfiguration() {
            Token = await File.ReadAllTextAsync(tokenFile),
            TokenType = TokenType.Bot
        });
        CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration() {
            StringPrefixes = new string[] { "garf", "garf " }
        });
        commands.RegisterCommands<CommandModule>();

        Console.WriteLine("< GARFBOT ACTIVATED >");

        List<string> triggerWords = new List<string> {
            "bongo",
            "cum",
            "sex"
        };

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

            // Amogus
            if (msg.Contains("among us") || msg.Contains("amogus") || msg.Contains("sus")) {
                await dEvent.Message.CreateReactionAsync(DiscordEmoji.FromName(dClient, ":regional_indicator_a:"));
                await Task.Delay(500);
                await dEvent.Message.CreateReactionAsync(DiscordEmoji.FromName(dClient, ":regional_indicator_m:"));
                await Task.Delay(500);
                await dEvent.Message.CreateReactionAsync(DiscordEmoji.FromName(dClient, ":regional_indicator_o:"));
                await Task.Delay(500);
                await dEvent.Message.CreateReactionAsync(DiscordEmoji.FromName(dClient, ":regional_indicator_g:"));
                await Task.Delay(500);
                await dEvent.Message.CreateReactionAsync(DiscordEmoji.FromName(dClient, ":regional_indicator_u:"));
                await Task.Delay(500);
                await dEvent.Message.CreateReactionAsync(DiscordEmoji.FromName(dClient, ":regional_indicator_s:"));
            }

            // Say a joke if someone says a trigger word
            if (triggerWords.Any(word => msg.Contains(word))) {
                string[] jokes = await File.ReadAllLinesAsync(jokesFile);
                DiscordMessage discordMessage = await new DiscordMessageBuilder()
                    .WithContent(jokes[new Random().Next(0, jokes.Length)])
                    .SendAsync(dEvent.Channel);
            }
        };

        await discord.ConnectAsync();
        await Task.Delay(-1);
    }
}
