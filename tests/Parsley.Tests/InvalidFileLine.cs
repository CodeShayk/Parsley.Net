using parsley;

namespace Parsley.Tests
{
    public class InvalidFileLine : IFileLine
    {
        public int Index { get; set; }
        public IList<string> Errors { get; set; }
    }
}