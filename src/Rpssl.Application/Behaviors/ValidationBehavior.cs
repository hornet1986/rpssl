using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = new List<ValidationFailure>();

        foreach (IValidator<TRequest> validator in validators)
        {
            ValidationResult? result = await validator.ValidateAsync(context, cancellationToken);
            if (!result.IsValid)
            {
                failures.AddRange(result.Errors);
            }
        }

        if (failures.Count == 0)
        {
            return await next(cancellationToken);
        }

        // Try to construct a Result<T> with validation error; if response is non-Result, throw.
        var error = Error.Validation("Validation.Failed", string.Join("; ", failures.Select(f => f.ErrorMessage)));

        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            // Pick the generic overload Result.Failure<T>(Error) explicitly to avoid ambiguity with Result.Failure(Error)
            MethodInfo genericFailure = typeof(Result)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m is { Name: nameof(Result.Failure), IsGenericMethodDefinition: true } && m.GetParameters().Length == 1);

            MethodInfo closed = genericFailure.MakeGenericMethod(typeof(TResponse).GetGenericArguments()[0]);
            return (TResponse)closed.Invoke(null, [error])!;
        }

        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        throw new ValidationException(failures);
    }
}
