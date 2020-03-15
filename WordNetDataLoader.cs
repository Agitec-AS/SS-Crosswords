using System.IO;
using System.Text;
using System.Text.Json;

namespace Crosswords
{
    /// <summary>
    /// Class that loads the WordNet dataset from file
    /// </summary>
    public static class WordNetDataLoader
    {
        /// <summary>
        /// Loads the WordNet dataset from local file
        /// </summary>
        public static void LoadWordNetData()
        {
            // Loading the JSON file containing the WordNet dataset (originally from https://wordnet.princeton.edu/)
            //using var stream = new FileStream(@"C:\users\ssann\source\repos\WordsOpenLinkedData\WordNet\wordnet.json", FileMode.Open);
            using var stream = new FileStream(@"WordNet\wordnet.json", FileMode.Open);
            using var reader = new StreamReader(stream);
            var json = new StringBuilder();
            reader.ReadLine(); // Skip initial "{"
            reader.ReadLine(); // Skip "synset : {"
            int wordCounter = 0;
            do
            {
                // Read records as individual JSON snippets
                if (wordCounter == 117791)
                {
                    wordCounter = 0;
                }
                var s = reader.ReadLine();
                if (s.StartsWith("  }", System.StringComparison.OrdinalIgnoreCase)) // There are multiple JSON sets in the file, we only want the first (synset) one
                {
                    break;
                }
                var id = s.Replace(": {", "", System.StringComparison.OrdinalIgnoreCase).Trim();
                json.AppendLine($"{{\"id\":{id},");
                do
                {
                    s = reader.ReadLine();
                    if (s == "    },") // Snip off the separating comma, it is syntactically illegal
                    {
                        s = "    }";
                    }
                    json.AppendLine(s);
                }
                while (s != "    }");
                var wordJson = json.ToString();
                json.Clear();
                Synset wordObject = (Synset)JsonSerializer.Deserialize(wordJson, typeof(Synset));
                WordNetData.SynsetList.Add(wordObject);
                WordNetData.SynsetDictionary.Add(wordObject.Id, wordObject);
                wordCounter++;
            }
            while (1 == 1);
        }
    }
}
