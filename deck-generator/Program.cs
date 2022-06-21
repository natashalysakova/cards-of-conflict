using System.Text.Json;
using CardsOfConflict.Library.Model;

public class DeckGenerator
{
    static string whiteCardsPath;
    static string blackCardsPath;
    static string deckName;
    static string resultDir;

    /// -white {path} -black {path} -name {deckName} -dir {decksDir}
    public static void Main(string[] args)
    {
        if (args.Length != 8)
        {
            Console.WriteLine("Wrong arguments");
            return;
        }

        for (int i = 0; i < args.Length; i += 2)
        {
            var value = args[i + 1];
            switch (args[i])
            {
                case "-white":
                    whiteCardsPath = value;
                    break;
                case "-black":
                    blackCardsPath = value;
                    break;
                case "-name":
                    deckName = value;
                    break;
                case "-dir":
                    resultDir = value;
                    break;
            }
        }

        var whiteCards = GenerateCards(whiteCardsPath, (x) => { return new WhiteCard(x); });
        var blackCards = GenerateCards(blackCardsPath, (x) => { return new BlackCard(x); });

        whiteCards.ForEach(x => x.SetNewId());
        blackCards.ForEach(x => x.SetNewId());

        SaveToFile(whiteCards, "white.json", deckName, resultDir);
        SaveToFile(blackCards, "black.json", deckName, resultDir);

        Console.WriteLine("Done");
    }

    private static void SaveToFile<T>(List<T> cards, string fileName, string deckName, string resultDir) where T : Card
    {
        var fullDir = Path.Combine(resultDir, deckName);
        if (!Directory.Exists(fullDir))
            Directory.CreateDirectory(fullDir);

        var fullName = Path.Combine(fullDir, fileName);
        using (var sw = new StreamWriter(fullName, new FileStreamOptions() {
            Access = FileAccess.Write,
            Mode = FileMode.OpenOrCreate
        }))
        {
            var json = JsonSerializer.Serialize(cards);
            sw.Write(json);
        }
    }

    public static List<T> GenerateCards<T>(string path, Func<string, T> del) where T: Card
    {
        var strings = ReadStringsFromFile(path);

        return strings.Select(x => del(x)).ToList();
    }


    private static string[] ReadStringsFromFile(string path)
    {
        using (var sr = new StreamReader(path))
        {
            return sr.ReadToEnd().Split(Environment.NewLine);
        }
    }
}

