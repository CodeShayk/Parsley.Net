using parsley;
using Microsoft.Extensions.DependencyInjection;
using Parsley.Tests.FileLines;

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
        public void TestParseWithFileInputShouldReturnCorrectlyParsedArray()
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
    }
}