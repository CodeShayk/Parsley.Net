using System.ComponentModel;
using parsley;

namespace Parsley.Tests.FileLines
{
    [TypeConverter(typeof(CustomConverter<CodeType>))]
    public class CodeType : ICustomType
    {
        public string Batch { get; set; }
        public int SerialNo { get; set; }

        public ICustomType Parse(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            var codes = input.Split('-');

            if (codes.Length != 2 || !int.TryParse(codes[1], out int serialNo))
                throw new FormatException($"Invalid code format: {input}");

            return new CodeType { Batch = codes[0], SerialNo = serialNo };
        }
    }
}