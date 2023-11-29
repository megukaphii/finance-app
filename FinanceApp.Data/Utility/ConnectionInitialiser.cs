namespace FinanceApp.Data.Utility;

public abstract class ConnectionInitialiser
{
    public async Task<bool> Initialise()
    {
        bool success = true;
        success &= await IsCompatibleAsync();
        return success;
    }

    protected abstract Task<bool> IsCompatibleAsync();
}