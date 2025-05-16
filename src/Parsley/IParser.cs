namespace parsley
{
    public interface IParser
    {
        T[] Parse<T>(string filepath) where T : IFileLine, new();

        T[] Parse<T>(string[] lines) where T : IFileLine, new();
    }
}