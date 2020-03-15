using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Crosswords.Controllers
{
    /// <summary>
    /// API controller for the WordsOpenLinkedData web service.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class WordsController : ControllerBase
    {
        private readonly ILogger<WordsController> _logger;
        private const int MaxRecurseDepth = 5;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        public WordsController(ILogger<WordsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Search for words related to the input word. Returns a simple list of distinct matching words. 
        /// Depth of recursion is critical. With the default value of 2, returns the word you searched for and words directly related to it.
        /// Note that synonyms are counted as one and the same word for our purposes here.
        /// </summary>
        /// <param name="searchWord">The word to search for, e.g. 'shark'</param>
        /// <param name="length">If specified, only return related words this many characters long.</param>
        /// <param name="recurse">Depth of recursion when searching. Minimum is 1, maximum is 5, default is 2. Bigger numbers give a larger number of hits but also less relevant ones.</param>
        /// <param name="pattern">Character pattern to match. Optional. Underscore matches everything. E.g. _a_c - only return words with 'a' as the second character and 'c' as the fourth. Can be combined with the length parameter or used alone. If used alone, only words at least as long as the pattern will be returned.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{searchWord}")]
        public ActionResult<List<string>> Get(string searchWord, int length = 0, int recurse = 2, string pattern = null)
        {
            if (recurse < 1 || recurse > MaxRecurseDepth)
            {
                return Content($"The depth of recursion must be between 1 and {MaxRecurseDepth}.");
            }
            if (!string.IsNullOrEmpty(pattern) && length > 0 && pattern.Length > length)
            {
                return Content("Pattern length must be less than length parameter.");
            }
            var result = new WordsWorker().GetWords(searchWord?.ToUpperInvariant(), length, recurse, pattern, false);
            if (result != null && result.Any())
            {
                return result.Select(row=>row.Word).Distinct().ToList();
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Search for words related to the input word. Returns a list of records with detailed info for each hit, and includes various meanings of the search word as distinct hits. 
        /// Depth of recursion is critical. With the default value of 2, returns the word you searched for and words directly related to it. 
        /// Note that synonyms are counted as one and the same word for our purposes here.
        /// </summary>
        /// <param name="searchWord">The word to search for, e.g. 'shark'</param>
        /// <param name="length">If specified, only return related words this many characters long.</param>
        /// <param name="recurse">Depth of recursion when searching. Minimum is 1, maximum is 5, default is 2. Bigger numbers give a larger number of hits but also less relevant ones.</param>
        /// <param name="pattern">Character pattern to match. Optional. Underscore matches everything. E.g. _a_c - only return words with 'a' as the second character and 'c' as the fourth. Can be combined with the length parameter or used alone. If used alone, only words at least as long as the pattern will be returned.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Verbose/{searchWord}")]
        // public ActionResult<IEnumerable<string>> Get(string word)
        public ActionResult<List<SearchResultEntry>> GetVerbose(string searchWord, int length = 0, int recurse = 2, string pattern = null)
        {
            if (recurse < 1 || recurse > MaxRecurseDepth)
            {
                return Content($"The depth of recursion must be between 1 and {MaxRecurseDepth}.");
            }
            if (!string.IsNullOrEmpty(pattern) && length > 0 && pattern.Length > length)
            {
                return Content("Pattern length must be less than length parameter.");
            }
            var result = new WordsWorker().GetWords(searchWord?.ToUpperInvariant(), length, recurse, pattern, true);
            if (result != null && result.Any())
            {
                return result;
            }
            return new NotFoundResult();
        }
    }
}
