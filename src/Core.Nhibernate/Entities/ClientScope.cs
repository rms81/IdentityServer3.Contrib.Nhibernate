using System;

namespace IdentityServer3.Contrib.Nhibernate.Entities
{
    public class ClientScope : BaseEntity<Guid>
    {
        public virtual string Scope { get; set; }

        public virtual Client Client { get; set; }
    }
}
