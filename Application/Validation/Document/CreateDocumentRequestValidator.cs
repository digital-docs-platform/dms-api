using Application.UseCases.Commands.Requests.Document;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Document
{
    public class CreateDocumentRequestValidator  : AbstractValidator<CreateDocumentRequest>
    {
        public CreateDocumentRequestValidator() 
        {
            
        }
    }
}
