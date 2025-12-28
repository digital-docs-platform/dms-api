using Application.Exceptions;



namespace Application.PermissionHandling
{
    public class PermissionHandler : IPermissionHandler
    {
        private readonly IApplicationActor _actor;

        public PermissionHandler(IApplicationActor actor) 
        {
            _actor = actor;
        }
        public void EnsureAll(IEnumerable<string> permissions)
        {
            foreach(string permission in permissions)
            {
                if(!_actor.ActorPermissions.Contains(permission))
                    throw new ForbiddenException("You are not allowed to perform this actions");
            }
        }

        public void EnsureHas(string permission)
        {
            if (!_actor.ActorPermissions.Contains(permission))
            {
                throw new ForbiddenException("You are not allowed to perform this action");
            }
        }

        public bool Has(string permission) 
        {
            if(_actor.ActorPermissions.Contains(permission))
                return true;

            return false;
        }

        public bool HasAll(IEnumerable<string> permissions) 
        {
            foreach(var permission in permissions)
            {
                if(!_actor.ActorPermissions.Contains(permission))
                    return false;
            }

            return true;
        }
    }
}
