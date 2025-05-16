using System;
using System.ComponentModel;
using System.Globalization;

namespace parsley
{
    public class CustomConverter<T> : TypeConverter where T : ICustomType, new()
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
                result = new T().Parse(stringValue);
            }

            return result ?? base.ConvertFrom(context, culture, value);
        }
    }
}