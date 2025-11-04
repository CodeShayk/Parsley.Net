namespace parsley
{
    /// <summary>
    /// Configuration options for parsing delimiter-separated files
    /// </summary>
    public class ParseOptions
    {
        /// <summary>
        /// Delimiter character to use for parsing. Default is comma (',')
        /// </summary>
        public char Delimiter { get; set; } = ',';

        /// <summary>
        /// Whether to skip the first line of the file, assuming it's a header
        /// </summary>
        public bool SkipHeaderLine { get; set; } = false;

        /// <summary>
        /// Whether to trim whitespace from field values
        /// Default is true for backward compatibility
        /// </summary>
        public bool TrimFieldValues { get; set; } = true;

        /// <summary>
        /// Whether to include empty lines in the result (as objects with errors)
        /// Default is true to maintain original behavior
        /// </summary>
        public bool IncludeEmptyLines { get; set; } = true;

        /// <summary>
        /// Maximum number of errors to collect before stopping (use -1 for unlimited)
        /// Default is -1 (unlimited) for backward compatibility
        /// </summary>
        public int MaxErrors { get; set; } = -1;

        /// <summary>
        /// Buffer size for streaming operations (number of lines to process at once)
        /// </summary>
        public int BufferSize { get; set; } = 1024;

        /// <summary>
        /// Creates a new ParseOptions instance with default values
        /// </summary>
        public ParseOptions()
        {
        }

        /// <summary>
        /// Creates a new ParseOptions instance with specified delimiter
        /// </summary>
        public ParseOptions(char delimiter)
        {
            Delimiter = delimiter;
        }
    }
}