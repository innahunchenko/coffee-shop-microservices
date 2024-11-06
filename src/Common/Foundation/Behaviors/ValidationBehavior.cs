using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Any())
        {
            var errors = new Dictionary<string, string[]>();
            var contractResolver = new CamelCasePropertyNamesContractResolver();

            foreach (var validationFailure in failures)
            {
                var propertyName = validationFailure.PropertyName;
                if (propertyName.Contains('.'))
                {
                    propertyName = propertyName.Substring(propertyName.LastIndexOf('.') + 1);
                }

                propertyName = contractResolver.GetResolvedPropertyName(propertyName);

                if (errors.ContainsKey(propertyName))
                {
                    errors[propertyName] = errors[propertyName]
                        .Concat(new[] { validationFailure.ErrorMessage }).ToArray();
                }
                else
                {
                    errors.Add(propertyName, new[] { validationFailure.ErrorMessage });
                }
            }

            return (TResponse)Results.ValidationProblem(errors, statusCode: StatusCodes.Status400BadRequest);
        }

        return await next();
    }
}
