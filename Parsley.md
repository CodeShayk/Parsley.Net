# Developer Guide
## Installation: Nuget
Install the latest version of nuget package using the command below.
```
NuGet\Install-Package Parsley.Net
```

## Implementation: Using Parsley.Net
### Step 1. Initialise and use Parser class.
`Parser` is an implementation of `IParser` interface that provides methods for 
- parsing content of a file by specifying the file path
- parsing an array of delimiter separated strings

Please see below.
```
  public interface IParser
  {
     T[] Parse<T>(string filepath) where T : IFileLine, new();
     T[] Parse<T>(string[] lines) where T : IFileLine, new();
     T[] Parse<T>(Stream stream) where T : IFileLine, new();
     T[] Parse<T>(byte[] bytes) where T : IFileLine, new();

     T[] ParseAsync<T>(string filepath) where T : IFileLine, new();
     T[] ParseAsync<T>(string[] lines) where T : IFileLine, new();
     T[] ParseAsync<T>(Stream stream) where T : IFileLine, new();
     T[] ParseAsync<T>(byte[] bytes) where T : IFileLine, new();
  }
```
To initialise `Parser` class you could do it manually or use dependency injection as shown below. The parser class has parameterised constructor that takes the delimiter character to initialise the instance. Default character is ',' (comma) to initialise the parser for a CSV file parsing.

Example of Manual Instantiation is
```
 var parser = new Parser('|');
```
Example of IoC is
```
var services = new ServiceCollection();

services.AddTransient(typeof(IParser), c => new Parser(','));
// or use extension method
services.UseParsley('|');

serviceProvider = services.BuildServiceProvider();
var parser = serviceProvider.GetService<IParser>();
```
### Step 2. Define the `IFileLine` implementation to parse a file record into a strongly typed line class.
Consider the file below. To parse a row into a C# class, you need to implement `IFileLine` interface. By doing this you create a strongly typed line representation for each row in the file.
```
|Mr|Jack Marias|Male|London|
|Dr|Bony Stringer|Male|New Jersey|
|Mrs|Mary Ward|Female||
|Mr|Robert Webb|||
```
Let us create an employee class which will hold data for each row shown in the file above. The properties in the line class should match to the column index and data type of the fields of the row.

We use the column attribute to specify the column index and can optionally specify a default value for the associated column should it be be empty. As a rule of thumb, the number of properties with column attributes should match the number of columns in the row else the engine will throw an exception.

IFileLine interface provides 
- `Index` property that holds the index of the parsed line relative to the whole file,
- `Errors` property which is an array representing any column parsing failures.

Please see below.
```
public interface IFileLine
{
    public int Index { get; set; }
    public IList<string> Errors { get; set; }
}
```

Example.
```
public class Employee : IFileLine
{
    // Custom column properties

    [Column(0)]
    public string Title { get; set; }
    [Column(1)]
    public string Name { get; set; }
    [Column(2)]
    public EnumGender Sex { get; set; }
    [Column(3, "London")]
    public string Location { get; set; }

    // IFileLine Members
    public int Index { get; set; }
    public IList<string> Errors { get; set; }
} 
```
Once you have created the line class it is as simple as calling one of the parser.Parse() methods below

i. By providing the path of the file to parse method.
```
var records = new Parser('|').Parse<Employee>("c://employees.txt");
```
ii. By providing the list of delimiter separated string values to parse method.
```
 var lines = new[]
 {
      "|Mr|Jack Marias|Male|London|",
      "|Dr|Bony Stringer|Male|New Jersey|",
 };

var records = new Parser('|').Parse<Employee>(lines);
```
### Step 3. Advanced Parsing of data using nested types in the FileLine class.
You could implement advance parsing of data by implementing `TypeConverter` class. Suppose we have to change the Name string property in Employee class above to a `NameType` property shown below. 
```
public class Employee : IFileLine
{
    [Column(0)]
    public string Title { get; set; }
    [Column(1)]
    public NameType Name { get; set; }
    [Column(2)]
    public EnumGender Sex { get; set; }
    [Column(3, "London")]
    public string Location { get; set; }

    // IFileLine Members
    public int Index { get; set; }
    public IList<string> Errors { get; set; }
}

public class NameType
{
    public string FirstName { get; set; }
    public string Surname { get; set; }
}
```

