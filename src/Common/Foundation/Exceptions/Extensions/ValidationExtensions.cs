using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Foundation.Exceptions.Extensions
{
    using Newtonsoft.Json.Serialization;

    public static class ValidationExtensions
    {
        public static ValidationProblemDetails ToProblemDetails(this ValidationException ex)
        {
            var error = new ValidationProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Status = 400
            };

            var contractResolver = new CamelCasePropertyNamesContractResolver();

            foreach (var validationFailure in ex.Errors)
            {
                var propertyName = validationFailure.PropertyName;
                if (propertyName.Contains('.'))
                {
                    propertyName = propertyName.Substring(propertyName.LastIndexOf('.') + 1);
                }

                propertyName = contractResolver.GetResolvedPropertyName(propertyName);

                if (error.Errors.ContainsKey(propertyName))
                {
                    error.Errors[propertyName] = error.Errors[propertyName]
                        .Concat(new[] { validationFailure.ErrorMessage }).ToArray();
                }
                else
                {
                    error.Errors.Add(propertyName, new[] { validationFailure.ErrorMessage });
                }
            }

            return error;
        }
    }
}
