namespace IdentityServer3.Contrib.Nhibernate.Entities
{
    public abstract class BaseEntity : IBaseEntity
    {

    }

    public abstract class BaseEntity<TKey> : BaseEntity, IBaseEntity<TKey>
    {
        public virtual TKey Id { get; set; }
    }
}
