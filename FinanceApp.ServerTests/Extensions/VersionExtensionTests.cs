using FinanceApp.Server.Extensions;

namespace FinanceApp.ServerTests.Extensions;

[TestFixture]
[TestOf(typeof(VersionExtensions))]
public class VersionExtensionsTests
{
	[Test]
	public void IsCompatible_ShouldReturnTrue_WhenBothMajorVersionsAreZeroAndMinorVersionsMatch()
	{
		Version version = new(0, 1);
		Version other = new(0, 1);

		bool isCompatible = version.IsCompatible(other);

		Assert.That(isCompatible, Is.True);
	}

	[Test]
	public void IsCompatible_ShouldReturnFalse_WhenBothMajorVersionsAreZeroButMinorVersionsDoNotMatch()
	{
		Version version = new(0, 1);
		Version other = new(0, 2);

		bool isCompatible = version.IsCompatible(other);

		Assert.That(isCompatible, Is.False);
	}

	[Test]
	public void IsCompatible_ShouldReturnTrue_WhenMajorVersionsMatch()
	{
		Version version = new(1, 0);
		Version other = new(1, 1);

		bool isCompatible = version.IsCompatible(other);

		Assert.That(isCompatible, Is.True);
	}

	[Test]
	public void IsCompatible_ShouldReturnFalse_WhenMajorVersionsDoNotMatch()
	{
		Version version = new(1, 0);
		Version other = new(2, 0);

		bool isCompatible = version.IsCompatible(other);

		Assert.That(isCompatible, Is.False);
	}
}