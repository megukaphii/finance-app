public static class VersionExtensions
{
	public static bool IsCompatible(this Version version, Version other)
	{
		if (version.Major == 0) {
			if (version.Minor != other.Minor) {
				return false;
			} else {
				return true;
			}
		} else {
			if (version.Major == other.Major) {
				return true;
			}
		}
		return false;
	}
}