using parsley;

namespace Parsley.Tests
{
    [TestFixture]
    public class ParseOptionsFixture
    {
        [Test]
        public void TestParseOptionsConstructorShouldInitializeWithDefaults()
        {
            var options = new ParseOptions();

            Assert.That(options.Delimiter, Is.EqualTo(','));
            Assert.That(options.SkipHeaderLine, Is.False);
            Assert.That(options.TrimFieldValues, Is.True);
            Assert.That(options.IncludeEmptyLines, Is.True);
            Assert.That(options.MaxErrors, Is.EqualTo(-1));
            Assert.That(options.BufferSize, Is.EqualTo(1024));
        }

        [Test]
        public void TestParseOptionsConstructorWithDelimiterShouldInitializeWithSpecifiedDelimiter()
        {
            var options = new ParseOptions(';');

            Assert.That(options.Delimiter, Is.EqualTo(';'));
            Assert.That(options.SkipHeaderLine, Is.False);
            Assert.That(options.TrimFieldValues, Is.True);
            Assert.That(options.IncludeEmptyLines, Is.True);
            Assert.That(options.MaxErrors, Is.EqualTo(-1));
            Assert.That(options.BufferSize, Is.EqualTo(1024));
        }

        [Test]
        public void TestParseOptionsPropertiesShouldBeSettable()
        {
            var options = new ParseOptions
            {
                Delimiter = '|',
                SkipHeaderLine = true,
                TrimFieldValues = false,
                IncludeEmptyLines = false,
                MaxErrors = 5,
                BufferSize = 2048
            };

            Assert.That(options.Delimiter, Is.EqualTo('|'));
            Assert.That(options.SkipHeaderLine, Is.True);
            Assert.That(options.TrimFieldValues, Is.False);
            Assert.That(options.IncludeEmptyLines, Is.False);
            Assert.That(options.MaxErrors, Is.EqualTo(5));
            Assert.That(options.BufferSize, Is.EqualTo(2048));
        }
    }
}