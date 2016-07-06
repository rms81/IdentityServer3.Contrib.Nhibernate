using NHibernate;

namespace Core.Nhibernate.Extensions
{
    public class NhHelper
    {
        public static ISessionFactory GetSessionFactory()
        {
            var configuration = Fluently.Configure()
                .Database(/*.....*/)
                .ExposeConfiguration(cfg => {
                                                cfg.AddDeserializedMapping(MappingHelper.GetIdentityMappings(myEntities), null);
                });

            var factory = configuration.BuildSessionFactory();
        }
    }
}