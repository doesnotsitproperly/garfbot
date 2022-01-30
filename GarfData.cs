using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;

public class GarfData {
    public List<string> jokes { get; set; }
    public List<string> triggerWords { get; set; }

    public GarfData() {
        string jsonText = File.ReadAllText(
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "GarfData.json"
            )
        );
        JsonNode jsonNode = JsonNode.Parse(jsonText);

        jokes = new List<string>();
        foreach (string joke in jsonNode["jokes"].AsArray()) {
            jokes.Add(joke);
        }

        triggerWords = new List<string>();
        foreach (string triggerWord in jsonNode["triggerWords"].AsArray()) {
            triggerWords.Add(triggerWord);
        }
    }
}
