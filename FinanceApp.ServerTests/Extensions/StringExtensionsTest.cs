using FinanceApp.Data.Extensions;
using NUnit.Framework;

namespace FinanceApp.ServerTests.Extensions;

[TestFixture]
[TestOf(typeof(StringExtensions))]
public class StringExtensionsTest
{
	[Test]
	[TestCase(null, 5, "...")]
	[TestCase("", 5, "...")]
	[TestCase("Test", 5, "...")]
	[TestCase("LongTextTest", 5, "...")]
	[TestCase("Test", 0, "...")]
	public void Truncate_ShouldHandleCornerCasesCorrectly(string? input, int maxLength, string truncationSuffix)
	{
		string? result = input.Truncate(maxLength, truncationSuffix);

		if (input == null) {
			Assert.That(result, Is.Null);
		} else if (input.Length <= maxLength) {
			Assert.That(input, Is.EqualTo(result));
		} else {
			Assert.That(string.Concat(input.AsSpan(0, maxLength), truncationSuffix), Is.EqualTo(result));
		}
	}

	[Test]
	public void Truncate_ShouldThrowOnNegativeMaxLength()
	{
		const string input = "Test";

		Assert.Throws<ArgumentOutOfRangeException>(() => input.Truncate(-1, "..."));
	}
}