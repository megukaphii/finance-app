using FinanceApp.Data.RequestPatterns;
using FinanceApp.Server.Validators;
using NSubstitute;

namespace FinanceApp.ServerTests.Validators;

[TestFixture]
[TestOf(typeof(PageNumberValidator))]
public class PageNumberValidatorTests
{
	[SetUp]
	public void SetUp()
	{
		_pageNumberValidator = new();
	}

	private PageNumberValidator _pageNumberValidator = null!;

	[Test]
	public async Task ValidateAsync_ShouldReturnTrue_WhenPageNumberIsWithinValidRange()
	{
		IPageNumber request = Substitute.For<IPageNumber>();
		request.Page.Returns(new RequestField<long> { Value = 1 });

		bool result = await _pageNumberValidator.ValidateAsync(request);

		Assert.That(result, Is.True);
		Assert.That(request.Page.Error, Is.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenPageNumberIsNegative()
	{
		IPageNumber request = Substitute.For<IPageNumber>();
		request.Page.Returns(new RequestField<long> { Value = -1 });

		bool result = await _pageNumberValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Page.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenPageNumberExceedsMaximumAllowed()
	{
		IPageNumber request = Substitute.For<IPageNumber>();
		request.Page.Returns(new RequestField<long> { Value = long.MaxValue });

		bool result = await _pageNumberValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Page.Error, Is.Not.Empty);
	}
}