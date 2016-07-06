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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using FluentNHibernate.Testing;
using IdentityServer3.Contrib.Nhibernate.Entities;
using IdentityServer3.Contrib.Nhibernate.NhibernateConfig;
using Moq;
using NHibernate;
using NHibernate.Mapping.ByCode.Conformist;
using Xunit;
using Xunit.Sdk;

namespace Core.Nhibernate.Tests
{
    public class AutoMappingTests
    {
        private readonly List<Type> _operationalServicesEntities = new List<Type>
            {
                typeof(Token),
                typeof(Consent),
            };

        private readonly List<Type> _configurationServicesEntities = new List<Type>
            {
                typeof(Client),
                typeof(ClientClaim),
                typeof(ClientCorsOrigin),
                typeof(ClientCustomGrantType),
                typeof(ClientIdPRestriction),
                typeof(ClientPostLogoutRedirectUri),
                typeof(ClientRedirectUri),
                typeof(ClientScope),
                typeof(ClientSecret),
                typeof(Scope),
                typeof(ScopeClaim),
                typeof(ScopeSecret)
            };

        [Fact]
        public void MappingOperationalServiceEntities()
        {
            var mappings = MappingHelper.GetNhibernateServicesMappings(true, false);
            var b = mappings.BuildMappings();

            foreach (var operationalServicesEntity in _operationalServicesEntities)
            {

                var map = mappings.FindMapping(operationalServicesEntity);

                Assert.NotNull(map);
            }
        }

        [Fact]
        public void MappingConfigurationServiceEntities()
        {
            var mappings = MappingHelper.GetNhibernateServicesMappings(false, true);
            var b = mappings.BuildMappings();

            foreach (var configurationServicesEntity in _configurationServicesEntities)
            {
                var map = mappings.FindMapping(configurationServicesEntity);

                Assert.NotNull(map);
            }
        }
    }
}
