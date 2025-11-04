using parsley;
using Parsley.Tests.FileLines;

namespace Parsley.Tests
{
    [TestFixture]
    public class ParseResultFixture
    {
        [Test]
        public void TestParseResultConstructorShouldInitializeWithProvidedValues()
        {
            // Test with global errors present
            var noErrorLine1 = new FileLine { Index = 0 };
            noErrorLine1.Errors.Clear(); // Ensure no errors

            var noErrorLine2 = new FileLine { Index = 1 };
            noErrorLine2.Errors.Clear(); // Ensure no errors

            var values = new[] { noErrorLine1, noErrorLine2 };
            var globalErrors = new List<string> { "Global error 1", "Global error 2" }; // With global errors

            var result = new ParseResult<FileLine>(values, globalErrors);

            Assert.That(result.ParsedValues, Is.EqualTo(values));
            Assert.That(result.GlobalErrors, Is.EqualTo(globalErrors));
            Assert.That(result.TotalRecords, Is.EqualTo(2));
            Assert.That(result.ErrorCount, Is.EqualTo(0));
            Assert.That(result.SuccessCount, Is.EqualTo(2));
            Assert.That(result.HasErrors, Is.True); // Because of global errors
        }

        [Test]
        public void TestParseResultConstructorWithNoErrorsShouldHaveNoErrors()
        {
            // Test with no errors at all
            var noErrorLine1 = new FileLine { Index = 0 };
            noErrorLine1.Errors.Clear(); // Ensure no errors

            var noErrorLine2 = new FileLine { Index = 1 };
            noErrorLine2.Errors.Clear(); // Ensure no errors

            var values = new[] { noErrorLine1, noErrorLine2 };
            var globalErrors = new List<string>(); // No global errors

            var result = new ParseResult<FileLine>(values, globalErrors);

            Assert.That(result.ParsedValues, Is.EqualTo(values));
            Assert.That(result.TotalRecords, Is.EqualTo(2));
            Assert.That(result.ErrorCount, Is.EqualTo(0));
            Assert.That(result.SuccessCount, Is.EqualTo(2));
            Assert.That(result.HasErrors, Is.False); // No errors at all
        }

        [Test]
        public void TestParseResultConstructorWithNullValuesShouldInitializeWithEmptyArray()
        {
            var result = new ParseResult<FileLine>(null);

            Assert.That(result.ParsedValues, Is.Empty);
            Assert.That(result.GlobalErrors, Is.Empty);
            Assert.That(result.TotalRecords, Is.EqualTo(0));
            Assert.That(result.ErrorCount, Is.EqualTo(0));
            Assert.That(result.SuccessCount, Is.EqualTo(0));
            Assert.That(result.HasErrors, Is.False);
        }

        [Test]
        public void TestParseResultConstructorWithNullGlobalErrorsShouldInitializeWithEmptyList()
        {
            var values = new[] { new FileLine { Index = 0 } };

            var result = new ParseResult<FileLine>(values, null);

            Assert.That(result.ParsedValues, Is.EqualTo(values));
            Assert.That(result.GlobalErrors, Is.Empty);
            Assert.That(result.TotalRecords, Is.EqualTo(1));
            Assert.That(result.ErrorCount, Is.EqualTo(0));
            Assert.That(result.SuccessCount, Is.EqualTo(1));
            Assert.That(result.HasErrors, Is.False);
        }

        [Test]
        public void TestParseResultHasErrorsShouldReturnFalseWhenNoErrorsExist()
        {
            var values = new[]
            {
                new FileLine { Index = 0 },
                new FileLine { Index = 1 }
            };

            var result = new ParseResult<FileLine>(values);

            Assert.That(result.HasErrors, Is.False);
        }

        [Test]
        public void TestParseResultHasErrorsShouldReturnTrueWhenGlobalErrorsExist()
        {
            var values = new[] { new FileLine { Index = 0 } };
            var globalErrors = new List<string> { "Global error" };

            var result = new ParseResult<FileLine>(values, globalErrors);

            Assert.That(result.HasErrors, Is.True);
        }

