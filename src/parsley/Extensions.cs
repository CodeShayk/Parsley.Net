namespace parsley
{
    internal static class Extensions
    {
        internal static void SetError(this IFileLine obj, string error)
        {
            if (obj.Errors == null)
                obj.Errors = new List<string>();

            obj.Errors.Add(error);
        }
    }
}