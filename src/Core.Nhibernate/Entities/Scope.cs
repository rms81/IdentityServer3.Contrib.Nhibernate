using System;
using System.Collections.Generic;

namespace IdentityServer3.Contrib.Nhibernate.Entities
{
    public class Scope : BaseEntity<Guid>
    {
        public virtual bool Enabled { get; set; }

        public virtual string Name { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual string Description { get; set; }

        public virtual bool Required { get; set; }

        public virtual bool Emphasize { get; set; }

        public virtual int Type { get; set; }

        public virtual ISet<ScopeClaim> ScopeClaims { get; set; } = new HashSet<ScopeClaim>();

        public virtual bool IncludeAllClaimsForUser { get; set; }

        public virtual ISet<ScopeSecret> ScopeSecrets { get; set; } = new HashSet<ScopeSecret>();

        public virtual string ClaimsRule { get; set; }

        public virtual bool ShowInDiscoveryDocument { get; set; }

        public virtual bool AllowUnrestrictedIntrospection { get; set; }

    }
}
