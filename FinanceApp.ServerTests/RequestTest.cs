using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Requests.Transaction;
using FinanceApp.Data.Validators;
using NUnit.Framework;
using System.Text.Json;

namespace FinanceApp.ServerTests;

[TestFixture]
public class RequestTest
{
    private static readonly Create TestRequest = new()
    {
        Counterparty = new()
        {
            Value = new()
            {
                Name = "John"
            }
        },
        Value = new()
        {
            Value = 100
        },
        Timestamp = new()
        {
            Value = DateTime.Now.Date
        }
    };
	private static readonly string Message = $"<CreateTransaction>{JsonSerializer.Serialize(TestRequest)}";

    [Test]
    public void GetRequest()
    {
        IRequest request = IRequest.GetRequest(Message);

        Assert.IsInstanceOf<Create>(request);
    }

    [Test]
    public void ValidatorExists()
    {
        IRequest request = IRequest.GetRequest(Message);

        Assert.DoesNotThrow(() => IRequest.IsValid(request));
        Assert.IsTrue(request is ISingleTransaction);
        Assert.AreEqual(typeof(TransactionValidator), ISingleTransaction.Validator);
    }
}