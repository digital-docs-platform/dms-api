using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Exceptions
{
    public class RequestDataValidationException : Exception
    {
        public IReadOnlyList<ValidationError> Errors { get; }

        public RequestDataValidationException()
            : this(Enumerable.Empty<ValidationError>())
        {
        }

        public RequestDataValidationException(IEnumerable<ValidationError> errors)
            : base("Request data validation failed.")
        {
            Errors = (errors ?? Enumerable.Empty<ValidationError>())
                .ToList()
                .AsReadOnly();
        }

        public RequestDataValidationException(string property, string message)
            : this(new[] { new ValidationError(property, message) })
        {
        }
    }

    public sealed class ValidationError
    {
        public ValidationError(string property, string message)
        {
            Property = property;
            Message = message;
        }

        public string Property { get; }
        public string Message { get; }
    }
}
