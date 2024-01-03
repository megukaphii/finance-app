using FinanceApp.Data.Requests;
using FinanceApp.Data.Utility;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using NSubstitute;
using NUnit.Framework;

namespace FinanceApp.ServerTests.Utility;

[TestFixture]
[TestOf(typeof(ClientInitialiser))]
public class ClientInitialiserTests
{
	[SetUp]
	public void SetUp()
	{
		_client = Substitute.For<IClient>();
		_clientInitialiser = new(_client);
	}

	private ClientInitialiser _clientInitialiser = null!;
	private IClient _client = null!;

	[Test]
	public async Task Initialise_ReturnsTrueWhenClientVersionIsCompatible()
	{
		string compatibleVersion = ThisAssembly.Git.SemVer.Version.ToString();
		string message = Serialization.Serialize(new CompareVersion { SemanticVersion = new(compatibleVersion) });
		_client.ReadMessageAsync().Returns(message);

		bool result = await _clientInitialiser.Initialise();

		Assert.That(result);
		await _client.Received().Send(Arg.Any<CompareVersionResponse>());
	}

	[Test]
	public async Task Initialise_ReturnsFalseWhenClientVersionIsNotCompatible()
	{
		const string incompatibleVersion = "0.0.0";
		string message = Serialization.Serialize(new CompareVersion { SemanticVersion = new(incompatibleVersion) });
		_client.ReadMessageAsync().Returns(message);

		bool result = await _clientInitialiser.Initialise();

		Assert.That(!result);
		await _client.Received().Send(Arg.Any<CompareVersionResponse>());
	}

	[Test]
	public async Task Initialise_ReturnsFalseWhenMalformedRequestReceived()
	{
		_client.ReadMessageAsync().Returns("malformed request message");

		bool result = await _clientInitialiser.Initialise();

		Assert.That(!result);
		await _client.DidNotReceive().Send(Arg.Any<CompareVersionResponse>());
		_client.Received().WriteLine(Arg.Any<string>());
	}
}