        [Test]
        public void TestParseResultHasErrorsShouldReturnTrueWhenRecordErrorsExist()
        {
            var errorLine = new FileLine { Index = 0 };
            errorLine.Errors.Add("Record error");

            var values = new[] { errorLine };

            var result = new ParseResult<FileLine>(values);

            Assert.That(result.HasErrors, Is.True);
        }

        [Test]
        public void TestParseResultGetSuccessfulRecordsShouldReturnRecordsWithoutErrors()
        {
            var successfulLine = new FileLine { Index = 0 };
            var errorLine = new FileLine { Index = 1 };
            errorLine.Errors.Add("Record error");

            var values = new[] { successfulLine, errorLine };

            var result = new ParseResult<FileLine>(values);
            var successfulRecords = result.GetSuccessfulRecords().ToArray();

            Assert.That(successfulRecords.Length, Is.EqualTo(1));
            Assert.That(successfulRecords[0], Is.EqualTo(successfulLine));
        }

        [Test]
        public void TestParseResultGetFailedRecordsShouldReturnRecordsWithErrors()
        {
            var successfulLine = new FileLine { Index = 0 };
            var errorLine = new FileLine { Index = 1 };
            errorLine.Errors.Add("Record error");

            var values = new[] { successfulLine, errorLine };

            var result = new ParseResult<FileLine>(values);
            var failedRecords = result.GetFailedRecords().ToArray();

            Assert.That(failedRecords.Length, Is.EqualTo(1));
            Assert.That(failedRecords[0], Is.EqualTo(errorLine));
        }

        [Test]
        public void TestParseResultGetAllErrorsShouldReturnCombinedErrors()
        {
            var errorLine = new FileLine { Index = 0 };
            errorLine.Errors.Add("Record error 1");
            errorLine.Errors.Add("Record error 2");

            var values = new[] { errorLine };
            var globalErrors = new List<string> { "Global error 1", "Global error 2" };

            var result = new ParseResult<FileLine>(values, globalErrors);
            var allErrors = result.GetAllErrors().ToArray();

            Assert.That(allErrors.Length, Is.EqualTo(4));
            Assert.That(allErrors, Contains.Item("Global error 1"));
            Assert.That(allErrors, Contains.Item("Global error 2"));
            Assert.That(allErrors, Contains.Item("Record error 1"));
            Assert.That(allErrors, Contains.Item("Record error 2"));
        }

        [Test]
        public void TestParseResultGetAllErrorsShouldReturnOnlyGlobalErrorsWhenNoRecordErrors()
        {
            var values = new[] { new FileLine { Index = 0 } };
            var globalErrors = new List<string> { "Global error" };

            var result = new ParseResult<FileLine>(values, globalErrors);
            var allErrors = result.GetAllErrors().ToArray();

            Assert.That(allErrors.Length, Is.EqualTo(1));
            Assert.That(allErrors[0], Is.EqualTo("Global error"));
        }

        [Test]
        public void TestParseResultGetAllErrorsShouldReturnOnlyRecordErrorsWhenNoGlobalErrors()
        {
            var errorLine = new FileLine { Index = 0 };
            errorLine.Errors.Add("Record error");

            var values = new[] { errorLine };

            var result = new ParseResult<FileLine>(values);
            var allErrors = result.GetAllErrors().ToArray();

            Assert.That(allErrors.Length, Is.EqualTo(1));
            Assert.That(allErrors[0], Is.EqualTo("Record error"));
        }

        [Test]
        public void TestParseResultErrorCountShouldReflectActualRecordErrors()
        {
            var noErrorLine = new FileLine { Index = 0 };
            var errorLine1 = new FileLine { Index = 1 };
            errorLine1.Errors.Add("Error 1");
            var errorLine2 = new FileLine { Index = 2 };
            errorLine2.Errors.Add("Error 2");
            errorLine2.Errors.Add("Error 3");
            var noErrorLine2 = new FileLine { Index = 3 };

            var values = new[] { noErrorLine, errorLine1, errorLine2, noErrorLine2 };

            var result = new ParseResult<FileLine>(values);

            Assert.That(result.ErrorCount, Is.EqualTo(2)); // Two records have errors, regardless of how many errors per record
            Assert.That(result.SuccessCount, Is.EqualTo(2)); // Two records have no errors
            Assert.That(result.TotalRecords, Is.EqualTo(4));
        }
    }
}