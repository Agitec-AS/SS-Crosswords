using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Crosswords
{
    /// <summary>
    /// The basic data types for synset handling
    /// A synset is a set of synonymous words. The dataset handles synsets, not individual words. Some synsets contain only one word, others several.
    /// Distinct synsets may contain the same word.
    /// </summary>
    public class Synset
    {
        /// <summary>
        /// Id of synset entity
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Not used
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        /// <summary>
        /// The word type (verb, noun etc). See the WordNetData.WordTypes list for a description of this value.
        /// </summary>
        [JsonPropertyName("pos")]
        public string Pos { get; set; }

        /// <summary>
        /// A list of synonymous words that make up this synset
        /// </summary>
        [JsonPropertyName("word")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Content set by Json decoder>")]
        public List<string> Words { get; set; }

        /// <summary>
        /// Relationship pointers to other synsets
        /// </summary>
        [JsonPropertyName("pointer")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Content set by Json decoder>")]
        public List<SynsetPointer> SynsetPointers { get; set; }

        /// <summary>
        /// Not used
        /// </summary>
        [JsonPropertyName("frame")]
        public List<Frame> Frame { get; }

        /// <summary>
        /// A short description of the meaning of this synset
        /// </summary>
        [JsonPropertyName("gloss")]
        public string Gloss { get; set; }

        /// <summary>
        /// Not used
        /// </summary>
        [JsonPropertyName("example")]
        public List<Example> Example { get; }
    }

    /// <summary>
    /// A pointer from one synset to another
    /// </summary>
    public class SynsetPointer
    {
        /// <summary>
        /// Defines the relationship between the synset pointing from and the synset pointed to. See WordNetData.Relationships for a description of this value.
        /// </summary>
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// The synset being pointed to
        /// </summary>
        [JsonPropertyName("synset")]
        public string Synset { get; set; }

        /// <summary>
        /// Not used
        /// </summary>
        [JsonPropertyName("source")]
        public int Source { get; set; }

        /// <summary>
        /// Not used
        /// </summary>
        [JsonPropertyName("target")]
        public int Target { get; set; }
    }

    /// <summary>
    /// Not used
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// Not used
        /// </summary>
        [JsonPropertyName("wordNumber")]
        public int WordNumber { get; set; }

        /// <summary>
        /// Not used
        /// </summary>
        [JsonPropertyName("frameNumber")]
        public int FrameNumber { get; set; }
    }

    /// <summary>
    /// Not used
    /// </summary>
    public class Example
    {
        /// <summary>
        /// Not used
        /// </summary>
        [JsonPropertyName("wordNumber")]
        public int WordNumber { get; set; }

        /// <summary>
        /// Not used
        /// </summary>
        [JsonPropertyName("templateNumber")]
        public int TemplateNumber { get; set; }
    }
}
