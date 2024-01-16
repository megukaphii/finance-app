internal partial class ThisAssembly
{
	public static partial class Git
	{
		public static partial class SemVer
		{
			private const string VersionStr = Major + "." + Minor + "." + Patch;
			public static readonly Version Version = Version.Parse(VersionStr);
		}
	}
}