In order to parse the string name value from delimiter separated record in the file correctly to NameType instance, you need to implement custom `TypeConverter` converter.

Example - `NameConverter`
```
public class NameConverter : TypeConverter
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
```
After implementing the custom TypeConverter, you need to decorate the NameType class with ` [TypeConverter(typeof(NameConverter))]` attribute.

```
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
```
Now parsing the file should hydrate data correctly to the Employee FileLine class and its nested name type.

## Parsley.Net v2.0.0 - Major Release Features

Parsley.Net v2.0.0 represents a comprehensive evolution of the library with enhanced functionality while maintaining complete backward compatibility. This major release introduces new configuration options, improved error handling, and better performance.

### 1. Enhanced Error Reporting with Line Numbers

The v2.0.0 release significantly improves error reporting by providing line numbers and field names in error messages:

```
public class EnhancedErrorExample : IFileLine
{
    [Column(0)]
    public string Code { get; set; }
    
    [Column(1)] 
    public NameType Name { get; set; }
    
    public int Index { get; set; }
    public IList<string> Errors { get; set; }
}

// Usage - error messages now include line numbers and field details
var parser = new Parser('|');
var lines = new[] { "GB-01|Invalid Name Format", "XX-99|Another Invalid Entry" };

var result = parser.Parse<EnhancedErrorExample>(lines);

foreach (var item in result)
{
    if (item.Errors?.Any() == true)
    {
        Console.WriteLine($"Line {item.Index}: {string.Join(", ", item.Errors)}");
        // Example: "Line 1: Name failed to parse - Invalid name format in value 'Invalid Name Format'"
    }
}
```

### 2. ParseOptions Configuration Class

Introducing `ParseOptions` to configure parsing behavior with various options:

```
public class ParseOptions
{
    public char Delimiter { get; set; } = ',';        // Default delimiter
    public bool SkipHeaderLine { get; set; } = false; // Skip first line as header
    public bool TrimFieldValues { get; set; } = true; // Trim whitespace from values
    public bool IncludeEmptyLines { get; set; } = true; // Include empty lines in output
    public int MaxErrors { get; set; } = -1;         // Max errors to collect (-1 for unlimited)
    public int BufferSize { get; set; } = 1024;      // Buffer size for streaming operations
}

// Usage examples
var customParser = new Parser();

// Parse with custom options
var options = new ParseOptions 
{ 
    Delimiter = '|',
    SkipHeaderLine = true, 
    TrimFieldValues = true,
    IncludeEmptyLines = false,
    MaxErrors = 100 
};

var result = await parser.ParseAsync<Employee>("data.csv", options);
```

### 3. TryParse and TryParseAsync Methods

New methods added for more explicit error handling using the result pattern:

