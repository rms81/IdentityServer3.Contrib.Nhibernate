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


using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Contrib.Nhibernate.Entities;
using IdentityServer3.Core.Services;
using NHibernate;

namespace IdentityServer3.Contrib.Nhibernate.Stores
{
    public class ConsentStore : NhibernateStore, IConsentStore
    {
        public ConsentStore(ISession session)
            : base(session)
        {
        }

        public async Task<IdentityServer3.Core.Models.Consent> LoadAsync(string subject, string client)
        {
            var result = ExecuteInTransaction(session =>
              {
                  var item = session.QueryOver<Consent>()
                             .Where(c => c.Subject == subject && c.ClientId == client)
                             .SingleOrDefault();

                  return item == null
                      ? null
                      : new IdentityServer3.Core.Models.Consent
                      {
                          Subject = item.Subject,
                          ClientId = item.ClientId,
                          Scopes = ParseScopes(item.Scopes)
                      };
              });

            return await Task.FromResult(result);
        }

        public async Task UpdateAsync(IdentityServer3.Core.Models.Consent consent)
        {
            ExecuteInTransaction(session =>
            {
                var item = session.QueryOver<Consent>()
                        .Where(c => c.Subject == consent.Subject && c.ClientId == consent.ClientId)
                        .SingleOrDefault();

                if (item == null)
                {
                    if (consent.Scopes == null || !consent.Scopes.Any()) return;

                    item = new Consent
                    {
                        Subject = consent.Subject,
                        ClientId = consent.ClientId,
                        Scopes = StringifyScopes(consent.Scopes)
                    };

                    session.Save(item);
                }
                else
                {
                    if (consent.Scopes == null || !consent.Scopes.Any())
                    {
                        session.Delete(item);
                    }

                    item.Scopes = StringifyScopes(consent.Scopes);

                    session.SaveOrUpdate(item);
                }
            });

            await TaskExtensions.CompletedTask;
        }

        public async Task<IEnumerable<IdentityServer3.Core.Models.Consent>> LoadAllAsync(string subject)
        {
            var results = ExecuteInTransaction(session =>
              {
                  var items = session.QueryOver<Consent>()
                         .Where(c => c.Subject == subject)
                         .List();

                  return items.Select(i => new IdentityServer3.Core.Models.Consent
                  {
                      Subject = i.Subject,
                      ClientId = i.ClientId,
                      Scopes = ParseScopes(i.Scopes)
                  }).ToList();
              });

            return await Task.FromResult(results);
        }

        private IEnumerable<string> ParseScopes(string scopes)
        {
            return string.IsNullOrWhiteSpace(scopes) ? Enumerable.Empty<string>() : scopes.Split(',');
        }

        private string StringifyScopes(IEnumerable<string> scopes)
        {
            var enumerable = scopes as string[] ?? scopes.ToArray();
            if (scopes == null || !enumerable.Any())
            {
                return null;
            }

            var sb = new StringBuilder();
            foreach (var scope in enumerable)
            {
                sb.Append(scope);
                sb.Append(",");
            }

            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        public async Task RevokeAsync(string subject, string client)
        {
            ExecuteInTransaction(session =>
            {
                session.CreateQuery($"DELETE {nameof(Consent)} c " +
                                    $"WHERE c.{nameof(Consent.Subject)} = :subject " +
                                    $"and c.{nameof(Consent.ClientId)} = :clientId")
                    .SetParameter("subject", subject)
                    .SetParameter("clientId", client)
                    .ExecuteUpdate();
            });

            await TaskExtensions.CompletedTask;
        }
    }
}
