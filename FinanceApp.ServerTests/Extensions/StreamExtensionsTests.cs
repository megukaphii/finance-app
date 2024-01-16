using FinanceApp.Data.Extensions;
using FinanceApp.Data.Requests.Account;
using FinanceApp.Data.Utility;

namespace FinanceApp.ServerTests.Extensions;

[TestFixture]
[TestOf(typeof(StreamExtensions))]
public class StreamExtensionsTests
{
	[Test]
	public async Task SendRequestAsync_ShouldSendRequestSuccessfully()
	{
		MemoryStream memoryStream = new();
		CreateAccount accountRequest = new()
		{
			Name = new() { Value = "Test" },
			Description = new() { Value = "Test Description" }
		};

		await memoryStream.SendRequestAsync(accountRequest);

		string sentMessage = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
		Assert.That(sentMessage, Does.Contain(CreateAccount.Flag));
		Assert.That(sentMessage, Does.Contain(accountRequest.Name.Value));
		Assert.That(sentMessage, Does.Contain(accountRequest.Description.Value));
		Assert.That(sentMessage, Does.Contain(Serialization.Eof));
		memoryStream.Close();
	}

	[Test]
	public async Task SendResponseAsync_ShouldSendResponseSuccessfully()
	{
		MemoryStream memoryStream = new();
		CreateAccountResponse accountResponse = new()
		{
			Success = true,
			Id = 1
		};

		await memoryStream.SendResponseAsync(accountResponse);

		string sentMessage = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
		Assert.That(sentMessage, Does.Contain(accountResponse.Success.ToString().ToLower()));
		Assert.That(sentMessage, Does.Contain(accountResponse.Id.ToString()));
		Assert.That(sentMessage, Does.Contain(Serialization.Eof));
		memoryStream.Close();
	}

	[Test]
	public async Task ReadMessageAsync_ShouldReadMessageSuccessfully()
	{
		const string messageStr = "TestMessage<EOF>";
		MemoryStream memoryStream = new(System.Text.Encoding.UTF8.GetBytes(messageStr));

		string readMessage = await memoryStream.ReadMessageAsync();

		Assert.That(messageStr, Is.EqualTo(readMessage));
		memoryStream.Close();
	}
}