using System.Text.Json.Serialization;

namespace Crosswords
{
    /// <summary>
    /// The data type used to return data from the web service. Each unique word in the set is returned as a single entry, ie synsets are split up into words.
    /// </summary>
    public class SearchResultEntry
    {
        /// <summary>
        /// The word returned
        /// </summary>
        [JsonPropertyName("word")]
        public string Word { get; set; }

        /// <summary>
        /// The type of word (noun, verb, etc.)
        /// </summary>
        [JsonPropertyName("wordType")]
        public string WordType { get; set; }

        /// <summary>
        /// The relationship between this word and the synset linking to it
        /// </summary>
        [JsonPropertyName("relation")]
        public string Relation { get; set; }

        /// <summary>
        /// A brief description of the meaning of the word
        /// </summary>
        [JsonPropertyName("gloss")]
        public string Gloss { get; set; }

        /// <summary>
        /// Creates a brief single-line description of the entry - only the word itself, with _ replaced by space. 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Word.Replace("_", " ", System.StringComparison.OrdinalIgnoreCase)}";
        }

        /// <summary>
        /// Creates a verbose single-line description of the entry - the word (_ replaced by space) plus extra information about it and its relationship with the synset linking to it.
        /// </summary>
        /// <returns></returns>
        public string ToStringVerbose()
        {
            return $"{Word.Replace("_", " ", System.StringComparison.OrdinalIgnoreCase)} - ({WordType}) - {(string.IsNullOrEmpty(Relation) ? "" : $"[{Relation}]")} {Gloss}";
        }
    }
}
