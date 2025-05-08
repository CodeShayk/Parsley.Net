using parsley;

namespace Parsley.Tests.FileLines
{
    public class FileLine : IFileLine
    {
        public int Index { get; set; }
        public IList<string> Errors { get; set; }

        public FileLine()
        {
            Errors = [];
        }

        [Column(0)]
        public CodeType Code { get; set; }

        [Column(1)]
        public NameType Name { get; set; }

        [Column(2)]
        public bool IsActive { get; set; }

        [Column(3)]
        public Subcription Subcription { get; set; }
    }

    public enum Subcription
    {
        None,
        Paid,
        Free
    }
}