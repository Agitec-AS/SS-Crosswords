using System.Collections.Generic;

namespace Crosswords
{
    /// <summary>
    /// The variables used in synset handling
    /// </summary>
    public static class WordNetData
    {
        /// <summary>
        /// A list of all synsets in the dataset.
        /// </summary>
        public static List<Synset> SynsetList { get; } = new List<Synset>();
        
        /// <summary>
        /// A dictionary of synsets ( id + synset ) for faster access
        /// </summary>
        public static Dictionary<string, Synset> SynsetDictionary {get;} = new Dictionary<string, Synset>();

        /// <summary>
        /// A list of wordtypes, used to look up the meaning of Synset.Pos
        /// </summary>
        public static List<KeyValuePair<string, string>> WordTypes { get; } = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("a", "Adjective"),
            new KeyValuePair<string, string>("n", "Noun"),
            new KeyValuePair<string, string>("r", "Adverb"),
            new KeyValuePair<string, string>("s", "Satellite"),
            new KeyValuePair<string, string>("v", "Verb"),
        };

        /// <summary>
        /// A list of relationships between synsets, used to look up the meaning of SynsetPointer.Symbol.
        /// </summary>
        public static List<KeyValuePair<string, string>> Relationships { get; } = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("!", "Antonym"),
            new KeyValuePair<string, string>("@", "Hypernym"),
            new KeyValuePair<string, string>("@i", "InstanceHypernym"),
            new KeyValuePair<string, string>("~", "Hyponym"),
            new KeyValuePair<string, string>("~i", "InstanceHyponym"),
            new KeyValuePair<string, string>("#m", "MemberHolonym"),
            new KeyValuePair<string, string>("#s", "SubstanceHolonym"),
            new KeyValuePair<string, string>("#p", "PartHolonym"),
            new KeyValuePair<string, string>("%m", "MemberMeronym"),
            new KeyValuePair<string, string>("%s", "SubstanceMeronym"),
            new KeyValuePair<string, string>("%p", "PartMeronym"),
            new KeyValuePair<string, string>("=", "Attribute"),
            new KeyValuePair<string, string>("+", "DerivationallyRelatedForm"),
            new KeyValuePair<string, string>(";c", "DomainOfSynsetTopic"),
            new KeyValuePair<string, string>("-c", "MemberOfThisDomainTopic"),
            new KeyValuePair<string, string>(";r", "DomainOfSynsetRegion"),
            new KeyValuePair<string, string>("-r", "MemberOfThisDomainRegion"),
            new KeyValuePair<string, string>(";u", "DomainOfSynsetUsage"),
            new KeyValuePair<string, string>("-u", "MemberOfThisDomainUsage"),
            new KeyValuePair<string, string>("*", "Entailment"),
            new KeyValuePair<string, string>(">", "Cause"),
            new KeyValuePair<string, string>("^", "AlsoSee"),
            new KeyValuePair<string, string>("$", "VerbGroup"),
            new KeyValuePair<string, string>("&", "SimilarTo"),
            new KeyValuePair<string, string>("<", "ParticipleOfVerb"),
            new KeyValuePair<string, string>("\\", "Pertainym"),
            new KeyValuePair<string, string>("\\\\", "DerivedFromAdjective") // Duplicate of Pertainym in original source. It may be the wrong way around!
        };
    }
}
