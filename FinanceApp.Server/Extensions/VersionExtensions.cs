namespace FinanceApp.Server.Extensions;

public static class VersionExtensions
{
	public static bool IsCompatible(this Version version, Version other)
	{
		if (version.Major == 0) {
			return version.Minor == other.Minor;
		} else {
			if (version.Major == other.Major) return true;
		}

		return false;
	}
}