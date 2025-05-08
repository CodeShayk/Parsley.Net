using System.ComponentModel;
using System.Globalization;
using parsley;

namespace Parsley.Tests
{
    public class FileLine : IFileLine
    {
        public int Index { get; set; }
        public IList<string> Errors { get; set; }

        public FileLine()
        {
            Errors = new List<string>();
        }

        [Column(0)]
        public int Code { get; set; }

        [Column(1)]
        public NameType Name { get; set; }

        [Column(2)]
        public bool IsActive { get; set; }

        [Column(3)]
        public Subcription Subcription { get; set; }
    }

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

    internal class NameConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string stringValue;
            object result;

            result = null;
            stringValue = value as string;

            if (!string.IsNullOrEmpty(stringValue))
            {
                result = NameType.Parse(stringValue);
            }

            return result ?? base.ConvertFrom(context, culture, value);
        }
    }

    public enum Subcription
    {
        None,
        Paid,
        Free
    }
}