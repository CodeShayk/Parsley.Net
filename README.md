# <img src="https://github.com/CodeShayk/parsley.net/blob/master/Images/ninja-icon-16.png" alt="ninja" style="width:30px;"/> Parsley.Net v1.0.0
[![NuGet version](https://badge.fury.io/nu/Parsley.Net.svg)](https://badge.fury.io/nu/Parsley.Net) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/CodeShayk/Parsley.Net/blob/master/LICENSE.md) 
[![GitHub Release](https://img.shields.io/github/v/release/CodeShayk/Parsley.Net?logo=github&sort=semver)](https://github.com/CodeShayk/Parsley.Net/releases/latest)
[![master-build](https://github.com/CodeShayk/parsley.net/actions/workflows/Master-Build.yml/badge.svg)](https://github.com/CodeShayk/parsley.net/actions/workflows/Master-Build.yml)
[![master-codeql](https://github.com/CodeShayk/parsley.net/actions/workflows/Master-CodeQL.yml/badge.svg)](https://github.com/CodeShayk/parsley.net/actions/workflows/Master-CodeQL.yml)
[![.Net 9.0](https://img.shields.io/badge/.Net-9.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

## Introduction
### What is Parsley.Net?
`Parsley.Net` is a .net utility to help parse `Fixed Width` or `Delimiter Separated` file using strongly typed objects.
> It is a simple and easy to use library that can be used to parse delimeter separated files in .net applications. It is designed to be lightweight and fast, making it ideal for use in high-performance applications.

### What is Fixed width or Delimiter separated text file?
`Fixed width` or `Delimiter separated` text file is a file that has a specific format which allows for the manipulation of textual information in an organized fashion.
- Each row contains one record of information; each record can contain multiple pieces of data fields or columns.
- The data columns are separated by any character you specify called the delimiter. 
- All rows in the file follow a consistent format and should be with the same number of data columns. 
- Data columns could be empty with no value.

Example: Simple pipe '|' separated Delimeter File is shown below (this could even be comma ',' separated CSV)
```
|Mr|Jack Marias|Male|London|Active|||
|Dr|Bony Stringer|Male|New Jersey|Active||Paid|
|Mrs|Mary Ward|Female||Active|||
|Mr|Robert Webb|||Active|||
```

## Getting Started?
### i. Installation
Install the latest version of Parsley.Net nuget package with command below. 

```
NuGet\Install-Package Parsley.Net 
```
### ii. Implementation: Using Parsley.Net
#### <ins>Step 1<ins>. Initialise and use Parser class.
`Parser` is an implementation of `IParser` interface that provides methods for 
- parsing content of a file by specifying the file path
- parsing an array of delimiter separated strings

Please see below.
```
  public interface IParser
  {
     public T[] Parse<T>(string filepath) where T : IFileLine, new();
     public T[] Parse<T>(string[] lines) where T : IFileLine, new();
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
#### <ins>Step 2<ins>. Define the `IFileLine` implementation to parse a file record into a strongly typed line class.
Consider the file below. To parse a row into a C# class, you need to implement `IFileLine` interface. By doing this you create a strongly typed line representation for each row in the file.
```
|Mr|Jack Marias|Male|London|
|Dr|Bony Stringer|Male|New Jersey|
|Mrs|Mary Ward|Female||
|Mr|Robert Webb|||
```
Let us create an employee class which will hold data for each row shown in the file above. The properties in the line class should match to the column index and data type of the fields of the row.

We use the column attribute to specify the column index and can optionally specify a default value for the associated column should it be be empty. As a rule of thumb, the number of properties with column attributes should match the number of columns in the row else the parser will throw an exception.

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

Example. `Employee` class
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
#### <ins>Step 3<ins>. Advanced Parsing of data using nested types in the FileLine class.
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

In order to parse the string value (name) from delimiter separated record in the file correctly to custom type (NameType), you need to implement custom converter deriving from `TypeConverter` class.

Example - `NameConverter` class - converts name string value to NameType instance
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

## Support

If you are having problems, please let me know by [raising a new issue](https://github.com/CodeShayk/Parsley.Net/issues/new/choose).

## License

This project is licensed with the [MIT license](LICENSE).

## Version History
The main branch is now on .NET 9.0. 
| Version  | Release Notes |
| -------- | --------|
| [`v1.0.0`](https://github.com/CodeShayk/Parsley.Net/tree/v1.0.0) |  [Notes](https://github.com/CodeShayk/Parsley.Net/releases/tag/v1.0.0) |

## Credits
Thank you for reading. Please fork, explore, contribute and report. Happy Coding !! :)
