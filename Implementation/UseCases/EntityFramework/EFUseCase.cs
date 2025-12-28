using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCases.EntityFramework
{
    public abstract class EFUseCase
    {
        protected readonly DatabaseContext _context;
        protected EFUseCase(DatabaseContext context) 
        {
            _context = context;
        }
    }
}
