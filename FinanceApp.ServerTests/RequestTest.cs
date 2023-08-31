using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Requests;
using FinanceApp.Data.Validators;
using NUnit.Framework;
using Newtonsoft.Json;

namespace FinanceApp.ServerTests;

[TestFixture]
public class RequestTest
{
    private static readonly CreateTransaction TestRequest = new()
    {
        Value = 100,
        Counterparty = new Counterparty
        {
            Name = "John"
        }
    };
    private static readonly string Message = $"<CreateTransaction>{JsonConvert.SerializeObject(TestRequest)}";

    [Test]
    public void TestGetRequest()
    {
        IRequest request = IRequest.GetRequest(Message);

        Assert.IsInstanceOf<CreateTransaction>(request);
    }

    [Test]
    public void TestValidatorExists()
    {
        IRequest request = IRequest.GetRequest(Message);

        Assert.DoesNotThrow(() => request.Validate());
        Assert.IsTrue(request is ISingleTransaction);
        Assert.AreEqual(typeof(TransactionValidator), ISingleTransaction.Validator);
    }
}