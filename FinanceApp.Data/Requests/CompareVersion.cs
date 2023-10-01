namespace FinanceApp.Data.Requests;

public class CompareVersion
{
	public required Version SemanticVersion { get; init; }

	public override string ToString()
	{
		return $"{nameof(SemanticVersion)}: {SemanticVersion}";
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj))
			return false;
		if (ReferenceEquals(this, obj))
			return true;
		if (obj.GetType() != GetType())
			return false;
		return Equals((CompareVersion) obj);
	}

	private bool Equals(CompareVersion other)
	{
		return SemanticVersion.Equals(other.SemanticVersion);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(SemanticVersion);
	}
}