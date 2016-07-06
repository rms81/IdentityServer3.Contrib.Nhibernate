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
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Contrib.Nhibernate.Entities;
using IdentityServer3.Core.Services;
using NHibernate;
using NHibernate.Linq;

namespace IdentityServer3.Contrib.Nhibernate.Services
{
    public class ClientConfigurationCorsPolicyService : ICorsPolicyService
    {
        private readonly ISessionFactory _nhSessionFactory;

        public ClientConfigurationCorsPolicyService(ISessionFactory nhSessionFactory)
        {
            if (nhSessionFactory == null) throw new ArgumentNullException(nameof(nhSessionFactory));

            _nhSessionFactory = nhSessionFactory;
        }

        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            return await Task.Run(() =>
            {
                using (var ctx = _nhSessionFactory.OpenSession())
                {
                    var query =
                        from client in ctx.Query<Client>()
                        from allowed in client.AllowedCorsOrigins
                        select allowed.Origin;

                    var urls = query.ToList();

                    var origins = urls.Select(x => x.GetOrigin()).Where(x => x != null).Distinct();

                    var result = origins.Contains(origin, StringComparer.OrdinalIgnoreCase);

                    return result;
                }
            });
        }
    }
}
