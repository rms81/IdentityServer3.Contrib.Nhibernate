using System;

namespace IdentityServer3.Contrib.Nhibernate.Entities
{
    public class ClientIdPRestriction : BaseEntity<Guid>
    {
        public virtual string Provider { get; set; }

        public virtual Client Client { get; set; }
    }
}
