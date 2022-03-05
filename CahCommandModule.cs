using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

public class CahCommandModule : BaseCommandModule {
    string cahDir = Path.Combine(Directory.GetCurrentDirectory(), "Cah");

    string cardCzar;
    List<Tuple<string, string>> submittedCards;

    [Command("cahstart")]
    public async Task CahStartCommand(CommandContext ctx, params DiscordMember[] names) {
        File.Copy(
            Path.Combine(cahDir, "Cards.json"),
            Path.Combine(cahDir, "CardsInPlay.json"),
            true
        );

        JsonObject players = new JsonObject();
        foreach (DiscordMember name in names) {
            string jsonText = File.ReadAllText(Path.Combine(cahDir, "CardsInPlay.json"));
            JsonNode jsonNode = JsonNode.Parse(jsonText);

            List<string> whiteCards = new List<string>();
            foreach (string whiteCard in jsonNode["whiteCards"].AsArray()) {
                whiteCards.Add(whiteCard);
            }

            JsonArray cards = new JsonArray();
            for (int i = 0; i < 10; i++) {
                int card = new Random().Next(0, whiteCards.Count);
                cards.Add(whiteCards[card]);
                whiteCards.RemoveAt(card);
            }

            JsonObject player = new JsonObject();
            player.Add("cards", cards);
            player.Add("score", 0);
            players.Add(name.Id.ToString(), player);

            JsonArray whiteCardsArray = new JsonArray();
            foreach (string whiteCard in whiteCards) {
                whiteCardsArray.Add(whiteCard);
            }

            string blackCardsString = jsonNode["blackCards"].ToJsonString();
            JsonArray blackCardsArray = (JsonArray)JsonNode.Parse(blackCardsString);

            JsonObject cardsInPlayObject = new JsonObject {
                ["blackCards"] = blackCardsArray,
                ["whiteCards"] = whiteCardsArray
            };
            string cardsInPlayString = cardsInPlayObject.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
            using (StreamWriter sw = new StreamWriter(Path.Combine(cahDir, "CardsInPlay.json"))) {
                await sw.WriteAsync(cardsInPlayString);
            }

            string message = "```\n";
            for (int i = 0; i < cards.Count; i++) {
                message += $"{i} {cards[i]}\n";
            }
            message += "```";
            await name.SendMessageAsync(message);
        }

        JsonObject playersObject = new JsonObject {
            ["players"] = players
        };
        string playersString = playersObject.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        using (StreamWriter sw = new StreamWriter(Path.Combine(cahDir, "Players.json"))) {
            await sw.WriteAsync(playersString);
        }

        submittedCards = new List<Tuple<string, string>>();

        string playersText = File.ReadAllText(Path.Combine(cahDir, "Players.json"));
        JsonNode playersNode = JsonNode.Parse(playersText);
        JsonArray playersArray = playersNode["players"].AsArray();
        cardCzar = (string)playersArray[new Random().Next(playersArray.Count + 1)];
    }

    [Command("cahsubmitcard")]
    public async Task CahSubmitCard(CommandContext ctx, int card) {
        string id = ctx.Message.Id.ToString();

        string jsonText = File.ReadAllText(Path.Combine(cahDir, "Players.json"));
        JsonNode jsonNode = JsonNode.Parse(jsonText);

        if (!jsonNode["players"].AsArray().Contains(id)) {
            await ctx.RespondAsync("cannot find you in the list of players :/");
            return;
        }

        if (cardCzar == id) {
            return;
        }

        foreach (Tuple<string, string> submittedCard in submittedCards) {
            if (submittedCard.Item2 == id) {
                return;
            }
        }

        JsonArray cardArray = jsonNode["players"][id]["cards"].AsArray();
        submittedCards.Add(new Tuple<string, string>((string)cardArray[card], id));
    }
}
