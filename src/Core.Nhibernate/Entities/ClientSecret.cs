using System;

namespace IdentityServer3.Contrib.Nhibernate.Entities
{
    public class ClientSecret : BaseEntity<Guid>
    {
        public virtual string Value { get; set; }

        public virtual string Type { get; set; }

        public virtual string Description { get; set; }

        public virtual DateTimeOffset? Expiration { get; set; }

        public virtual Client Client { get; set; }
    }
}
