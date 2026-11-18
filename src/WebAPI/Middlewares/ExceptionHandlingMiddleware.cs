namespace UsuariosAPI.WebAPI.Middlewares
{
    using System;
    using APICore.Exceptions;
    using APICore.Responses;
    using ElmahCore;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;

        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (CustomValidationException customException)
            {
                context.Response.StatusCode =
                    customException.Errors!.Any(x => x.code == APICore.Responses.Error.AuthorizationError(string.Empty).code)
                    ? StatusCodes.Status401Unauthorized
                    : StatusCodes.Status400BadRequest;

                var response = Response.Failure(customException.Errors!.ToList()!);

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Ocurrió una excepción: {ex.Message}");

                var exceptionDetails = GetExceptionDetails(ex);

                var problemDetails = new ProblemDetails
                {
                    Status = exceptionDetails.status,
                    Type = exceptionDetails.type,
                    Title = exceptionDetails.title,
                    Detail = exceptionDetails.detail,
                };

                if (exceptionDetails.errors is not null)
                {
                    problemDetails.Extensions["errors"] = exceptionDetails.errors;
                }

                context.Response.StatusCode = exceptionDetails.status;

                // Elmah config
                if (exceptionDetails.status == StatusCodes.Status500InternalServerError)
                {
                    await context.RaiseError(ex);
                }

                await context.Response.WriteAsJsonAsync(Response.Failure(APICore.Responses.Error.GenericError(problemDetails.Detail)));
            }
        }

        private static ExceptionDetails GetExceptionDetails(Exception exception)
        {
            return exception switch
            {
                CustomValidationException validationException => new ExceptionDetails(
                    StatusCodes.Status400BadRequest,
                    "ValidationFailure",
                    "Validación de Error",
                    "Han ocurrido uno o mas errores de validacion",
                    validationException.Errors),
                _ => new ExceptionDetails(
                    StatusCodes.Status500InternalServerError,
                    "ServerError",
                    "Error de Servidor",
                    "Un inesperado error ocurrió en la App",
                    null)
            };
        }

        internal record ExceptionDetails(
            int status,
            string type,
            string title,
            string detail,
            IEnumerable<object>? errors);
    }
}
