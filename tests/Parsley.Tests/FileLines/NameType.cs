using System.ComponentModel;

namespace Parsley.Tests.FileLines
{
    [TypeConverter(typeof(NameConverter))]
    public class NameType
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }

        public static NameType Parse(string input)
        {
            var values = input.Split(' ');

            if (values.Length == 1)
                return new NameType { FirstName = values[0] };

            if (values.Length == 2)
                return new NameType { FirstName = values[0], Surname = values[1] };

            if (values.Length > 2)
            {
                var forenames = string.Empty;
                for (var i = 0; i < values.Length - 1; i++)
                    forenames += string.Concat(values[i]) + " ";

                return new NameType { FirstName = forenames.Trim(), Surname = values[values.Length - 1] };
            }

            return new NameType { FirstName = input };
        }
    }
}