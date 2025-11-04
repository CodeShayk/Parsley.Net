using parsley;
using Microsoft.Extensions.DependencyInjection;
using Parsley.Tests.FileLines;
using System.Text;

namespace Parsley.Tests
{
    [TestFixture]
    internal class ParserFixture
    {
        private Parser parser;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestParseForNullOrEmptyInputShouldReturnEmptyArray()
        {
            Assert.That(parser.Parse<FileLine>((string[])null), Is.Empty);
            Assert.That(parser.Parse<FileLine>(Array.Empty<string>()), Is.Empty);

            Assert.That(parser.Parse<FileLine>(string.Empty), Is.Empty);
            Assert.That(parser.Parse<FileLine>((string)null), Is.Empty);
        }

        [Test]
        public void TestParseForDependencyInjectionShouldReturnInitialisedInstance()
        {
            var services = new ServiceCollection();

            services.UseParsley();

            var serviceProvider = services.BuildServiceProvider();
            var parser = serviceProvider.GetService<IParser>();

            Assert.That(parser, Is.Not.Null);
        }

        [Test]
        public void TestParseWithFilePathShouldReturnCorrectlyParsedArray()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "TestFile.txt");

            parser = new Parser();

            var parsed = parser.Parse<FileLine>(filePath);

            Assert.That(parsed.Length, Is.EqualTo(2));

