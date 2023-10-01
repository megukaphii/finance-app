internal partial class ThisAssembly
{
	public static partial class Git
	{
		public static partial class SemVer
		{
			private const string _versionStr = Major + "." + Minor + "." + Patch;
			public static readonly Version Version = Version.Parse(_versionStr);
		}
	}
}