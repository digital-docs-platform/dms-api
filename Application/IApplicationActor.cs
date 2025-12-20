using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IApplicationActor
    {
        public int Id { get; }
        public string Email { get; }
        public bool IsAdmin { get; }
        public IReadOnlyCollection<string> ActorPermissions { get; }
    }
}
