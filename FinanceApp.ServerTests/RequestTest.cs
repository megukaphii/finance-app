using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Requests.Transaction;
using FinanceApp.Data.Validators;
using NUnit.Framework;
using System.Text.Json;

namespace FinanceApp.ServerTests;

[TestFixture]
public class RequestTest
{
    private static readonly CreateTransaction TestRequest = new()
    {
        Value = new()
        {
            Value = 100
        },
        Counterparty = new()
        {
            Value = new()
            {
                Name = "John"
            }
        }
    };
	private static readonly string Message = $"<CreateTransaction>{JsonSerializer.Serialize(TestRequest)}";

    [Test]
    public void GetRequest()
    {
        IRequest request = IRequest.GetRequest(Message);

        Assert.IsInstanceOf<CreateTransaction>(request);
    }

    [Test]
    public void ValidatorExists()
    {
        IRequest request = IRequest.GetRequest(Message);

        // TODO - This test doesn't work! It doesn't actually confirm the Validator is run, because IsValid skips if it doesn't find the Validator
        Assert.DoesNotThrowAsync(async () => await IRequest.IsValidAsync(request, new()));
        Assert.IsTrue(request is ISingleTransaction);
        Assert.AreEqual(typeof(TransactionValidator), ISingleTransaction.Validator);
    }
}