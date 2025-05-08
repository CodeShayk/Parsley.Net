namespace parsley
{
    public interface IFileLine
    {
        public int Index { get; set; }
        public IList<string> Errors { get; set; }
    }
}