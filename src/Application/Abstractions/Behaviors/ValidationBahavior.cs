namespace UsuariosAPI.Application.Abstractions.Behaviors
{
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Exceptions;
    using APICore.Responses;
    using FluentValidation;
    using MediatR;
    using UsuariosAPI.Application.Abstractions.Messaging;

    public class ValidationBahavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IBaseCommand
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;

        public ValidationBahavior(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!this.validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var validationErrors = this.validators
                .Select(validators => validators.Validate(context))
                .Where(validationResult => validationResult.Errors.Count != 0)
                .SelectMany(validationResult => validationResult.Errors)
                .Select(validationFailure =>
                {
                    var formattedPropertyName = FormatPropertyName(validationFailure.PropertyName);

                    // Reemplazar también en el mensaje de error el PropertyName original
                    var formattedMessage = string.IsNullOrWhiteSpace(validationFailure.ErrorMessage)
                        ? string.Empty
                        : ReplacePropertyNameInMessage(validationFailure.ErrorMessage, validationFailure.PropertyName, formattedPropertyName);

                    return string.IsNullOrWhiteSpace(formattedMessage) ?
                            Error.InvalidInput(formattedPropertyName) :
                            Error.InvalidInput(formattedPropertyName, formattedMessage);
                })
                .ToList();

            if (validationErrors.Count != 0)
            {
                throw new CustomValidationException(validationErrors);
            }

            return await next();
        }

        private static string ReplacePropertyNameInMessage(string errorMessage, string originalPropertyName, string formattedPropertyName)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                return errorMessage;
            }

            var result = Regex.Replace(errorMessage, @"\bdto\b", string.Empty, RegexOptions.IgnoreCase);

            // Agregar espacios después de comillas cuando están pegadas a palabras
            result = Regex.Replace(result, @"'([a-zA-Z])", "' $1");  // 'debe -> ' debe (comilla de apertura)
            result = Regex.Replace(result, @"([a-zA-Z])'([a-zA-Z])", "$1' $2");  // Id'debe -> Id' debe (comilla de cierre)
            result = Regex.Replace(result, @"'(\d)", "' $1");        // '0 -> ' 0

            // Limpiar espacios múltiples que puedan quedar después de remover "dto" y agregar espacios
            result = Regex.Replace(result, @"\s+", " ").Trim();

            // Limpiar espacios extra que quedaron después de las comillas
            result = Regex.Replace(result, @"'\s+", "'");   // '  texto -> 'texto
            result = Regex.Replace(result, @"\s+'\s*", " '"); // texto  '  -> texto '

            return result;
        }

        private static string FormatPropertyName(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return propertyName;
            }

            // Remover prefijo "dto." si existe (case insensitive)
            var formattedName = Regex.Replace(propertyName, @"^dto\.", string.Empty, RegexOptions.IgnoreCase);

            // Separar camelCase/PascalCase con espacios
            // Inserta un espacio antes de cada letra mayúscula que esté precedida por una minúscula o número
            formattedName = Regex.Replace(formattedName, @"(?<=[a-z0-9])(?=[A-Z])", " ");

            // Separar números de letras (ej: "Id2Name" -> "Id 2 Name")
            formattedName = Regex.Replace(formattedName, @"(?<=[A-Za-z])(?=\d)", " ");
            formattedName = Regex.Replace(formattedName, @"(?<=\d)(?=[A-Za-z])", " ");

            // Limpiar espacios múltiples y trim
            formattedName = Regex.Replace(formattedName, @"\s+", " ").Trim();

            return formattedName;
        }
    }
}
