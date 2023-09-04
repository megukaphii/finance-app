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
    private static readonly TransactionCreate TestRequest = new()
    {
        Value = new RequestField<long>
        {
            Value = 100
        },
        Counterparty = new RequestField<Counterparty>
        {
            Value = new Counterparty
            {
                Name = "John"
            }
        }
    };
    private static readonly string Message = $"<CreateTransaction>{JsonConvert.SerializeObject(TestRequest)}";

    [Test]
    public void GetRequest()
    {
        IRequest request = IRequest.GetRequest(Message);

        Assert.IsInstanceOf<TransactionCreate>(request);
    }

    [Test]
    public void ValidatorExists()
    {
        IRequest request = IRequest.GetRequest(Message);

        Assert.DoesNotThrow(() => request.Validate());
        Assert.IsTrue(request is ISingleTransaction);
        Assert.AreEqual(typeof(TransactionValidator), ISingleTransaction.Validator);
    }
}