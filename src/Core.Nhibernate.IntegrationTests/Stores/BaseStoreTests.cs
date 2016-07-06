/*MIT License
*
*Copyright (c) 2016 Ricardo Santos
*
*Permission is hereby granted, free of charge, to any person obtaining a copy
*of this software and associated documentation files (the "Software"), to deal
*in the Software without restriction, including without limitation the rights
*to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*copies of the Software, and to permit persons to whom the Software is
*furnished to do so, subject to the following conditions:
*
*The above copyright notice and this permission notice shall be included in all
*copies or substantial portions of the Software.
*
*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
*SOFTWARE.
*/



using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using IdentityServer3.Contrib.Nhibernate.NhibernateConfig;
using IdentityServer3.Contrib.Nhibernate.Serialization;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Moq;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Configuration = NHibernate.Cfg.Configuration;

namespace Core.Nhibernate.IntegrationTests.Stores
{
    public abstract class BaseStoreTests
    {
        protected ISessionFactory NhSessionFactory;

        protected ISession NhibernateSession { get; }
        private readonly ISession _nhibernateAuxSession;

        protected readonly Mock<IScopeStore> ScopeStoreMock = new Mock<IScopeStore>();
        protected readonly Mock<IClientStore> ClientStoreMock = new Mock<IClientStore>();

        protected BaseStoreTests()
        {
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            NhSessionFactory = GetNHibernateSessionFactory();

            _nhibernateAuxSession = NhSessionFactory.OpenSession();
            NhibernateSession = NhSessionFactory.OpenSession();
        }

        private ISessionFactory GetNHibernateSessionFactory()
        {
            var connString = ConfigurationManager.ConnectionStrings["IdSvr3Config"];

            var sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString(connString.ToString())
                    .ShowSql()
                    .FormatSql()
                    .AdoNetBatchSize(20)
                )
                .Mappings(
                    m => m.AutoMappings.Add(MappingHelper.GetNhibernateServicesMappings(true, true))
                )
                .ExposeConfiguration(cfg =>
                {
                    SchemaMetadataUpdater.QuoteTableAndColumns(cfg);
                    //BuildSchema(cfg);
                })
                .BuildSessionFactory();

            return sessionFactory;

        }

        private void BuildSchema(Configuration cfg)
        {
            new SchemaUpdate(cfg).Execute(false, true);
        }

        protected void ExecuteInTransaction(Action<ISession> actionToExecute, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_nhibernateAuxSession.Transaction != null && _nhibernateAuxSession.Transaction.IsActive)
            {
                actionToExecute.Invoke(_nhibernateAuxSession);
            }
            else
            {
                using (var tx = _nhibernateAuxSession.BeginTransaction(isolationLevel))
                {
                    try
                    {
                        actionToExecute.Invoke(_nhibernateAuxSession);
                        tx.Commit();
                    }
                    catch (Exception)
                    {
                        tx.Rollback();
                        throw;
                    }
                }

            }
        }

        private JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new ClaimConverter());
            settings.Converters.Add(new ClaimsPrincipalConverter());
            settings.Converters.Add(new ClientConverter(ClientStoreMock.Object));
            settings.Converters.Add(new ScopeConverter(ScopeStoreMock.Object));
            return settings;
        }

        protected string ConvertToJson<T>(T value)
        {
            return JsonConvert.SerializeObject(value, GetJsonSerializerSettings());
        }

        protected T ConvertFromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, GetJsonSerializerSettings());
        }

        protected virtual void SetupScopeStoreMock()
        {
            ScopeStoreMock.Setup(st => st.FindScopesAsync(It.IsAny<IEnumerable<string>>()))
                .Returns((IEnumerable<string> scopeNames) =>
                {
                    return Task.FromResult(
                        scopeNames.Select(s => new Scope { Name = s, DisplayName = s }));
                });
        }
    }
}