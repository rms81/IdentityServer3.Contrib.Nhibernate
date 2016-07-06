using NHibernate;

namespace IdentityServer3.Contrib.Nhibernate
{
    public class NhibernateServiceOptions
    {
        public ISessionFactory NhibernateSessionFactory { get; set; }
        public bool RegisterOperationalServices { get; set; }
        public bool RegisterConfigurationServices { get; set; }

        public NhibernateServiceOptions(ISessionFactory nhSessionFactory)
        {
            NhibernateSessionFactory = nhSessionFactory;
        }

    }
}
