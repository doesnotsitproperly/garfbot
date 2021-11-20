using DSharpPlus.CommandsNext.Attributes;

public class CommandModule : BaseCommandModule {
    string jokesFile = Path.Combine(Directory.GetCurrentDirectory(), "jokes.txt");
    
    // Add a joke to "jokes.txt"
    [Command("add")]
    public async Task AddCommand(CommandContext ctx, string joke) {
        using (StreamWriter sw = File.AppendText(jokesFile)) {
            await sw.WriteAsync("\n" + joke);
        }
        await ctx.RespondAsync("added joke: " + "\"" + joke + "\"");
    }

    // Remove a joke to "jokes.txt"
    [Command("remove")]
    public async Task RemoveCommand(CommandContext ctx, int joke) {
        await ctx.RespondAsync(@"not yet implemented :\");
    }

    // List all jokes from "jokes.txt"
    [Command("jokes")]
    public async Task JokesCommand(CommandContext ctx) {
        string[] fileContents = await File.ReadAllLinesAsync(jokesFile);
        string jokes = "";
        for (int i = 0; i < fileContents.Length; i++) {
            jokes += i.ToString() + " " + fileContents[i] + "\n";
        }
        await ctx.RespondAsync(jokes);
    }

    // Roll some dice
    [Command("roll")]
    public async Task RollCommand(CommandContext ctx, string arg1, string arg2) {
        string strAmount = "";
        foreach (char character in arg1) {
            if (Char.IsNumber(character)) {
                strAmount += character;
            }
        }
        int amount = Int32.Parse(strAmount);
        string strDice = "";
        foreach (char character in arg2) {
            if (Char.IsNumber(character)) {
                strDice += character;
            }
        }
        int dice = Int32.Parse(strDice);

        int finalAmount = 0;
        string finalMessage = "you rolled: ";

        int firstRoll = new Random().Next(1, dice + 1);
        finalAmount += firstRoll;
        finalMessage += firstRoll.ToString();

        for (int i = 2; i <= amount; i++) {
            int roll = new Random().Next(1, dice + 1);
            finalAmount += roll;
            finalMessage += " + " + roll.ToString();
        }

        if (amount > 1) {
            finalMessage += " = " + finalAmount.ToString();
        }

        await ctx.RespondAsync(finalMessage);
    }
}
