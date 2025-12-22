using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logging
{
    public class AppError
    {
        public Exception Exception { get; set; }
        public string Email { get; set; }
        public Guid ErrorId { get; set; }
    }
}
