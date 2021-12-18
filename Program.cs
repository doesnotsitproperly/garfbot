global using DSharpPlus;
global using DSharpPlus.CommandsNext;
global using DSharpPlus.Entities;
global using DSharpPlus.VoiceNext;

class Program {
    static void Main() {
        MainAsync().GetAwaiter().GetResult();
    }

    static async Task MainAsync() {
        string currentDir = Directory.GetCurrentDirectory();
        string tokenFile = Path.Combine(currentDir, "token.txt");
        string jokesFile = Path.Combine(currentDir, "jokes.txt");
        string triggerWordsFile = Path.Combine(currentDir, "trigger_words.txt");

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
        discord.UseVoiceNext();
        CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration() {
            StringPrefixes = new string[] { "garf", "garf " }
        });
        commands.RegisterCommands<CommandModule>();

        Console.WriteLine("< GARFBOT ACTIVATED >");

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
            /* if (msg.Contains("among us") || msg.Contains("amogus")) {
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
            } */

            // Say a joke if someone says a trigger word
            string[] triggerWords = await File.ReadAllLinesAsync(triggerWordsFile);
            if (triggerWords.Any(msg.Contains)) {
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
