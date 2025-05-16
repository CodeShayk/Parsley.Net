namespace parsley
{
    public interface ICustomType
    {
        ICustomType Parse(string column);
    }
}