            Assert.That(parsed[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(parsed[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(parsed[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(parsed[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(parsed[0].IsActive, Is.EqualTo(true));
            Assert.That(parsed[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(parsed[0].Errors, Is.Empty);

            Assert.That(parsed[1].Code.Batch, Is.EqualTo("UG"));
            Assert.That(parsed[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(parsed[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(parsed[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(parsed[1].IsActive, Is.EqualTo(false));
            Assert.That(parsed[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(parsed[1].Errors, Is.Empty);
        }

        [Test]
        public void TestParseWithStringArrayInputShouldReturnCorrectlyParsedArray()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');

            var parsed = parser.Parse<FileLine>(lines);

            Assert.That(parsed.Length, Is.EqualTo(2));

            Assert.That(parsed[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(parsed[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(parsed[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(parsed[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(parsed[0].IsActive, Is.EqualTo(true));
            Assert.That(parsed[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(parsed[0].Errors, Is.Empty);

            Assert.That(parsed[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(parsed[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(parsed[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(parsed[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(parsed[1].IsActive, Is.EqualTo(false));
            Assert.That(parsed[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(parsed[1].Errors, Is.Empty);
        }

        [TestCase("hbtrb")]
        [TestCase("hbtrb|ej ef|fer|")]
        [TestCase("H|hbtrb")]
        [TestCase("H|hbtrb|ej ef|fer|rc |")]
        public void TestParseForInvalidInputShouldReturnError(string line)
        {
            var result = parser.Parse<FileLine>(new[] { line });

            Assert.That(result[0].Errors, Is.Not.Empty);
        }

        [Test]
        public void TestParseForInvalidFileLineWithNoColumnAttributesShouldReturnError()
        {
            parser = new Parser('|');

            var result = parser.Parse<InvalidFileLine>(new[] { "01|edndx|medmd" });

            Assert.That(result[0].Errors, Is.Not.Empty);

            result = parser.Parse<InvalidFileLine>(new[] { "edndx|true" });

            Assert.That(result[0].Errors, Is.Not.Empty);
        }

        [Test]
        public void TestParseWithStreamInputShouldReturnCorrectlyParsedArray()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');

            var parsed = parser.Parse<FileLine>(new MemoryStream(Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines))));

            Assert.That(parsed.Length, Is.EqualTo(2));

            Assert.That(parsed[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(parsed[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(parsed[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(parsed[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(parsed[0].IsActive, Is.EqualTo(true));
            Assert.That(parsed[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(parsed[0].Errors, Is.Empty);

            Assert.That(parsed[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(parsed[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(parsed[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(parsed[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(parsed[1].IsActive, Is.EqualTo(false));
            Assert.That(parsed[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(parsed[1].Errors, Is.Empty);
        }

        [Test]
        public void TestParseWithByteArrayInputShouldReturnCorrectlyParsedArray()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');

            var parsed = parser.Parse<FileLine>(Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines)));

            Assert.That(parsed.Length, Is.EqualTo(2));

            Assert.That(parsed[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(parsed[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(parsed[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(parsed[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(parsed[0].IsActive, Is.EqualTo(true));
            Assert.That(parsed[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(parsed[0].Errors, Is.Empty);

            Assert.That(parsed[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(parsed[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(parsed[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(parsed[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(parsed[1].IsActive, Is.EqualTo(false));
            Assert.That(parsed[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(parsed[1].Errors, Is.Empty);
        }

        [Test]
        public async Task TestParseAsyncWithFilePathShouldReturnCorrectlyParsedArray()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "TestFile.txt");

            parser = new Parser();

            var parsed = await parser.ParseAsync<FileLine>(filePath);

            Assert.That(parsed.Length, Is.EqualTo(2));

            Assert.That(parsed[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(parsed[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(parsed[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(parsed[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(parsed[0].IsActive, Is.EqualTo(true));
            Assert.That(parsed[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(parsed[0].Errors, Is.Empty);

            Assert.That(parsed[1].Code.Batch, Is.EqualTo("UG"));
            Assert.That(parsed[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(parsed[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(parsed[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(parsed[1].IsActive, Is.EqualTo(false));
            Assert.That(parsed[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(parsed[1].Errors, Is.Empty);
        }

        [Test]
        public async Task TestParseAsyncWithStringArrayInputShouldReturnCorrectlyParsedArray()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid",
                 "UH-03|Fred Wigg|False|Paid",
             };

            parser = new Parser('|');

            var parsed = await parser.ParseAsync<FileLine>(lines);

            Assert.That(parsed.Length, Is.EqualTo(3));

            Assert.That(parsed[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(parsed[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(parsed[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(parsed[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(parsed[0].IsActive, Is.EqualTo(true));
            Assert.That(parsed[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(parsed[0].Errors, Is.Empty);

            Assert.That(parsed[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(parsed[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(parsed[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(parsed[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(parsed[1].IsActive, Is.EqualTo(false));
            Assert.That(parsed[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(parsed[1].Errors, Is.Empty);
        }

        [Test]
        public async Task TestParseAsyncWithStreamInputShouldReturnCorrectlyParsedArray()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');

            var parsed = await parser.ParseAsync<FileLine>(new MemoryStream(Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines))));

            Assert.That(parsed.Length, Is.EqualTo(2));

            Assert.That(parsed[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(parsed[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(parsed[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(parsed[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(parsed[0].IsActive, Is.EqualTo(true));
            Assert.That(parsed[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(parsed[0].Errors, Is.Empty);

            Assert.That(parsed[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(parsed[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(parsed[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(parsed[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(parsed[1].IsActive, Is.EqualTo(false));
            Assert.That(parsed[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(parsed[1].Errors, Is.Empty);
        }

        [Test]
        public async Task TestParseAsyncWithByteArrayInputShouldReturnCorrectlyParsedArray()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');

            var parsed = await parser.ParseAsync<FileLine>(Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines)));

            Assert.That(parsed.Length, Is.EqualTo(2));

            Assert.That(parsed[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(parsed[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(parsed[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(parsed[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(parsed[0].IsActive, Is.EqualTo(true));
            Assert.That(parsed[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(parsed[0].Errors, Is.Empty);

            Assert.That(parsed[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(parsed[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(parsed[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(parsed[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(parsed[1].IsActive, Is.EqualTo(false));
            Assert.That(parsed[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(parsed[1].Errors, Is.Empty);
        }

        [Test]
        public void TestTryParseForNullFilepathShouldReturnSuccessResultWithEmptyArray()
        {
            parser = new Parser();

            var result = parser.TryParse<FileLine>((string)null);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Empty);
        }

        [Test]
        public void TestTryParseForNonExistentFilepathShouldReturnSuccessResultWithEmptyArray()
        {
            parser = new Parser();

            var result = parser.TryParse<FileLine>("nonexistent.txt");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Empty);
        }

        [Test]
        public void TestTryParseWithFilePathShouldReturnSuccessResult()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "TestFile.txt");

            parser = new Parser();

            var result = parser.TryParse<FileLine>(filePath);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Length, Is.EqualTo(2));

            Assert.That(result.Value[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(result.Value[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(result.Value[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(result.Value[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(result.Value[0].IsActive, Is.EqualTo(true));
            Assert.That(result.Value[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(result.Value[0].Errors, Is.Empty);

            Assert.That(result.Value[1].Code.Batch, Is.EqualTo("UG"));
            Assert.That(result.Value[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(result.Value[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(result.Value[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(result.Value[1].IsActive, Is.EqualTo(false));
            Assert.That(result.Value[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(result.Value[1].Errors, Is.Empty);
        }

        [Test]
        public void TestTryParseWithFilePathAndOptionsShouldReturnSuccessResult()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "TestFile.txt");

            parser = new Parser();
            var options = new ParseOptions();

            var result = parser.TryParse<FileLine>(filePath, options);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Length, Is.EqualTo(2));

            Assert.That(result.Value[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(result.Value[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(result.Value[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(result.Value[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(result.Value[0].IsActive, Is.EqualTo(true));
            Assert.That(result.Value[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(result.Value[0].Errors, Is.Empty);

            Assert.That(result.Value[1].Code.Batch, Is.EqualTo("UG"));
            Assert.That(result.Value[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(result.Value[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(result.Value[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(result.Value[1].IsActive, Is.EqualTo(false));
            Assert.That(result.Value[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(result.Value[1].Errors, Is.Empty);
        }

        [Test]
        public void TestTryParseForNullOrEmptyLinesShouldReturnSuccessResultWithEmptyArray()
        {
            parser = new Parser();

            var nullResult = parser.TryParse<FileLine>((string[])null);
            var emptyResult = parser.TryParse<FileLine>(Array.Empty<string>());

            Assert.That(nullResult.IsSuccess, Is.True);
            Assert.That(nullResult.Value, Is.Empty);

            Assert.That(emptyResult.IsSuccess, Is.True);
            Assert.That(emptyResult.Value, Is.Empty);
        }

        [Test]
        public void TestTryParseWithLinesShouldReturnSuccessResult()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');

            var result = parser.TryParse<FileLine>(lines);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Length, Is.EqualTo(2));

            Assert.That(result.Value[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(result.Value[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(result.Value[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(result.Value[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(result.Value[0].IsActive, Is.EqualTo(true));
            Assert.That(result.Value[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(result.Value[0].Errors, Is.Empty);

            Assert.That(result.Value[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(result.Value[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(result.Value[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(result.Value[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(result.Value[1].IsActive, Is.EqualTo(false));
            Assert.That(result.Value[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(result.Value[1].Errors, Is.Empty);
        }

        [Test]
        public void TestTryParseWithLinesAndOptionsShouldReturnSuccessResult()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');
            var options = new ParseOptions('|');

            var result = parser.TryParse<FileLine>(lines, options);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Length, Is.EqualTo(2));

            Assert.That(result.Value[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(result.Value[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(result.Value[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(result.Value[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(result.Value[0].IsActive, Is.EqualTo(true));
            Assert.That(result.Value[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(result.Value[0].Errors, Is.Empty);

            Assert.That(result.Value[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(result.Value[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(result.Value[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(result.Value[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(result.Value[1].IsActive, Is.EqualTo(false));
            Assert.That(result.Value[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(result.Value[1].Errors, Is.Empty);
        }

        [Test]
        public void TestTryParseForNullByteArrayShouldReturnSuccessResultWithEmptyArray()
        {
            parser = new Parser();

            var result = parser.TryParse<FileLine>((byte[])null);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Empty);
        }

        [Test]
        public void TestTryParseWithByteArrayShouldReturnSuccessResult()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');
            var bytes = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines));

            var result = parser.TryParse<FileLine>(bytes);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Length, Is.EqualTo(2));

            Assert.That(result.Value[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(result.Value[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(result.Value[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(result.Value[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(result.Value[0].IsActive, Is.EqualTo(true));
            Assert.That(result.Value[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(result.Value[0].Errors, Is.Empty);

            Assert.That(result.Value[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(result.Value[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(result.Value[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(result.Value[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(result.Value[1].IsActive, Is.EqualTo(false));
            Assert.That(result.Value[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(result.Value[1].Errors, Is.Empty);
        }

        [Test]
        public void TestTryParseWithByteArrayAndOptionsShouldReturnSuccessResult()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');
            var options = new ParseOptions('|');
            var bytes = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines));

            var result = parser.TryParse<FileLine>(bytes, Encoding.UTF8, options);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Length, Is.EqualTo(2));

            Assert.That(result.Value[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(result.Value[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(result.Value[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(result.Value[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(result.Value[0].IsActive, Is.EqualTo(true));
            Assert.That(result.Value[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(result.Value[0].Errors, Is.Empty);

            Assert.That(result.Value[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(result.Value[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(result.Value[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(result.Value[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(result.Value[1].IsActive, Is.EqualTo(false));
            Assert.That(result.Value[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(result.Value[1].Errors, Is.Empty);
        }

        [Test]
        public void TestTryParseForNullStreamShouldReturnSuccessResultWithEmptyArray()
        {
            parser = new Parser();

            var result = parser.TryParse<FileLine>((Stream)null);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Empty);
        }

        [Test]
        public void TestTryParseWithStreamShouldReturnSuccessResult()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines)));

            var result = parser.TryParse<FileLine>(stream);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Length, Is.EqualTo(2));

            Assert.That(result.Value[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(result.Value[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(result.Value[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(result.Value[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(result.Value[0].IsActive, Is.EqualTo(true));
            Assert.That(result.Value[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(result.Value[0].Errors, Is.Empty);

            Assert.That(result.Value[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(result.Value[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(result.Value[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(result.Value[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(result.Value[1].IsActive, Is.EqualTo(false));
            Assert.That(result.Value[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(result.Value[1].Errors, Is.Empty);
        }

        [Test]
        public void TestTryParseWithStreamAndOptionsShouldReturnSuccessResult()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');
            var options = new ParseOptions('|');
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines)));

            var result = parser.TryParse<FileLine>(stream, Encoding.UTF8, options);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Length, Is.EqualTo(2));

            Assert.That(result.Value[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(result.Value[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(result.Value[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(result.Value[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(result.Value[0].IsActive, Is.EqualTo(true));
            Assert.That(result.Value[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(result.Value[0].Errors, Is.Empty);

            Assert.That(result.Value[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(result.Value[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(result.Value[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(result.Value[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(result.Value[1].IsActive, Is.EqualTo(false));
            Assert.That(result.Value[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(result.Value[1].Errors, Is.Empty);
        }

        [Test]
        public async Task TestTryParseAsyncForNullFilepathShouldReturnSuccessResultWithEmptyArray()
        {
            parser = new Parser();

            var result = await parser.TryParseAsync<FileLine>((string)null);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Empty);
        }

        [Test]
        public async Task TestTryParseAsyncForNonExistentFilepathShouldReturnSuccessResultWithEmptyArray()
        {
            parser = new Parser();

            var result = await parser.TryParseAsync<FileLine>("nonexistent.txt");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Empty);
        }

        [Test]
        public async Task TestTryParseAsyncWithFilePathShouldReturnSuccessResult()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "TestFile.txt");

            parser = new Parser();

            var result = await parser.TryParseAsync<FileLine>(filePath);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Length, Is.EqualTo(2));

            Assert.That(result.Value[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(result.Value[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(result.Value[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(result.Value[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(result.Value[0].IsActive, Is.EqualTo(true));
            Assert.That(result.Value[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(result.Value[0].Errors, Is.Empty);

            Assert.That(result.Value[1].Code.Batch, Is.EqualTo("UG"));
            Assert.That(result.Value[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(result.Value[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(result.Value[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(result.Value[1].IsActive, Is.EqualTo(false));
            Assert.That(result.Value[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(result.Value[1].Errors, Is.Empty);
        }

        [Test]
        public async Task TestTryParseAsyncWithFilePathAndOptionsShouldReturnSuccessResult()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "TestFile.txt");

            parser = new Parser();
            var options = new ParseOptions();

            var result = await parser.TryParseAsync<FileLine>(filePath, options);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Length, Is.EqualTo(2));

            Assert.That(result.Value[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(result.Value[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(result.Value[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(result.Value[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(result.Value[0].IsActive, Is.EqualTo(true));
            Assert.That(result.Value[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(result.Value[0].Errors, Is.Empty);

            Assert.That(result.Value[1].Code.Batch, Is.EqualTo("UG"));
            Assert.That(result.Value[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(result.Value[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(result.Value[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(result.Value[1].IsActive, Is.EqualTo(false));
            Assert.That(result.Value[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(result.Value[1].Errors, Is.Empty);
        }

        [Test]
        public async Task TestTryParseAsyncForNullOrEmptyLinesShouldReturnSuccessResultWithEmptyArray()
        {
            parser = new Parser();

            var nullResult = await parser.TryParseAsync<FileLine>((string[])null);
            var emptyResult = await parser.TryParseAsync<FileLine>(Array.Empty<string>());

            Assert.That(nullResult.IsSuccess, Is.True);
            Assert.That(nullResult.Value, Is.Empty);

            Assert.That(emptyResult.IsSuccess, Is.True);
            Assert.That(emptyResult.Value, Is.Empty);
        }

        [Test]
        public async Task TestTryParseAsyncWithLinesShouldReturnSuccessResult()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');

            var result = await parser.TryParseAsync<FileLine>(lines);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Length, Is.EqualTo(2));

            Assert.That(result.Value[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(result.Value[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(result.Value[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(result.Value[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(result.Value[0].IsActive, Is.EqualTo(true));
            Assert.That(result.Value[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(result.Value[0].Errors, Is.Empty);

            Assert.That(result.Value[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(result.Value[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(result.Value[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(result.Value[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(result.Value[1].IsActive, Is.EqualTo(false));
            Assert.That(result.Value[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(result.Value[1].Errors, Is.Empty);
        }

        [Test]
        public async Task TestTryParseAsyncWithLinesAndOptionsShouldReturnSuccessResult()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');
            var options = new ParseOptions('|');

            var result = await parser.TryParseAsync<FileLine>(lines, options);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Length, Is.EqualTo(2));

            Assert.That(result.Value[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(result.Value[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(result.Value[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(result.Value[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(result.Value[0].IsActive, Is.EqualTo(true));
            Assert.That(result.Value[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(result.Value[0].Errors, Is.Empty);

            Assert.That(result.Value[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(result.Value[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(result.Value[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(result.Value[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(result.Value[1].IsActive, Is.EqualTo(false));
            Assert.That(result.Value[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(result.Value[1].Errors, Is.Empty);
        }

        [Test]
        public async Task TestTryParseAsyncForNullByteArrayShouldReturnSuccessResultWithEmptyArray()
        {
            parser = new Parser();

            var result = await parser.TryParseAsync<FileLine>((byte[])null);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Empty);
        }

        [Test]
        public async Task TestTryParseAsyncWithByteArrayShouldReturnSuccessResult()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');
            var bytes = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines));

            var result = await parser.TryParseAsync<FileLine>(bytes);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Length, Is.EqualTo(2));

            Assert.That(result.Value[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(result.Value[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(result.Value[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(result.Value[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(result.Value[0].IsActive, Is.EqualTo(true));
            Assert.That(result.Value[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(result.Value[0].Errors, Is.Empty);

            Assert.That(result.Value[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(result.Value[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(result.Value[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(result.Value[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(result.Value[1].IsActive, Is.EqualTo(false));
            Assert.That(result.Value[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(result.Value[1].Errors, Is.Empty);
        }

        [Test]
        public async Task TestTryParseAsyncWithByteArrayAndOptionsShouldReturnSuccessResult()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');
            var options = new ParseOptions('|');
            var bytes = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines));

            var result = await parser.TryParseAsync<FileLine>(bytes, Encoding.UTF8, options);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Length, Is.EqualTo(2));

            Assert.That(result.Value[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(result.Value[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(result.Value[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(result.Value[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(result.Value[0].IsActive, Is.EqualTo(true));
            Assert.That(result.Value[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(result.Value[0].Errors, Is.Empty);

            Assert.That(result.Value[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(result.Value[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(result.Value[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(result.Value[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(result.Value[1].IsActive, Is.EqualTo(false));
            Assert.That(result.Value[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(result.Value[1].Errors, Is.Empty);
        }

        [Test]
        public async Task TestTryParseAsyncForNullStreamShouldReturnSuccessResultWithEmptyArray()
        {
            parser = new Parser();

            var result = await parser.TryParseAsync<FileLine>((Stream)null);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Empty);
        }

        [Test]
        public async Task TestTryParseAsyncWithStreamShouldReturnSuccessResult()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines)));

            var result = await parser.TryParseAsync<FileLine>(stream);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Length, Is.EqualTo(2));

            Assert.That(result.Value[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(result.Value[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(result.Value[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(result.Value[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(result.Value[0].IsActive, Is.EqualTo(true));
            Assert.That(result.Value[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(result.Value[0].Errors, Is.Empty);

            Assert.That(result.Value[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(result.Value[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(result.Value[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(result.Value[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(result.Value[1].IsActive, Is.EqualTo(false));
            Assert.That(result.Value[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(result.Value[1].Errors, Is.Empty);
        }

        [Test]
        public async Task TestTryParseAsyncWithStreamAndOptionsShouldReturnSuccessResult()
        {
            var lines = new[]
            {
                 "GB-01|Bob Marley|True|Free",
                 "UH-02|John Walsh McKinsey|False|Paid"
             };

            parser = new Parser('|');
            var options = new ParseOptions('|');
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines)));

            var result = await parser.TryParseAsync<FileLine>(stream, Encoding.UTF8, options);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Length, Is.EqualTo(2));

            Assert.That(result.Value[0].Code.Batch, Is.EqualTo("GB"));
            Assert.That(result.Value[0].Code.SerialNo, Is.EqualTo(1));
            Assert.That(result.Value[0].Name.FirstName, Is.EqualTo("Bob"));
            Assert.That(result.Value[0].Name.Surname, Is.EqualTo("Marley"));
            Assert.That(result.Value[0].IsActive, Is.EqualTo(true));
            Assert.That(result.Value[0].Subcription, Is.EqualTo(Subcription.Free));
            Assert.That(result.Value[0].Errors, Is.Empty);

            Assert.That(result.Value[1].Code.Batch, Is.EqualTo("UH"));
            Assert.That(result.Value[1].Code.SerialNo, Is.EqualTo(2));
            Assert.That(result.Value[1].Name.FirstName, Is.EqualTo("John Walsh"));
            Assert.That(result.Value[1].Name.Surname, Is.EqualTo("McKinsey"));
            Assert.That(result.Value[1].IsActive, Is.EqualTo(false));
            Assert.That(result.Value[1].Subcription, Is.EqualTo(Subcription.Paid));
            Assert.That(result.Value[1].Errors, Is.Empty);
        }
    }
}