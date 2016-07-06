using System;

namespace IdentityServer3.Contrib.Nhibernate.Entities
{
    public class ScopeSecret : BaseEntity<int>
    {
        public virtual string Description { get; set; }

        public virtual DateTimeOffset? Expiration { get; set; }

        public virtual string Type { get; set; }

        public virtual string Value { get; set; }

        public virtual Scope Scope { get; set; }
    }
}
