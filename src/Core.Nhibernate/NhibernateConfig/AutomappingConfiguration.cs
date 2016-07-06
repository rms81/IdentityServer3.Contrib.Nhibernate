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
using FluentNHibernate.Automapping;
using IdentityServer3.Contrib.Nhibernate.Entities;

namespace IdentityServer3.Contrib.Nhibernate.NhibernateConfig
{
    public class AutomappingConfiguration : DefaultAutomappingConfiguration
    {
        private readonly bool _registerOperationalServices;
        private readonly bool _registerConfigurationServices;

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

        public AutomappingConfiguration(bool registerOperationalServices, bool registerConfigurationServices)
        {
            _registerOperationalServices = registerOperationalServices;
            _registerConfigurationServices = registerConfigurationServices;
        }

        public override bool ShouldMap(Type type)
        {
            var result = type.Namespace != null && type.Namespace.Equals("Core.Nhibernate.Entities")
                         && (_registerOperationalServices && _operationalServicesEntities.Contains(type)
                             || _registerConfigurationServices && _configurationServicesEntities.Contains(type));

            return result;
        }
    }
}
