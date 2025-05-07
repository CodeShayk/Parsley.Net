# <img src="https://github.com/CodeShayk/parsley.net/blob/master/Images/ninja-icon-16.png" alt="ninja" style="width:30px;"/> Parsley.Net v1.0.0
[![NuGet version](https://badge.fury.io/nu/Parsley.Net.svg)](https://badge.fury.io/nu/Parsley.Net) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/CodeShayk/Parsley.Net/blob/master/LICENSE.md) 
[![master-build](https://github.com/CodeShayk/parsley.net/actions/workflows/Master-Build.yml/badge.svg)](https://github.com/CodeShayk/parsley.net/actions/workflows/Master-Build.yml)
[![GitHub Release](https://img.shields.io/github/v/release/CodeShayk/Parsley.Net?logo=github&sort=semver)](https://github.com/CodeShayk/Parsley.Net/releases/latest)
[![Master-CodeQL](https://github.com/CodeShayk/Parsley.Net/actions/workflows/Master-CodeQL.yml/badge.svg)](https://github.com/CodeShayk/Parsley.Net/actions/workflows/Master-CodeQL.yml) 
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

### ii. Developer Guide

Please read [Developer Guide](https://github.com/CodeShayk/Parsley.Net/blob/master/Parsley.md) for details on how to implement Parsley.Net in your project.

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
