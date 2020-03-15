using System;
using System.Collections.Generic;
using System.Linq;

namespace Crosswords
{
    /// <summary>
    /// Worker class for working with synsets
    /// </summary>
    public class WordsWorker
    {
        private string _pattern;
        private int? _desiredWordLength;
        private int? _patternLength;
        private int _maxRecurseDepth;
        private bool _verbose;

        /// <summary>
        /// Get a list of words that are related to the search word.
        /// </summary>
        /// <param name="searchWord">The word to base the search on</param>
        /// <param name="length">If set, only words this long will be returned</param>
        /// <param name="recurse">The depth of recursion - 1 will return only the start word, 2 that and words related to it, 3 those and words that are related to those... and so on.</param>
        /// <param name="pattern">If set, return only words that match the pattern. _ is used as joker. _a__ returns only words that are at least 4 characters long with 'a' as second character.</param>
        /// <param name="verbose">If true, will return extended information about each result entry.</param>
        /// <returns></returns>
        public List<SearchResultEntry> GetWords(string searchWord, int length = 0, int recurse = 2, string pattern = null, bool verbose = false)
        {
            _verbose = verbose;
            _maxRecurseDepth = recurse;
            _pattern = string.IsNullOrEmpty(pattern) || pattern.All(c => c == '_')
                ? null
                : pattern.ToUpperInvariant();
            _desiredWordLength = length == 0 
                ? (int?)null 
                : length;
            _patternLength = length == 0 && !string.IsNullOrEmpty(_pattern)
                ? pattern.Length
                : (int?)null;
            var matches = WordNetData.SynsetList
                .Where(row => row.Words
                .Any(entry => entry.Equals(searchWord, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            // May find more than one synset containing the search word, in cases where a word has multiple meanings. Also note each synset may contain several words (synonyms)
            return matches.Any()
                ? FindSynsetsRelatedToInitialHits(matches) 
                : null;
        }

        private List<SearchResultEntry> FindSynsetsRelatedToInitialHits(List<Synset> matches)
        {
            var response = new List<SearchResultEntry>();
            foreach (var synset in matches)
            {
                var wordsInSynset = string.Join(", ", synset.Words.Select(row => row).ToList());
                foreach (var entry in synset.Words)
                {
                    AddWordToResultSet(entry, "", synset.Pos, synset.Gloss, response);
                }
                foreach (var pointer in synset.SynsetPointers)
                {
                    // Follow the links from each initial synset hit to to other synset entries, adding them to the result. 
                    // Recurses to the desired recursion depth.
                    GetLinkedSynsets(response, pointer.Synset, wordsInSynset, 1);
                }
            }
            // Orders the result set a bit differently depending on whether we want a simple or verbose result set.
            List<SearchResultEntry> result = _desiredWordLength == 0
                ? response.OrderBy(row => row.Relation.Length != 0)
                    .ThenBy(row => row.Word.Length)
                    .ThenBy(row => row.Word)
                    .ToList()
                : response.OrderBy(row => row.Relation.Length != 0)
                    .ThenBy(row => row.Word)
                    .ToList();
            if (!_verbose)
            {
                // Duplicate words are removed from the simple result set. They are kept in the verbose one because
                // they are different meanings of the same word.
                result = result.Distinct().ToList(); 
            }
            return result;
        }


        private void AddWordToResultSet(string word, string relationship, string pos, string gloss, List<SearchResultEntry> synonymList)
        {
            if (!synonymList.Any(row => row.Word == word && row.Gloss == gloss))
            {
                if (word.Length == _desiredWordLength
                    || (_desiredWordLength == null
                        && (string.IsNullOrEmpty(_pattern) || !string.IsNullOrEmpty(_pattern) && word.Length >= _patternLength)))
                {
                    if (EntryMatchesPattern(word))
                    {
                        synonymList.Add(new SearchResultEntry
                        {
                            Word = word,
                            WordType = WordNetData.WordTypes.First(row => row.Key == pos).Value,
                            Relation = relationship,
                            Gloss = gloss
                        });
                    }
                }
            }
        }

        private bool EntryMatchesPattern(string word)
        {
            word = word.ToUpperInvariant();
            if (!string.IsNullOrEmpty(_pattern))
            {
                int i = 0;
                foreach (var c in _pattern)
                {
                    if (c != '_' && word.Length > i && word[i] != c)
                    {
                        return false;
                    }
                    i++;
                }
            }
            return true;
        }

        private void GetLinkedSynsets(List<SearchResultEntry> synonymList, string synsetId, string wordsInSourceSynset, int recurseCount)
        {
            recurseCount++;
            if (recurseCount > _maxRecurseDepth)
            {
                return;
            }
            var synset = WordNetData.SynsetDictionary[synsetId];
            var wordsInSynset = string.Join(", ", synset.Words.Select(row => row).ToList());
            var relationships = new List<string>();
            foreach (var pointer in synset.SynsetPointers)
            {
                string s = $"{WordNetData.Relationships.First(row => row.Key == pointer.Symbol).Value} of {wordsInSourceSynset}";
                // Examples:
                // ("fish" is a) "Hypernym of shark" 
                // ("shark" is a) "DerivationallyRelatedForm of fish" 
                // ("approval" is a) "Hypernym of praise, congratulations, kudos, extolment" - praise/congratulations/kudos/extolment are four synonyms in a single synset
                if (!relationships.Contains(s))
                {
                    relationships.Add(s);
                }
            }
            foreach (var word in synset.Words)
            {
                AddWordToResultSet(word, string.Join(", ", relationships), synset.Pos, synset.Gloss, synonymList);
            }
            foreach (var pointer in synset.SynsetPointers)
            {
                GetLinkedSynsets(synonymList, pointer.Synset, wordsInSynset, recurseCount);
            }
        }
    }
}
