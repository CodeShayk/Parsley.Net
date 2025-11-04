using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Parsley.Tests.FileLines;
using parsley;

namespace Parsley.Tests
{
    [TestFixture]
    public class DebugParseResultFixture
    {
        [Test]
        public void TestDebugFileLineErrors()
        {
            var fileLine = new FileLine { Index = 0 };
            System.Console.WriteLine($"FileLine Errors Count: {fileLine.Errors.Count}");
            System.Console.WriteLine($"FileLine Errors Is Null: {fileLine.Errors == null}");
            
            // Test ParseResult with one FileLine that has no errors
            var result = new ParseResult<FileLine>(new[] { fileLine });
            
            System.Console.WriteLine($"Result Total Records: {result.TotalRecords}");
            System.Console.WriteLine($"Result Error Count: {result.ErrorCount}");
            System.Console.WriteLine($"Result Success Count: {result.SuccessCount}");
            System.Console.WriteLine($"Result Has Errors: {result.HasErrors}");
            
            Assert.That(result.TotalRecords, Is.EqualTo(1));
            Assert.That(result.ErrorCount, Is.EqualTo(0)); // Expect no errors
            Assert.That(result.SuccessCount, Is.EqualTo(1)); // Expect 1 success
            Assert.That(result.HasErrors, Is.False); // Expect no errors
        }
    }
}