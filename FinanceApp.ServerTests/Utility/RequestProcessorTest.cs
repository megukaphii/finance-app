using FinanceApp.Data.Interfaces;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using NSubstitute;
using NUnit.Framework;

namespace FinanceApp.ServerTests.Utility;

[TestFixture]
[TestOf(typeof(RequestProcessor))]
public class RequestProcessorTest
{
	[SetUp]
	public void Setup()
	{
		_mockServiceProvider = Substitute.For<IServiceProvider>();
		_mockValidatorResolver = Substitute.For<IValidatorResolver>();
		_requestProcessor = new RequestProcessor(_mockServiceProvider, _mockValidatorResolver);
	}

	private IServiceProvider _mockServiceProvider = null!;
	private IValidatorResolver _mockValidatorResolver = null!;
	private IRequestProcessor _requestProcessor = null!;

	[Test]
	public async Task ProcessAsync_Returns_If_RequestIsValid()
	{
		IRequest? mockRequest = Substitute.For<IRequest>();
		IClient? mockClient = Substitute.For<IClient>();
		using MemoryStream memoryStream = new();
		mockClient.Stream.Returns(memoryStream);
		IRequestHandler<IRequest>? mockRequestHandler = Substitute.For<IRequestHandler<IRequest>>();
		Type test = typeof(IRequestHandler<>).MakeGenericType(mockRequest.GetType());
		_mockServiceProvider.GetService(test).Returns(mockRequestHandler);
		IValidator<IRequest>? mockValidator = Substitute.For<IValidator<IRequest>>();
		mockValidator.ValidateAsync(mockRequest).Returns(true);
		_mockValidatorResolver.GetValidator<IRequest>().Returns(mockValidator);

		await _requestProcessor.ProcessAsync(mockRequest, mockClient);

		await mockRequestHandler.Received().HandleAsync(mockRequest, mockClient);
	}

	[Test]
	public async Task ProcessAsync_Calls_SendErrorResponseAsync_If_RequestIsNotValid()
	{
		IRequest? mockRequest = Substitute.For<IRequest>();
		IClient? mockClient = Substitute.For<IClient>();
		MemoryStream stream = new();
		mockClient.Stream.Returns(stream);
		IValidator<IRequest>? mockValidator = Substitute.For<IValidator<IRequest>>();
		_mockValidatorResolver.GetValidator<IRequest>().Returns(mockValidator);
		mockValidator.ValidateAsync(mockRequest).Returns(false);

		await _requestProcessor.ProcessAsync(mockRequest, mockClient);

		Assert.That(stream.Length, Is.GreaterThan(0));
	}
}