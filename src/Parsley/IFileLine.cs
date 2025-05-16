using System.Collections.Generic;

namespace parsley
{
    public interface IFileLine
    {
        int Index { get; set; }
        IList<string> Errors { get; set; }
    }
}