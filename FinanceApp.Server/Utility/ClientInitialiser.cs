using FinanceApp.Data.Requests;
using FinanceApp.Data.Utility;
using FinanceApp.Server.Extensions;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Utility;

public class ClientInitialiser : ConnectionInitialiser
{
	private readonly IClient _client;

	public ClientInitialiser(IClient client) => _client = client;

	protected override async Task<bool> IsCompatibleAsync()
	{
		try {
			string messageReceived = await _client.ReadMessageAsync();
			CompareVersion request =
				RequestResolver.Deserialize<CompareVersion>(messageReceived)
				?? throw new($"Malformed {nameof(CompareVersion)} request received");

			Version serverVersion = ThisAssembly.Git.SemVer.Version;
			bool isCompatible = serverVersion.IsCompatible(request.SemanticVersion);
			CompareVersionResponse response = new()
			{
				Success = isCompatible,
				SemanticVersion = serverVersion
			};
			await _client.Send(response);

			if (!isCompatible)
				_client.WriteLine($"Client has incompatible version - {request.SemanticVersion}");

			return isCompatible;
		} catch {
			_client.WriteLine($"Client did not send appropriate {nameof(CompareVersion)} request, disconnecting.");
			return false;
		}
	}
}