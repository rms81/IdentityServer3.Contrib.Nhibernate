namespace IdentityServer3.Contrib.Nhibernate.Entities
{
    public interface IBaseEntity
    {
    }

    public interface IBaseEntity<TKey> : IBaseEntity
    {
        TKey Id { get; set; }
    }
}