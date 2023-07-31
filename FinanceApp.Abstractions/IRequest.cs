using System.Net.Security;

namespace FinanceApp.Abstractions;

public interface IRequest
{
    //public void Handle(SslStream sslStream);
    public static abstract string GetFlag();
}