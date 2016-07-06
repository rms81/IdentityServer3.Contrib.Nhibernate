using System;
using IdentityServer3.Contrib.Nhibernate.Enums;

namespace IdentityServer3.Contrib.Nhibernate.Entities
{
    public class Token : BaseEntity<Guid>
    {
        public virtual string Key { get; set; }

        public virtual DateTimeOffset Expiry { get; set; }

        public virtual string JsonCode { get; set; }

        public virtual TokenType TokenType { get; set; }

        public virtual string SubjectId { get; set; }

        public virtual string ClientId { get; set; }

    }
}
