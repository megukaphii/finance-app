using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FinanceApp.Data.Utility;

namespace FinanceApp.Data.Interfaces;

public interface IResponse
{
    public bool Success { get; init; }

    // TODO - Can we derive T from the instance's class?
    public async Task Send<T>(Client client) where T : IResponse
    {
        string strResponse = JsonSerializer.Serialize((T)this, new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        });
        byte[] message = Encoding.UTF8.GetBytes(strResponse + Serialization.Eof);
        await client.Stream.WriteAsync(message);
        await client.Stream.FlushAsync();
    }
}