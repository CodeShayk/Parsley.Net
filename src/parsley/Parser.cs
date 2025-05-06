using System.ComponentModel;
using System.Reflection;

namespace parsley
{
    public class Parser : IParser
    {
        protected char Delimiter { get; set; }

        public Parser() : this(',')
        {
        }

        public Parser(char delimiter)
        {
            Delimiter = delimiter;
        }

        public T[] Parse<T>(string filepath) where T : IFileLine, new()
        {
            if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
                return [];

            var lines = ReadToLines(filepath);

            return Parse<T>(lines);
        }

        public T[] Parse<T>(string[] lines) where T : IFileLine, new()
        {
            if (lines == null || lines.Length == 0)
                return [];

            var list = new T[lines.Length];

            var objLock = new object();

            var index = 0;
            var inputs = lines.Select(line => new { Line = line, Index = index++ });

            Parallel.ForEach(inputs, () => new List<T>(),
                (obj, loopstate, localStorage) =>
                {
                    var parsed = ParseLine<T>(obj.Line);

                    parsed.Index = obj.Index;

                    localStorage.Add(parsed);
                    return localStorage;
                },
                finalStorage =>
                {
                    if (finalStorage == null)
                        return;

                    lock (objLock)
                        finalStorage.ForEach(f => list[f.Index] = f);
                });

            return list;
        }

        private string[] ReadToLines(string path)
        {
            var lines = new List<string>();

            foreach (var line in File.ReadLines(path))
            {
                if (line != null)
                    lines.Add(line);
            }

            return lines.ToArray();
        }

        private T ParseLine<T>(string line) where T : IFileLine, new()
        {
            var obj = new T();

            var values = GetDelimiterSeparatedValues(line);

            if (values.Length == 0 || values.Length == 1)
            {
                obj.SetError(Resources.InvalidLineFormat);
                return obj;
            }

            var propInfos = GetLineClassPropertyInfos<T>();

            if (propInfos.Length == 0)
            {
                obj.SetError(string.Format(Resources.NoColumnAttributesFoundFormat, typeof(T).Name));
                return obj;
            }

            if (propInfos.Length != values.Length)
            {
                obj.SetError(Resources.InvalidLengthErrorFormat);
                return obj;
            }

            foreach (var propInfo in propInfos)
                try
                {
                    var attribute = (ColumnAttribute)propInfo.GetCustomAttributes(typeof(ColumnAttribute), true).First();

                    var pvalue = values[attribute.Index];

                    if (string.IsNullOrWhiteSpace(pvalue) && attribute.DefaultValue != null)
                        pvalue = attribute.DefaultValue.ToString();

                    if (propInfo.PropertyType.IsEnum)
                    {
                        if (string.IsNullOrWhiteSpace(pvalue))
                        {
                            obj.SetError(string.Format(Resources.InvalidEnumValueErrorFormat, propInfo.Name));
                            continue;
                        }

                        if (long.TryParse(pvalue, out var enumLong))
                        {
                            var numeric = Enum.ToObject(propInfo.PropertyType, enumLong);
                            propInfo.SetValue(obj, numeric, null);
                            continue;
                        }

                        var val = Enum.Parse(propInfo.PropertyType, pvalue, true);
                        propInfo.SetValue(obj, val, null);
                        continue;
                    }

                    var converter = TypeDescriptor.GetConverter(propInfo.PropertyType);
                    var value = converter.ConvertFrom(pvalue);

                    propInfo.SetValue(obj, value, null);
                }
                catch (Exception e)
                {
                    obj.SetError(string.Format(Resources.LineExceptionFormat, propInfo.Name, e.Message));
                }

            return obj;
        }

        private static PropertyInfo[] GetLineClassPropertyInfos<T>() where T : IFileLine, new()
        {
            var propInfos = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(ColumnAttribute), true).Any() && p.CanWrite)
                .ToArray();
            return propInfos;
        }

        private string[] GetDelimiterSeparatedValues(string line)
        {
            var values = line.Split(Delimiter)
                .Select(x => !string.IsNullOrWhiteSpace(x) ? x.Trim() : x)
                .ToArray();
            return values;
        }
    }
}