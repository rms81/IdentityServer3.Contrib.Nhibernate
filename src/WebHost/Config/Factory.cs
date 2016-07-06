using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Nhibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Data;
using IdentityServer3.Contrib.Nhibernate;
using IdentityServer3.Contrib.Nhibernate.NhibernateConfig;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Models;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Tool.hbm2ddl;
using Client = IdentityServer3.Core.Models.Client;
using Configuration = NHibernate.Cfg.Configuration;
using Scope = IdentityServer3.Core.Models.Scope;

namespace WebHost.Config
{
    class Factory
    {
        public static IdentityServerServiceFactory Configure(string connString)
        {
            var nhSessionFactory = GetNHibernateSessionFactory();
            var nhSession = nhSessionFactory.OpenSession();
            var tokenCleanUpSession = nhSessionFactory.OpenSession();

            var cleanup = new TokenCleanup(tokenCleanUpSession, 10);
            cleanup.Start();

            // these two calls just pre-populate the test DB from the in-memory config
            ConfigureClients(Clients.Get(), nhSession);
            ConfigureScopes(Scopes.Get(), nhSession);

            var factory = new IdentityServerServiceFactory();

            factory.RegisterNhibernateStores(new NhibernateServiceOptions(nhSessionFactory)
            {
                RegisterOperationalServices = true,
                RegisterConfigurationServices = true
            });

            factory.UseInMemoryUsers(Users.Get().ToList());

            return factory;
        }

        private static ISessionFactory GetNHibernateSessionFactory()
        {
            var connString = ConfigurationManager.ConnectionStrings["IdSvr3Config"];

            var sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString(connString.ToString())
                    .ShowSql()
                    .FormatSql())
                .Mappings(
                    m => m.AutoMappings.Add(MappingHelper.GetNhibernateServicesMappings(true, true)))
                .ExposeConfiguration(cfg =>
                {
                    SchemaMetadataUpdater.QuoteTableAndColumns(cfg);
                    BuildSchema(cfg);
                })
                .BuildSessionFactory();

            return sessionFactory;

        }

        private static void BuildSchema(Configuration cfg)
        {
            new SchemaUpdate(cfg).Execute(false, true);
        }

        public static void ConfigureClients(ICollection<Client> clients, ISession nhSession)
        {
            var clientsInDb = nhSession.Query<IdentityServer3.Contrib.Nhibernate.Entities.Client>();

            if (clientsInDb.Any()) return;

            var toSave = clients.Select(c => c.ToEntity()).ToList();

            foreach (var client in toSave)
            {
                var result = nhSession.Save(client);
            }
        }

        public static void ConfigureScopes(ICollection<Scope> scopes, ISession nhSession)
        {
            var scopesInDb = nhSession.Query<IdentityServer3.Contrib.Nhibernate.Entities.Scope>();

            if (scopesInDb.Any()) return;

            var toSave = scopes.Select(s => s.ToEntity()).ToList();

            foreach (var scope in toSave)
            {
                var result = nhSession.Save(scope);
            }
        }
    }
}
