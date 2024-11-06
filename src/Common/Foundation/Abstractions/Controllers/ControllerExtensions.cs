using FluentValidation;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;

namespace Foundation.Abstractions.Controllers
{
    public static class ControllerExtensions
    {
        public static IActionResult ToOk<TResult>(this Result<TResult> result)
        {
            return result.Match<IActionResult>(response =>
            {
                return new OkObjectResult(response);
            }, exception =>
            {
                if (exception is Exception appException)
                {
                    return new BadRequestObjectResult(appException.Message);
                }

                return new StatusCodeResult(500);
            });
        }
    }
}
