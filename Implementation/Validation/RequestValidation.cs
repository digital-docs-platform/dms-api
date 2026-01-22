using Application.Exceptions;
using Application.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.Validation
{
    public class RequestValidation : IRequestValidation
    {
        private readonly IServiceProvider _sp;
        public RequestValidation(IServiceProvider sp)
        {
            _sp = sp;
        }
        public async Task ValidateAsync<T>(T request, CancellationToken ct)
        {
            var validator = _sp.GetService<IValidator<T>>();
            if (validator is null)
                return;

            var result = await validator.ValidateAsync(request, ct);
            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
                throw new RequestDataValidationException(errors);
            }
        }
    }
}
