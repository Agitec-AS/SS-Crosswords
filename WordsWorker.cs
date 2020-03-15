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
            _desiredWordLength = length == 0 ? (int?)null : length;
            _patternLength = length == 0 && !string.IsNullOrEmpty(_pattern)
                ? pattern.Length
                : (int?)null;
            var matches = WordNetData.SynsetList
                .Where(row => row.Words
                .Any(entry => entry.Equals(searchWord, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            return matches.Any()
                ? FindConnectedSynsets(matches) 
                : null;
        }

        private List<SearchResultEntry> FindConnectedSynsets(List<Synset> matches)
        {
            var response = new List<SearchResultEntry>();
            foreach (var match in matches)
            {
                var aggregateWord = string.Join(", ", match.Words.Select(row => row).ToList());
                foreach (var entry in match.Words)
                {
                    AddWord(entry, "", match.Pos, match.Gloss, response);
                }
                foreach (var pointer in match.SynsetPointers)
                {
                    GetSynonyms(response, pointer.Synset, aggregateWord, 1);
                }
            }
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
                result = result.Distinct().ToList();
            }
            return result;
        }


        private void AddWord(string word, string relationship, string pos, string gloss, List<SearchResultEntry> synonymList)
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

        private void GetSynonyms(List<SearchResultEntry> synonymList, string synset, string sourceWord, int recurseCount)
        {
            recurseCount++;
            if (recurseCount > _maxRecurseDepth)
            {
                return;
            }
            var hit = WordNetData.SynsetDictionary[synset];
            var aggregateWord = string.Join(", ", hit.Words.Select(row => row).ToList());
            var relationships = new List<string>();
            foreach (var hitPointer in hit.SynsetPointers)
            {
                string s = $"{WordNetData.Relationships.First(row => row.Key == hitPointer.Symbol).Value} of {sourceWord}";
                if (!relationships.Contains(s))
                {
                    relationships.Add(s);
                }
            }
            foreach (var word in hit.Words)
            {
                AddWord(word, string.Join(", ", relationships), hit.Pos, hit.Gloss, synonymList);
            }
            foreach (var hitPointer in hit.SynsetPointers)
            {
                GetSynonyms(synonymList, hitPointer.Synset, aggregateWord, recurseCount);
            }
        }
    }
}
