using FluentValidation;
using FluentValidation.Results;
using Moq;
using Rpssl.Application.Abstractions;
using Rpssl.Application.Behaviors;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Tests.Behaviors;

public sealed record SampleRequest(int Value);

[TestClass]
public class ValidationBehaviorTests
{

    [TestMethod]
    public async Task NoValidators_CallsNext_AndReturnsResponse()
    {
        IEnumerable<IValidator<SampleRequest>> validators = System.Array.Empty<IValidator<SampleRequest>>();
        ValidationBehavior<SampleRequest, Result<string>> behavior = new(validators);

        int calls = 0;

        Result<string> response = await behavior.Handle(new SampleRequest(1), Next, CancellationToken.None);

        Assert.AreEqual(1, calls);
        Assert.IsTrue(response.IsSuccess);
        Assert.AreEqual("OK", response.Value);
        return;

        Task<Result<string>> Next(CancellationToken ct)
        {
            calls++;
            return Task.FromResult(Result.Success("OK"));
        }
    }

    [TestMethod]
    public async Task ValidRequest_WithValidators_PassesThrough()
    {
        Mock<IValidator<SampleRequest>> validator = new(MockBehavior.Strict);
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        IValidator<SampleRequest>[] validators = [validator.Object];
        ValidationBehavior<SampleRequest, Result<int>> behavior = new(validators);

        int calls = 0;

        Result<int> response = await behavior.Handle(new SampleRequest(5), Next, CancellationToken.None);

        Assert.AreEqual(1, calls);
        Assert.IsTrue(response.IsSuccess);
        Assert.AreEqual(42, response.Value);
        validator.VerifyAll();
        return;

        Task<Result<int>> Next(CancellationToken ct)
        {
            calls++;
            return Task.FromResult(Result.Success(42));
        }
    }

    [TestMethod]
    public async Task InvalidRequest_GenericResult_ReturnsFailureAndSkipsNext()
    {
        ValidationFailure failure = new("Value", "Value must be positive");
        Mock<IValidator<SampleRequest>> validator = new();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { failure }));

        IValidator<SampleRequest>[] validators = [validator.Object];
        ValidationBehavior<SampleRequest, Result<string>> behavior = new(validators);

        int calls = 0;

        Result<string> response = await behavior.Handle(new SampleRequest(-1), Next, CancellationToken.None);

        Assert.AreEqual(0, calls);
        Assert.IsTrue(response.IsFailure);
        Assert.AreEqual("Validation.Failed", response.Error.Code);
        Assert.IsTrue(response.Error.Description.Contains("Value must be positive"));
        return;

        Task<Result<string>> Next(CancellationToken ct)
        {
            calls++;
            return Task.FromResult(Result.Success("ShouldNotBeCalled"));
        }
    }

    [TestMethod]
    public async Task InvalidRequest_NonGenericResult_ReturnsFailure()
    {
        ValidationFailure failure = new("Id", "Id is required");
        Mock<IValidator<SampleRequest>> validator = new();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { failure }));

        IValidator<SampleRequest>[] validators = [validator.Object];
        ValidationBehavior<SampleRequest, Result> behavior = new(validators);

        int calls = 0;

        Result response = await behavior.Handle(new SampleRequest(0), Next, CancellationToken.None);

        Assert.AreEqual(0, calls);
        Assert.IsTrue(response.IsFailure);
        Assert.AreEqual("Validation.Failed", response.Error.Code);
        Assert.IsTrue(response.Error.Description.Contains("Id is required"));
        return;

        Task<Result> Next(CancellationToken ct)
        {
            calls++;
            return Task.FromResult(Result.Success());
        }
    }

    [TestMethod]
    public async Task InvalidRequest_NonResultResponse_ThrowsValidationException()
    {
        ValidationFailure failure = new("Field", "Invalid");
        Mock<IValidator<SampleRequest>> validator = new();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { failure }));

        IValidator<SampleRequest>[] validators = [validator.Object];
        ValidationBehavior<SampleRequest, string> behavior = new(validators);

        int calls = 0;
        RequestHandlerDelegate<string> next = ct =>
        {
            calls++;
            return Task.FromResult("OK");
        };

        await Assert.ThrowsExceptionAsync<ValidationException>(async () => await behavior.Handle(new SampleRequest(0), next, CancellationToken.None));
        Assert.AreEqual(0, calls);
    }

    [TestMethod]
    public async Task MultipleValidators_CombineMessages_InErrorDescription()
    {
        ValidationFailure f1 = new("A", "Error A");
        ValidationFailure f2 = new("B", "Error B");

        Mock<IValidator<SampleRequest>> v1 = new();
        v1.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { f1 }));
        Mock<IValidator<SampleRequest>> v2 = new();
        v2.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { f2 }));

        IValidator<SampleRequest>[] validators = [v1.Object, v2.Object];
        ValidationBehavior<SampleRequest, Result<int>> behavior = new(validators);

        int calls = 0;

        Result<int> response = await behavior.Handle(new SampleRequest(0), Next, CancellationToken.None);

        Assert.AreEqual(0, calls);
        Assert.IsTrue(response.IsFailure);
        Assert.IsTrue(response.Error.Description.Contains("Error A"));
        Assert.IsTrue(response.Error.Description.Contains("Error B"));
        Assert.IsTrue(response.Error.Description.Contains(";"));
        return;

        Task<Result<int>> Next(CancellationToken ct)
        {
            calls++;
            return Task.FromResult(Result.Success(1));
        }
    }
}
