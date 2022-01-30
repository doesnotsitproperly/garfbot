using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

public class CommandModule : BaseCommandModule {
    string currentDir = Directory.GetCurrentDirectory();
    string garfDataFile = Path.Combine(Directory.GetCurrentDirectory(), "GarfData.json");

    // Init GarfData
    GarfData data = new GarfData();

    // Add a joke to GarfData
    [Command("add")]
    public async Task AddCommand(CommandContext ctx, string joke) {
        JsonArray jokesArray = new JsonArray();
        foreach (string j in data.jokes) {
            jokesArray.Add(j);
        }
        jokesArray.Add(joke);

        JsonArray triggerWordsArray = new JsonArray();
        foreach (string w in data.triggerWords) {
            triggerWordsArray.Add(w);
        }

        JsonObject jsonObject = new JsonObject {
            ["jokes"] = jokesArray,
            ["triggerWords"] = triggerWordsArray
        };
        string jsonString = jsonObject.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        using (StreamWriter sw = new StreamWriter(garfDataFile)) {
            await sw.WriteAsync(jsonString);
        }

        data = new GarfData();
    
        await ctx.RespondAsync($"added joke: \"{joke}\"");
    }

    // Remove a joke from GarfData
    [Command("remove")]
    public async Task RemoveCommand(CommandContext ctx, int joke) {
        JsonArray jokesArray = new JsonArray();
        foreach (string j in data.jokes) {
            jokesArray.Add(j);
        }
        jokesArray.RemoveAt(joke);

        JsonArray triggerWordsArray = new JsonArray();
        foreach (string w in data.triggerWords) {
            triggerWordsArray.Add(w);
        }

        JsonObject jsonObject = new JsonObject {
            ["jokes"] = jokesArray,
            ["triggerWords"] = triggerWordsArray
        };
        string jsonString = jsonObject.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        using (StreamWriter sw = new StreamWriter(garfDataFile)) {
            await sw.WriteAsync(jsonString);
        }

        data = new GarfData();
    
        await ctx.RespondAsync($"removed joke {joke}");
    }

    // List all jokes from GarfData
    [Command("jokes")]
    public async Task JokesCommand(CommandContext ctx) {
        string jokes = "";
        for (int i = 0; i < data.jokes.Count; i++) {
            jokes += $"{i} {data.jokes[i]}\n";
        }
        await ctx.RespondAsync(jokes);
    }

    //
    // Dice-rolling commands
    //

    private int GetInt(string s) {
        string newString = "";
        foreach (char c in s) {
            if (Char.IsNumber(c)) {
                newString += c;
            }
        }
        return Int32.Parse(newString);
    }

    // Roll some dice
    [Command("roll")]
    public async Task RollCommand(CommandContext ctx, string arg1, string arg2) {
        int amount = GetInt(arg1);
        int dice = GetInt(arg2);

        int finalAmount = 0;
        string finalMessage = "you rolled: ";

        int firstRoll = new Random().Next(1, dice + 1);
        finalAmount += firstRoll;
        finalMessage += firstRoll.ToString();

        for (int i = 2; i <= amount; i++) {
            int roll = new Random().Next(1, dice + 1);
            finalAmount += roll;
            finalMessage += $" + {roll}";
        }

        if (amount > 1) {
            finalMessage += $" = {finalAmount}";
        }

        await ctx.RespondAsync(finalMessage);
    }

    //
    // VoiceNext Commands
    //

    private bool DownloadAudio(string link) {
        Process youTube = new Process() {
            StartInfo = new ProcessStartInfo {
                FileName = "youtube-dl",
                Arguments = $"--no-playlist -f bestaudio/best -o ''download/download.%(ext)s'' {link}",
                UseShellExecute = false
            }
        };
        
        if (youTube.Start()) {
            youTube.WaitForExitAsync();
            youTube.Dispose();
            return true;
        } else {
            youTube.Dispose();
            return false;
        }
    }

    private Stream ConvertAudio(string filePath) {
        Process ffmpeg = new Process() {
            StartInfo = new ProcessStartInfo {
                FileName = "ffmpeg",
                Arguments = $"-i \"{filePath}\" -ac 2 -f wav -ar 48000 -y pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };
        ffmpeg.Start();
        return ffmpeg.StandardOutput.BaseStream;
    }

    [Command("join")]
    public async Task JoinCommand(CommandContext ctx) {
        VoiceNextExtension voiceNext = ctx.Client.GetVoiceNext();

        if (ctx.Member.VoiceState.Channel != null) {
            DiscordChannel channel = ctx.Member.VoiceState.Channel;

            if (voiceNext.GetConnection(ctx.Guild) != null) {
                voiceNext.GetConnection(ctx.Guild).Disconnect();
            }

            // string[] downloadFiles = Directory.GetFiles(Path.Combine(currentDir, "download"));
            // foreach (string f in downloadFiles) {
            //     File.Delete(f);
            // }

            VoiceNextConnection connection = await channel.ConnectAsync();
        }
    }

    [Command("play")]
    public async Task PlayCommand(CommandContext ctx, string link) {
        VoiceNextExtension voiceNext = ctx.Client.GetVoiceNext();
        VoiceNextConnection connection = voiceNext.GetConnection(ctx.Guild);
        VoiceTransmitSink transmit = connection.GetTransmitSink();

        if (DownloadAudio(link)) {
            string downloadFile = Directory.GetFiles(Path.Combine(currentDir, "download"))[0];
            using (Stream pcm = ConvertAudio(downloadFile)) {
                await pcm.CopyToAsync(transmit);
            }
        } else {
            await ctx.RespondAsync("failed to download");
        }
    }

    [Command("leave")]
    public async Task LeaveCommand(CommandContext ctx) {
        VoiceNextExtension voiceNext = ctx.Client.GetVoiceNext();
        VoiceNextConnection connection = voiceNext.GetConnection(ctx.Guild);
        connection.Disconnect();

        // Stop an annoying warning that this method doesn't use "async"
        await File.ReadAllTextAsync(garfDataFile);
    }
}
