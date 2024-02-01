using System.Diagnostics.CodeAnalysis;
using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Server.Utility;
using FinanceApp.Server.Validators;
using NSubstitute;

namespace FinanceApp.ServerTests.Validators;

[TestFixture]
[TestOf(typeof(SingleAccountValidator))]
public class SingleAccountValidatorTests
{
	[SetUp]
	public void SetUp()
	{
		_singleAccountValidator = new();
	}

	private static readonly int MinNameLength = PropertyHelpers.GetMinLength((Account a) => a.Name);
	private static readonly int MaxNameLength = PropertyHelpers.GetMaxLength((Account a) => a.Name);
	private static readonly int SafeNameLength = (MinNameLength + MaxNameLength) / 2;
	private static readonly int MinDescriptionLength = PropertyHelpers.GetMinLength((Account a) => a.Description);
	private static readonly int MaxDescriptionLength = PropertyHelpers.GetMaxLength((Account a) => a.Description);
	private static readonly int SafeDescriptionLength = (MinDescriptionLength + MaxDescriptionLength) / 2;

	private SingleAccountValidator _singleAccountValidator = null!;

	[Test]
	public async Task ValidateAsync_ShouldReturnTrue_WhenNameAndDescriptionAreWithinValidLengths()
	{
		ISingleAccount request = Substitute.For<ISingleAccount>();
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });
		request.Description.Returns(new RequestField<string> { Value = new('a', SafeDescriptionLength) });

		bool result = await _singleAccountValidator.ValidateAsync(request);

		Assert.That(result, Is.True);
		Assert.That(request.Name.Error, Is.Empty);
		Assert.That(request.Description.Error, Is.Empty);
	}

	[Test]
	[ExcludeFromCodeCoverage]
	public async Task ValidateAsync_ShouldReturnFalse_WhenNameIsTooShort()
	{
		if (MinNameLength == 0) {
			Assert.Pass($"{nameof(MinNameLength)} is 0, validation cannot be failed");
		}

		ISingleAccount request = Substitute.For<ISingleAccount>();
		request.Name.Returns(new RequestField<string> { Value = new('a', MinNameLength - 1) });
		request.Description.Returns(new RequestField<string> { Value = new('a', SafeDescriptionLength) });

		bool result = await _singleAccountValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Name.Error, Is.Not.Empty);
	}

	[Test]
	[ExcludeFromCodeCoverage]
	public async Task ValidateAsync_ShouldReturnFalse_WhenDescriptionIsTooShort()
	{
		if (MinDescriptionLength == 0) {
			Assert.Pass($"{nameof(MinDescriptionLength)} is 0, validation cannot be failed");
		}

		ISingleAccount request = Substitute.For<ISingleAccount>();
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });
		request.Description.Returns(new RequestField<string> { Value = new('a', MinDescriptionLength - 1) });

		bool result = await _singleAccountValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Description.Error, Is.Not.Empty);
	}

	[Test]
	[ExcludeFromCodeCoverage]
	public async Task ValidateAsync_ShouldReturnFalse_WhenNameAndDescriptionIsTooShort()
	{
		if (MinNameLength == 0 && MinDescriptionLength == 0) {
			Assert.Pass(
				$"{nameof(MinNameLength)} and {nameof(MinDescriptionLength)} is 0, validation cannot be failed");
		}

		ISingleAccount request = Substitute.For<ISingleAccount>();
		request.Name.Returns(new RequestField<string> { Value = new('a', int.Max(MinNameLength - 1, 0)) });
		request.Description.Returns(new RequestField<string>
			{ Value = new('a', int.Max(MinDescriptionLength - 1, 0)) });

		bool result = await _singleAccountValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		if (MinNameLength != 0) Assert.That(request.Name.Error, Is.Not.Empty);
		if (MinDescriptionLength != 0) Assert.That(request.Description.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenNameIsTooLong()
	{
		ISingleAccount request = Substitute.For<ISingleAccount>();
		request.Name.Returns(new RequestField<string> { Value = new('a', MaxNameLength + 1) });
		request.Description.Returns(new RequestField<string> { Value = new('a', SafeDescriptionLength) });

		bool result = await _singleAccountValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Name.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenDescriptionIsTooLong()
	{
		ISingleAccount request = Substitute.For<ISingleAccount>();
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });
		request.Description.Returns(new RequestField<string> { Value = new('a', MaxDescriptionLength + 1) });

		bool result = await _singleAccountValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Description.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenNameAndDescriptionIsTooLong()
	{
		ISingleAccount request = Substitute.For<ISingleAccount>();
		request.Name.Returns(new RequestField<string> { Value = new('a', MaxNameLength + 1) });
		request.Description.Returns(new RequestField<string> { Value = new('a', MaxDescriptionLength + 1) });

		bool result = await _singleAccountValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Name.Error, Is.Not.Empty);
		Assert.That(request.Description.Error, Is.Not.Empty);
	}
}