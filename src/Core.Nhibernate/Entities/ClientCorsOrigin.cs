using System;

namespace IdentityServer3.Contrib.Nhibernate.Entities
{
    public class ClientCorsOrigin : BaseEntity<Guid>
    {
        public virtual string Origin { get; set; }

        public virtual Client Client { get; set; }
    }
}
