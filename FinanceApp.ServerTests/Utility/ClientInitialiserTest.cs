using FinanceApp.Data.Requests;
using FinanceApp.Data.Utility;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using NSubstitute;
using NUnit.Framework;

namespace FinanceApp.ServerTests.Classes;

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
		// Arrange
		string compatibleVersion = ThisAssembly.Git.SemVer.Version.ToString();
		string message = Serialization.Serialize(new CompareVersion { SemanticVersion = new(compatibleVersion) });

		_client.ReadMessageAsync().Returns(message);

		// Act
		bool result = await _clientInitialiser.Initialise();

		// Assert
		Assert.That(result);
		await _client.Received().Send(Arg.Any<CompareVersionResponse>());
	}

	[Test]
	public async Task Initialise_ReturnsFalseWhenClientVersionIsNotCompatible()
	{
		// Arrange
		const string incompatibleVersion = "0.0.0";
		string message = Serialization.Serialize(new CompareVersion { SemanticVersion = new(incompatibleVersion) });

		_client.ReadMessageAsync().Returns(message);

		Console.WriteLine(message);
		// Act
		bool result = await _clientInitialiser.Initialise();

		// Assert
		Assert.That(!result);
		await _client.Received().Send(Arg.Any<CompareVersionResponse>());
	}

	[Test]
	public async Task Initialise_ReturnsFalseWhenMalformedRequestReceived()
	{
		// Arrange
		_client.ReadMessageAsync().Returns("malformed request message");

		// Act
		bool result = await _clientInitialiser.Initialise();

		// Assert
		Assert.That(!result);
		await _client.DidNotReceive().Send(Arg.Any<CompareVersionResponse>());
		_client.Received().WriteLine(Arg.Any<string>());
	}
}