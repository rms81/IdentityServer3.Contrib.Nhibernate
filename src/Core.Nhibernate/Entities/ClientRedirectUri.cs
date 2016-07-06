using System;

namespace IdentityServer3.Contrib.Nhibernate.Entities
{
    public class ClientRedirectUri : BaseEntity<Guid>
    {
        public virtual string Uri { get; set; }

        public virtual Client Client { get; set; }
    }

}
