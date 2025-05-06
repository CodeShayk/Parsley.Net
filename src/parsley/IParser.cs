namespace parsley
{
    public interface IParser
    {
        public T[] Parse<T>(string filepath) where T : IFileLine, new();

        public T[] Parse<T>(string[] lines) where T : IFileLine, new();
    }
}