```
// Synchronous TryParse methods
public Result<T[]> TryParse<T>(string filepath) where T : IFileLine, new();
public Result<T[]> TryParse<T>(string filepath, ParseOptions options) where T : IFileLine, new();
public Result<T[]> TryParse<T>(string[] lines) where T : IFileLine, new();
public Result<T[]> TryParse<T>(string[] lines, ParseOptions options) where T : IFileLine, new();
public Result<T[]> TryParse<T>(byte[] bytes, Encoding encoding = null) where T : IFileLine, new();
public Result<T[]> TryParse<T>(byte[] bytes, Encoding encoding, ParseOptions options) where T : IFileLine, new();
public Result<T[]> TryParse<T>(Stream stream, Encoding encoding = null) where T : IFileLine, new();
public Result<T[]> TryParse<T>(Stream stream, Encoding encoding, ParseOptions options) where T : IFileLine, new();

// Asynchronous TryParse methods  
public async Task<Result<T[]>> TryParseAsync<T>(string filepath) where T : IFileLine, new();
public async Task<Result<T[]>> TryParseAsync<T>(string filepath, ParseOptions options) where T : IFileLine, new();
public async Task<Result<T[]>> TryParseAsync<T>(string[] lines) where T : IFileLine, new();
public async Task<Result<T[]>> TryParseAsync<T>(string[] lines, ParseOptions options) where T : IFileLine, new();
public async Task<Result<T[]>> TryParseAsync<T>(byte[] bytes, Encoding encoding = null) where T : IFileLine, new();
public async Task<Result<T[]>> TryParseAsync<T>(byte[] bytes, Encoding encoding, ParseOptions options) where T : IFileLine, new();
public async Task<Result<T[]>> TryParseAsync<T>(Stream stream, Encoding encoding = null) where T : IFileLine, new();
public async Task<Result<T[]>> TryParseAsync<T>(Stream stream, Encoding encoding, ParseOptions options) where T : IFileLine, new();

// Usage example with TryParse
var parser = new Parser('|');

// Safe parsing without throwing exceptions
var result = parser.TryParse<Employee>("employees.csv");

if (result.IsSuccess)
{
    var employees = result.Value;
    Console.WriteLine($"Successfully parsed {employees.Length} employees");
    
    // Check for individual record errors
    var validRecords = employees.Where(e => e.Errors?.Any() != true).ToArray();
    var errorRecords = employees.Where(e => e.Errors?.Any() == true).ToArray();
    
    Console.WriteLine($"Valid records: {validRecords.Length}, Error records: {errorRecords.Length}");
}
else
{
    // Global parsing errors occurred
    Console.WriteLine($"Parsing failed: {result.Error}");
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  - {error}");
    }
}
```

### 4. Result Pattern Implementation

The `Result<T>` class provides explicit success/failure semantics:

```
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public IList<string> Errors { get; }
    
    public static Result<T> Success(T value);
    public static Result<T> Failure(string error);
    public static Result<T> Failure(IList<string> errors);
    public static Result<T> Failure(string error, IList<string> errors);
}

// Examples:
var successResult = Result<string>.Success("Operation completed successfully");
var errorResult = Result<Employee[]>.Failure("File not found");
```

### 5. Configuration Options Usage

The new ParseOptions class allows for flexible parsing configurations:

```
// Example 1: CSV with headers, skipping first line
var csvOptions = new ParseOptions 
{ 
    Delimiter = ',', 
    SkipHeaderLine = true 
};
var csvResult = parser.Parse<Employee>("employees.csv", csvOptions);

// Example 2: TSV with tab delimiter, no trimming
var tsvOptions = new ParseOptions 
{ 
    Delimiter = '\t', 
    TrimFieldValues = false 
};
var tsvResult = parser.Parse<DataRecord>("data.tsv", tsvOptions);

// Example 3: PSV with pipe delimiter, limiting errors
var psvOptions = new ParseOptions 
{ 
    Delimiter = '|', 
    MaxErrors = 50,
    IncludeEmptyLines = false
};
var psvResult = await parser.TryParseAsync<Employee>("data.psv", psvOptions);
```

### 6. Backward Compatibility

All v2.0.0 changes maintain complete backward compatibility:

```
// All existing code continues to work unchanged
var parser = new Parser('|');
var employees = parser.Parse<Employee>("employees.csv"); // Still works
var employeesAsync = await parser.ParseAsync<Employee>("employees.csv"); // Still works

// New functionality builds upon existing methods
var tryResult = parser.TryParse<Employee>("employees.csv"); // New in v2.0.0
```

### Migration Guide

Upgrading from v1.x to v2.0.0 is seamless for existing code:

1. **Existing code**: No changes required - all previous APIs remain the same
2. **New features**: Gradually adopt TryParse methods and ParseOptions for enhanced functionality
3. **Better error handling**: Switch to TryParse methods where more explicit error handling is needed


