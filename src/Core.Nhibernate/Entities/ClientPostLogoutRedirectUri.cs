using System;

namespace IdentityServer3.Contrib.Nhibernate.Entities
{
    public class ClientPostLogoutRedirectUri : BaseEntity<Guid>
    {
        public virtual string Uri { get; set; }

        public virtual Client Client { get; set; }
    }
}
