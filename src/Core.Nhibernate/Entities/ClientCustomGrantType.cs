using System;

namespace IdentityServer3.Contrib.Nhibernate.Entities
{
    public class ClientCustomGrantType : BaseEntity<Guid>
    {
        public virtual string GrantType { get; set; }

        public virtual Client Client { get; set; }
    }
}
