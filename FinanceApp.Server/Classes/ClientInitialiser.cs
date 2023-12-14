using FinanceApp.Data.Extensions;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Requests;
using FinanceApp.Data.Utility;

namespace FinanceApp.Server.Classes;

public class ClientInitialiser : ConnectionInitialiser
{
	private readonly IClient _client;

	public ClientInitialiser(IClient client) => _client = client;

	protected override async Task<bool> IsCompatibleAsync()
	{
		try {
			string messageReceived = await _client.ReadMessageAsync();
			CompareVersion request =
				Serialization.Deserialize<CompareVersion>(messageReceived.Replace(CompareVersion.Flag, string.Empty))
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