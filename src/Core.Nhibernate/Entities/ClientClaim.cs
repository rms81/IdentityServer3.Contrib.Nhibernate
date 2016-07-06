using System;

namespace IdentityServer3.Contrib.Nhibernate.Entities
{
    public class ClientClaim : BaseEntity<Guid>
    {
        public virtual string Type { get; set; }

        public virtual string Value { get; set; }

        public virtual Client Client { get; set; }
    }
}
