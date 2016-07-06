using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System.Configuration;
using Core.Nhibernate;
using IdentityServer3.Contrib.Nhibernate;
using IdentityServer3.Contrib.Nhibernate.Services;
using IdentityServer3.Contrib.Nhibernate.Stores;

namespace IdentityServer3.Core.Configuration
{
    public static class IdentityServerServiceFactoryExtensions
    {

        public static void RegisterNhibernateStores(this IdentityServerServiceFactory factory,
            NhibernateServiceOptions serviceOptions)
        {
            if (serviceOptions.RegisterOperationalServices || serviceOptions.RegisterConfigurationServices)
            {
                RegisterSessionFactory(factory, serviceOptions);
            }

            if (serviceOptions.RegisterOperationalServices)
            {
                RegisterOperationalServices(factory, serviceOptions);
            }

            if (serviceOptions.RegisterConfigurationServices)
            {
                RegisterConfigurationServices(factory, serviceOptions);
            }
        }
        private static void RegisterOperationalServices(IdentityServerServiceFactory factory, NhibernateServiceOptions serviceOptions)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (serviceOptions == null) throw new ArgumentNullException(nameof(serviceOptions));

            factory.AuthorizationCodeStore = new Registration<IAuthorizationCodeStore, AuthorizationCodeStore>();
            factory.TokenHandleStore = new Registration<ITokenHandleStore, TokenHandleStore>();
            factory.ConsentStore = new Registration<IConsentStore, ConsentStore>();
            factory.RefreshTokenStore = new Registration<IRefreshTokenStore, RefreshTokenStore>();
        }

        private static void RegisterConfigurationServices(IdentityServerServiceFactory factory, NhibernateServiceOptions serviceOptions)
        {
            RegisterClientStore(factory, serviceOptions);
            RegisterScopeStore(factory, serviceOptions);
        }

        private static void RegisterClientStore(IdentityServerServiceFactory factory, NhibernateServiceOptions serviceOptions)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (serviceOptions == null) throw new ArgumentNullException(nameof(serviceOptions));

            RegisterSessionFactory(factory, serviceOptions);

            factory.ClientStore = new Registration<IClientStore, ClientStore>();
            factory.CorsPolicyService = new Registration<ICorsPolicyService, ClientConfigurationCorsPolicyService>();

        }

        private static void RegisterScopeStore(IdentityServerServiceFactory factory, NhibernateServiceOptions serviceOptions)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (serviceOptions == null) throw new ArgumentNullException(nameof(serviceOptions));

            RegisterSessionFactory(factory, serviceOptions);

            factory.ScopeStore = new Registration<IScopeStore, ScopeStore>();
        }

        private static void RegisterSessionFactory(IdentityServerServiceFactory factory, NhibernateServiceOptions serviceOptions)
        {
            if (factory.Registrations.All(r => r.DependencyType != typeof(ISessionFactory)))
            {
                factory.Register(
                    new Registration<ISessionFactory>(serviceOptions.NhibernateSessionFactory));
                factory.Register(new Registration<ISession>(c => c.Resolve<ISessionFactory>().OpenSession())
                {
                    Mode = RegistrationMode.InstancePerHttpRequest
                });
            }
        }
    }
}
