using System;

namespace IdentityServer3.Contrib.Nhibernate.Entities
{
    public class Consent : BaseEntity<Guid>
    {
        public virtual string Subject { get; set; }

        public virtual string ClientId { get; set; }

        public virtual string Scopes { get; set; }
    }
}
