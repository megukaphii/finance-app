using FinanceApp.Data.Exceptions;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Requests.Account;
using FinanceApp.Data.Utility;
using NUnit.Framework;

namespace FinanceApp.ServerTests.Interfaces;

[TestFixture]
[TestOf(typeof(IRequest))]
// ReSharper disable once InconsistentNaming
public class IRequestTest
{
	[SetUp]
	public void SetUp()
	{
		CreateAccount request = new()
		{
			Name = new()
			{
				Value = "Test Name"
			},
			Description = new()
			{
				Value = "Test Description"
			}
		};
		_requestUnderTest = Serialization.Serialize(request);
	}

	private string _requestUnderTest = string.Empty;

	[Test]
	public void GetRequest_WithValidMessageType_ShouldReturnRequestObject()
	{
		dynamic resultRequest = IRequest.GetRequest(_requestUnderTest);

		Assert.That(resultRequest.GetType(), Is.EqualTo(typeof(CreateAccount)));
	}

	[Test]
	public void GetRequest_WithInvalidMessageType_ShouldThrowInvalidRequest()
	{
		string invalidRequest = _requestUnderTest.Remove(_requestUnderTest.Length / 2);

		Assert.Throws<InvalidRequestException>(() => IRequest.GetRequest(invalidRequest));
	}

	[Test]
	public void GetRequest_WithInvalidMessage_ShouldThrowInvalidMessage()
	{
		string invalidMessage = _requestUnderTest.Replace(CreateAccount.Flag, "Completely invalid message");

		Assert.Throws<InvalidMessageException>(() => IRequest.GetRequest(invalidMessage));
	}
}