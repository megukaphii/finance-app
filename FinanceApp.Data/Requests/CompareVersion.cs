using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Requests;

public class CompareVersion : IRequest
{
	public required Version SemanticVersion { get; init; }
	public static string Flag => "<CompareVersion>";

	public override string ToString() => $"{nameof(SemanticVersion)}: {SemanticVersion}";

	public override int GetHashCode() => HashCode.Combine(SemanticVersion);
}

public class CompareVersionResponse : IResponse
{
	public required Version SemanticVersion { get; init; }
	public required bool Success { get; init; }

	public override string ToString() => $"{nameof(Success)}: {Success}, {nameof(SemanticVersion)}: {SemanticVersion}";
}