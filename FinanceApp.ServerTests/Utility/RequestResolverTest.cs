using FinanceApp.Data.Requests.Account;
using FinanceApp.Data.Utility;
using FinanceApp.Server.Exceptions;
using FinanceApp.Server.Utility;

namespace FinanceApp.ServerTests.Utility;

[TestFixture]
[TestOf(typeof(RequestResolver))]
public class RequestResolverTest
{
	[SetUp]
	public void SetUp()
	{
		CreateAccount request = new()
		{
			Name = new() { Value = "Test Name" },
			Description = new() { Value = "Test Description" }
		};
		_requestUnderTest = Serialization.SerializeRequest(request);
	}

	private string _requestUnderTest = string.Empty;

	[Test]
	public void GetRequest_WithValidMessageFlag_ShouldReturnRequestObject()
	{
		dynamic resultRequest = RequestResolver.GetRequest(_requestUnderTest);

		Assert.That(resultRequest.GetType(), Is.EqualTo(typeof(CreateAccount)));
	}

	[Test]
	public void GetRequest_WithInvalidMessageFlag_ShouldThrowInvalidRequest()
	{
		string invalidRequest = _requestUnderTest.Remove(_requestUnderTest.Length / 2);

		Assert.Throws<InvalidRequestException>(() => RequestResolver.GetRequest(invalidRequest));
	}

	[Test]
	public void GetRequest_WithInvalidMessage_ShouldThrowInvalidMessage()
	{
		string invalidMessage = _requestUnderTest.Replace(CreateAccount.Flag, "Completely invalid message");

		Assert.Throws<InvalidMessageException>(() => RequestResolver.GetRequest(invalidMessage));
	}
}