using FinanceApp.Data.Extensions;
using FinanceApp.Data.Requests;
using FinanceApp.Data.Utility;
using FinanceApp.MauiClient.Services;

namespace FinanceApp.MauiClient.Classes;

public class ServerInitialiser(ServerConnection serverConnection) : ConnectionInitialiser
{
	protected override async Task<bool> IsCompatibleAsync()
	{
		try {
			CompareVersion request = new()
			{
				SemanticVersion = AppInfo.Version
			};
			CompareVersionResponse response =
				await serverConnection.SendMessageAsync<CompareVersion, CompareVersionResponse>(request);

			if (request.SemanticVersion.IsCompatible(AppInfo.Version)) {
				return true;
			} else {
				await Shell.Current.DisplayAlert("Version Issue",
					$"Server version {response.SemanticVersion} is not compatible with {request.SemanticVersion}",
					"OK");
				return false;
			}
		} catch {
			await Shell.Current.DisplayAlert("Version Issue", "Could not compare version against server.", "OK");
			return false;
		}
	}
}