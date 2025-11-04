using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace parsley
{
    public class Parser : IParser
    {
        protected ParseOptions Options { get; set; }

        public Parser() : this(new ParseOptions())
        {
        }

        public Parser(char delimiter) : this(new ParseOptions(delimiter))
        {
        }

        public Parser(ParseOptions options)
        {
            Options = options ?? new ParseOptions();
        }

        public T[] Parse<T>(string filepath) where T : IFileLine, new()
        {
            return Parse<T>(filepath, Options);
        }

        public T[] Parse<T>(string filepath, ParseOptions options) where T : IFileLine, new()
        {
            if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
                return Array.Empty<T>();

            var lines = ReadToLines(filepath);

            return Parse<T>(lines, options);
        }

        public T[] Parse<T>(string[] lines) where T : IFileLine, new()
        {
            return Parse<T>(lines, Options);
        }

        public T[] Parse<T>(string[] lines, ParseOptions options) where T : IFileLine, new()
        {
            var linesToProcess = ProcessLinesArray(lines, options);
            if (linesToProcess.Length == 0)
                return Array.Empty<T>();

            return ProcessLines<T>(linesToProcess, options);
        }

        private string[] ReadToLines(string path)
        {
            return File.ReadAllLines(path);
        }

        private T ParseLine<T>(string line, int lineIndex = -1) where T : IFileLine, new()
        {
            var obj = new T();

            var values = GetDelimiterSeparatedValues(line);

            if (values.Length == 0 || values.Length == 1)
            {
                if (lineIndex >= 0)
                {
                    // Enhanced error message with line number
                    obj.SetError($"Line {lineIndex + 1}: Invalid line format - is not delimeter separated");
                }
                else
                {
                    // Use original error message format
                    obj.SetError(Resources.InvalidLineFormat);
                }
                return obj;
            }

            var propInfos = GetLineClassPropertyInfos<T>();

            if (propInfos.Length == 0)
            {
                if (lineIndex >= 0)
                {
                    obj.SetError($"Line {lineIndex + 1}: No column attributes found on Line type - {typeof(T).Name}");
                }
                else
                {
                    obj.SetError(string.Format(Resources.NoColumnAttributesFoundFormat, typeof(T).Name));
                }
                return obj;
            }

            if (propInfos.Length != values.Length)
            {
                if (lineIndex >= 0)
                {
                    obj.SetError($"Line {lineIndex + 1}: Invalid line format - number of column values do not match");
                }
                else
                {
                    obj.SetError(Resources.InvalidLengthErrorFormat);
                }
                return obj;
            }

            foreach (var propInfo in propInfos)
            {
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
                            if (lineIndex >= 0)
                            {
                                obj.SetError($"Line {lineIndex + 1}: Property '{propInfo.Name}' failed to parse - Invalid enum value");
                            }
                            else
                            {
                                obj.SetError(string.Format(Resources.InvalidEnumValueErrorFormat, propInfo.Name));
                            }
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
                    if (lineIndex >= 0)
                    {
                        obj.SetError($"Line {lineIndex + 1}: Property '{propInfo.Name}' failed to parse with error - {e.Message}");
                    }
                    else
                    {
                        obj.SetError(string.Format(Resources.LineExceptionFormat, propInfo.Name, e.Message));
                    }
                }
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
            var values = line.Split(Options.Delimiter);

            if (Options.TrimFieldValues)
            {
                values = values.Select(x => !string.IsNullOrWhiteSpace(x) ? x.Trim() : x).ToArray();
            }

            return values;
        }

        private static Result<T[]> TryOperation<T>(Func<T[]> operation) where T : IFileLine, new()
        {
            try
            {
                return Result<T[]>.Success(operation());
            }
            catch (Exception ex)
            {
                return Result<T[]>.Failure(ex.Message);
            }
        }

        private static async Task<Result<T[]>> TryOperationAsync<T>(Func<Task<T[]>> operation) where T : IFileLine, new()
        {
            try
            {
                var result = await operation();
                return Result<T[]>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<T[]>.Failure(ex.Message);
            }
        }

        private T[] ProcessLines<T>(string[] linesToProcess, ParseOptions options) where T : IFileLine, new()
        {
            if (linesToProcess.Length == 0)
                return Array.Empty<T>();

            var list = new T[linesToProcess.Length];

            // Sequential processing to avoid potential parallel processing issues
            for (int i = 0; i < linesToProcess.Length; i++)
            {
                var parsed = ParseLine<T>(linesToProcess[i], i);
                parsed.Index = i;
                list[i] = parsed;
            }

            return list;
        }

        private async Task<List<string>> ReadLinesFromStreamAsync(Stream stream, ParseOptions options, Encoding encoding, Func<StreamReader, Task<string>> readLineFunc)
        {
            var lines = new List<string>();
            using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
            {
                string line;
                int lineNumber = 0;

                do
                {
                    line = await readLineFunc(reader);
                    if (line == null)
                        break;

                    // If it's the first line and we should skip header, skip it
                    if (options.SkipHeaderLine && lineNumber == 0)
                    {
                        lineNumber++;
                        continue;
                    }

                    var processedLine = options.TrimFieldValues ? line.Trim() : line;
                    if (!options.IncludeEmptyLines && string.IsNullOrWhiteSpace(processedLine))
                    {
                        // Skip empty lines if not including them
                    }
                    else
                    {
                        lines.Add(processedLine);
                    }
                    lineNumber++;
                } while (line != null);
            }
            return lines;
        }

        private string[] ProcessLinesArray(string[] lines, ParseOptions options)
        {
            if (options == null)
                options = new ParseOptions();

            if (lines == null || lines.Length == 0)
                return Array.Empty<string>();

            // Store original lines to work with
            var originalLines = lines;

            // Handle SkipHeaderLine option
            if (options.SkipHeaderLine && originalLines.Length > 0)
            {
                originalLines = originalLines.Skip(1).ToArray();
            }

            // Determine which lines to process
            var linesToProcess = options.IncludeEmptyLines
                ? originalLines
                : originalLines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

            return linesToProcess;
        }

        private List<string> ReadLinesFromStream(Stream stream, ParseOptions options, Encoding encoding)
        {
            var lines = new List<string>();
            using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
            {
                var allLines = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    allLines.Add(line);
                }

                for (int i = 0; i < allLines.Count; i++)
                {
                    // If it's the first line and we should skip header, skip it
                    if (options.SkipHeaderLine && i == 0)
                    {
                        continue;
                    }

                    var processedLine = options.TrimFieldValues ? allLines[i].Trim() : allLines[i];
                    if (!options.IncludeEmptyLines && string.IsNullOrWhiteSpace(processedLine))
                    {
                        // Skip empty lines if not including them
                    }
                    else
                    {
                        lines.Add(processedLine);
                    }
                }
            }
            return lines;
        }

        public T[] Parse<T>(Stream stream, Encoding encoding = null) where T : IFileLine, new()
        {
            return Parse<T>(stream, encoding, Options);
        }

        public T[] Parse<T>(Stream stream, Encoding encoding, ParseOptions options) where T : IFileLine, new()
        {
            if (options == null)
                options = new ParseOptions();

            if (stream == null || stream.Length == 0)
                return Array.Empty<T>();

            var lines = ReadLinesFromStream(stream, options, encoding);

            return lines.Any() ? Parse<T>(lines.ToArray(), options) : Array.Empty<T>();
        }

        public T[] Parse<T>(byte[] bytes, Encoding encoding = null) where T : IFileLine, new()
        {
            return Parse<T>(bytes, encoding, Options);
        }

        public T[] Parse<T>(byte[] bytes, Encoding encoding, ParseOptions options) where T : IFileLine, new()
        {
            if (bytes == null || bytes.Length == 0)
                return Array.Empty<T>();

            return Parse<T>(new MemoryStream(bytes), encoding, options);
        }

        public async Task<T[]> ParseAsync<T>(string filepath) where T : IFileLine, new()
        {
            return await ParseAsync<T>(filepath, Options);
        }

        public async Task<T[]> ParseAsync<T>(string filepath, ParseOptions options) where T : IFileLine, new()
        {
            if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
                return Array.Empty<T>();

            var lines = await Task.FromResult(ReadToLines(filepath));

            return await ParseAsync<T>(lines, options);
        }

        public async Task<T[]> ParseAsync<T>(byte[] bytes, Encoding encoding = null) where T : IFileLine, new()
        {
            return await ParseAsync<T>(bytes, encoding, Options);
        }

        public async Task<T[]> ParseAsync<T>(byte[] bytes, Encoding encoding, ParseOptions options) where T : IFileLine, new()
        {
            if (bytes == null || bytes.Length == 0)
                return Array.Empty<T>();

            return await ParseAsync<T>(new MemoryStream(bytes), encoding, options);
        }

        public async Task<T[]> ParseAsync<T>(Stream stream, Encoding encoding = null) where T : IFileLine, new()
        {
            return await ParseAsync<T>(stream, encoding, Options);
        }

        public async Task<T[]> ParseAsync<T>(Stream stream, Encoding encoding, ParseOptions options) where T : IFileLine, new()
        {
            if (options == null)
                options = new ParseOptions();

            if (stream == null)
                return Array.Empty<T>();

            // Only check Length if stream is not null
            if (stream.Length == 0)
                return Array.Empty<T>();

            var lines = await ReadLinesFromStreamAsync(stream, options, encoding, (reader) => reader.ReadLineAsync());

            return lines.Any() ? await ParseAsync<T>(lines.ToArray(), options) : Array.Empty<T>();
        }

        public async Task<T[]> ParseAsync<T>(string[] lines) where T : IFileLine, new()
        {
            return await ParseAsync<T>(lines, Options);
        }

        public async Task<T[]> ParseAsync<T>(string[] lines, ParseOptions options) where T : IFileLine, new()
        {
            string[] linesToProcess;
            try
            {
                linesToProcess = ProcessLinesArray(lines, options);
            }
            catch
            {
                return Array.Empty<T>();
            }

            if (linesToProcess.Length == 0)
                return Array.Empty<T>();

            return await Task.Run(() => ProcessLines<T>(linesToProcess, options));
        }

        public Result<T[]> TryParse<T>(string filepath) where T : IFileLine, new()
        {
            return TryOperation(() => Parse<T>(filepath, Options));
        }

        public Result<T[]> TryParse<T>(string filepath, ParseOptions options) where T : IFileLine, new()
        {
            return TryOperation(() => Parse<T>(filepath, options));
        }

        public Result<T[]> TryParse<T>(string[] lines) where T : IFileLine, new()
        {
            return TryOperation(() => Parse<T>(lines, Options));
        }

        public Result<T[]> TryParse<T>(string[] lines, ParseOptions options) where T : IFileLine, new()
        {
            return TryOperation(() => Parse<T>(lines, options));
        }

        public Result<T[]> TryParse<T>(byte[] bytes, Encoding encoding = null) where T : IFileLine, new()
        {
            return TryOperation(() => Parse<T>(bytes, encoding, Options));
        }

        public Result<T[]> TryParse<T>(byte[] bytes, Encoding encoding, ParseOptions options) where T : IFileLine, new()
        {
            return TryOperation(() => Parse<T>(bytes, encoding, options));
        }

        public Result<T[]> TryParse<T>(Stream stream, Encoding encoding = null) where T : IFileLine, new()
        {
            return TryOperation(() => Parse<T>(stream, encoding, Options));
        }

        public Result<T[]> TryParse<T>(Stream stream, Encoding encoding, ParseOptions options) where T : IFileLine, new()
        {
            return TryOperation(() => Parse<T>(stream, encoding, options));
        }

        public async Task<Result<T[]>> TryParseAsync<T>(string filepath) where T : IFileLine, new()
        {
            return await TryOperationAsync(() => ParseAsync<T>(filepath, Options));
        }

        public async Task<Result<T[]>> TryParseAsync<T>(string filepath, ParseOptions options) where T : IFileLine, new()
        {
            return await TryOperationAsync(() => ParseAsync<T>(filepath, options));
        }

        public async Task<Result<T[]>> TryParseAsync<T>(string[] lines) where T : IFileLine, new()
        {
            return await TryOperationAsync(() => ParseAsync<T>(lines, Options));
        }

        public async Task<Result<T[]>> TryParseAsync<T>(string[] lines, ParseOptions options) where T : IFileLine, new()
        {
            return await TryOperationAsync(() => ParseAsync<T>(lines, options));
        }

        public async Task<Result<T[]>> TryParseAsync<T>(byte[] bytes, Encoding encoding = null) where T : IFileLine, new()
        {
            return await TryOperationAsync(() => ParseAsync<T>(bytes, encoding, Options));
        }

        public async Task<Result<T[]>> TryParseAsync<T>(byte[] bytes, Encoding encoding, ParseOptions options) where T : IFileLine, new()
        {
            return await TryOperationAsync(() => ParseAsync<T>(bytes, encoding, options));
        }

        public async Task<Result<T[]>> TryParseAsync<T>(Stream stream, Encoding encoding = null) where T : IFileLine, new()
        {
            return await TryOperationAsync(() => ParseAsync<T>(stream, encoding, Options));
        }

        public async Task<Result<T[]>> TryParseAsync<T>(Stream stream, Encoding encoding, ParseOptions options) where T : IFileLine, new()
        {
            return await TryOperationAsync(() => ParseAsync<T>(stream, encoding, options));
        }
    }
}