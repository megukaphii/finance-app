using System.Diagnostics.CodeAnalysis;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.RequestPatterns.Account;
using FinanceApp.Server.Utility;
using FinanceApp.Server.Validators.Account;
using NSubstitute;

namespace FinanceApp.ServerTests.Validators.Account;

[TestFixture]
[TestOf(typeof(AccountFieldsValidator))]
public class AccountFieldsValidatorTest
{
	[SetUp]
	public void SetUp()
	{
		_accountFieldsValidator = new();
	}

	private static readonly int MinNameLength = PropertyHelpers.GetMinLength((Data.Models.Account a) => a.Name);
	private static readonly int MaxNameLength = PropertyHelpers.GetMaxLength((Data.Models.Account a) => a.Name);
	private static readonly int SafeNameLength = (MinNameLength + MaxNameLength) / 2;
	private static readonly int MinDescriptionLength =
		PropertyHelpers.GetMinLength((Data.Models.Account a) => a.Description);
	private static readonly int MaxDescriptionLength =
		PropertyHelpers.GetMaxLength((Data.Models.Account a) => a.Description);
	private static readonly int SafeDescriptionLength = (MinDescriptionLength + MaxDescriptionLength) / 2;

	private AccountFieldsValidator _accountFieldsValidator = null!;

	[Test]
	public async Task ValidateAsync_ShouldReturnTrue_WhenNameAndDescriptionAreOfValidLength()
	{
		IAccountFields request = Substitute.For<IAccountFields>();
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });
		request.Description.Returns(new RequestField<string> { Value = new('a', SafeDescriptionLength) });

		bool result = await _accountFieldsValidator.ValidateAsync(request);

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

		IAccountFields request = Substitute.For<IAccountFields>();
		request.Name.Returns(new RequestField<string> { Value = new('a', MinNameLength - 1) });
		request.Description.Returns(new RequestField<string> { Value = new('a', SafeDescriptionLength) });

		bool result = await _accountFieldsValidator.ValidateAsync(request);

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

		IAccountFields request = Substitute.For<IAccountFields>();
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });
		request.Description.Returns(new RequestField<string> { Value = new('a', MinDescriptionLength - 1) });

		bool result = await _accountFieldsValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Description.Error, Is.Not.Empty);
	}

	[Test]
	[ExcludeFromCodeCoverage]
	public async Task ValidateAsync_ShouldReturnFalse_WhenNameAndDescriptionAreTooShort()
	{
		if (MinNameLength == 0 && MinDescriptionLength == 0) {
			Assert.Pass(
				$"{nameof(MinNameLength)} and {nameof(MinDescriptionLength)} is 0, validation cannot be failed");
		}

		IAccountFields request = Substitute.For<IAccountFields>();
		request.Name.Returns(new RequestField<string> { Value = new('a', int.Max(MinNameLength - 1, 0)) });
		request.Description.Returns(new RequestField<string>
			{ Value = new('a', int.Max(MinDescriptionLength - 1, 0)) });

		bool result = await _accountFieldsValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		if (MinNameLength != 0) Assert.That(request.Name.Error, Is.Not.Empty);
		if (MinDescriptionLength != 0) Assert.That(request.Description.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenNameIsTooLong()
	{
		IAccountFields request = Substitute.For<IAccountFields>();
		request.Name.Returns(new RequestField<string> { Value = new('a', MaxNameLength + 1) });
		request.Description.Returns(new RequestField<string> { Value = new('a', SafeDescriptionLength) });

		bool result = await _accountFieldsValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Name.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenDescriptionIsTooLong()
	{
		IAccountFields request = Substitute.For<IAccountFields>();
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });
		request.Description.Returns(new RequestField<string> { Value = new('a', MaxDescriptionLength + 1) });

		bool result = await _accountFieldsValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Description.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenNameAndDescriptionAreTooLong()
	{
		IAccountFields request = Substitute.For<IAccountFields>();
		request.Name.Returns(new RequestField<string> { Value = new('a', MaxNameLength + 1) });
		request.Description.Returns(new RequestField<string> { Value = new('a', MaxDescriptionLength + 1) });

		bool result = await _accountFieldsValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Name.Error, Is.Not.Empty);
		Assert.That(request.Description.Error, Is.Not.Empty);
	}
}