using System;
using System.Collections.Generic;
using System.Linq;

namespace parsley
{
    /// <summary>
    /// Provides detailed information about a parsing operation result
    /// </summary>
    public class ParseResult<T> where T : IFileLine
    {
        public T[] ParsedValues { get; }
        public bool HasErrors => GlobalErrors?.Count > 0 || ParsedValues?.Any(v => v.Errors?.Count > 0) == true;
        public IList<string> GlobalErrors { get; }
        public int TotalRecords { get; }
        public int ErrorCount { get; }
        public int SuccessCount => TotalRecords - ErrorCount;

        public ParseResult(T[] parsedValues, IList<string> globalErrors = null)
        {
            ParsedValues = parsedValues ?? Array.Empty<T>();
            GlobalErrors = globalErrors ?? new List<string>();
            TotalRecords = ParsedValues.Length;
            ErrorCount = ParsedValues.Count(v => v.Errors != null && v.Errors.Count > 0);
        }

        public IEnumerable<T> GetSuccessfulRecords() =>
            ParsedValues.Where(v => v.Errors == null || v.Errors.Count == 0);

        public IEnumerable<T> GetFailedRecords() =>
            ParsedValues.Where(v => v.Errors != null && v.Errors.Count > 0);

        public IEnumerable<string> GetAllErrors()
        {
            var errors = new List<string>(GlobalErrors);
            foreach (var record in ParsedValues.Where(v => v.Errors != null))
            {
                errors.AddRange(record.Errors);
            }
            return errors;
        }
    }
}