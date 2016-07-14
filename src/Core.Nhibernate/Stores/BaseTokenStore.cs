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
using System.Threading.Tasks;
using IdentityServer3.Contrib.Nhibernate.Enums;
using IdentityServer3.Contrib.Nhibernate.Serialization;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Linq;
using Token = IdentityServer3.Contrib.Nhibernate.Entities.Token;

namespace IdentityServer3.Contrib.Nhibernate.Stores
{
    public abstract class BaseTokenStore<T> : NhibernateStore where T : class
    {
        protected readonly TokenType TokenType;
        protected readonly IScopeStore ScopeStore;
        protected readonly IClientStore ClientStore;

        protected BaseTokenStore(ISession session, TokenType tokenType, IScopeStore scopeStore, IClientStore clientStore)
            : base(session)
        {
            if (scopeStore == null) throw new ArgumentNullException(nameof(scopeStore));
            if (clientStore == null) throw new ArgumentNullException(nameof(clientStore));

            TokenType = tokenType;
            ScopeStore = scopeStore;
            ClientStore = clientStore;
        }

        JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new ClaimConverter());
            settings.Converters.Add(new ClaimsPrincipalConverter());
            settings.Converters.Add(new ClientConverter(ClientStore));
            settings.Converters.Add(new ScopeConverter(ScopeStore));
            return settings;
        }

        protected string ConvertToJson(T value)
        {
            return JsonConvert.SerializeObject(value, GetJsonSerializerSettings());
        }

        protected T ConvertFromJson(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, GetJsonSerializerSettings());
        }

        public async Task<T> GetAsync(string key)
        {
            var toReturn = ExecuteInTransaction(session =>
            {
                var token = session
                    .Query<Token>()
                    .SingleOrDefault(t => t.Key == key && t.TokenType == TokenType);

                return (token == null || token.Expiry < DateTimeOffset.UtcNow)
                    ? null
                    : ConvertFromJson(token.JsonCode);
            });
            return await Task.FromResult(toReturn);
        }

        public async Task RemoveAsync(string key)
        {
            ExecuteInTransaction(session =>
            {
                session.CreateQuery($"DELETE {nameof(Token)} t " +
                                    $"WHERE t.{nameof(Token.Key)} = :key " +
                                    $"and t.{nameof(Token.TokenType)} = :tokenType")
                    .SetParameter("key", key)
                    .SetParameter("tokenType", TokenType)
                    .ExecuteUpdate();
            });

            await TaskExtensions.CompletedTask;
        }

        public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subjectId)
        {
            var toReturn = ExecuteInTransaction(session =>
              {
                  var tokens = session.Query<Token>()
                      .Where(t => t.SubjectId == subjectId && t.TokenType == TokenType)
                      .ToList();

                  if (!tokens.Any()) return new List<ITokenMetadata>();

                  var results = tokens.Select(x => ConvertFromJson(x.JsonCode)).ToArray();
                  return results.Cast<ITokenMetadata>();
              });

            return await Task.FromResult(toReturn);
        }

        public async Task RevokeAsync(string subjectId, string clientId)
        {
            ExecuteInTransaction(session =>
            {
                session.CreateQuery($"DELETE {nameof(Token)} t " +
                                    $"WHERE t.{nameof(Token.SubjectId)} = :subject " +
                                    $"and t.{nameof(Token.ClientId)} = :clientId " +
                                    $"and t.{nameof(Token.TokenType)} = :tokenType")
                    .SetParameter("subject", subjectId)
                    .SetParameter("clientId", clientId)
                    .SetParameter("tokenType", TokenType)
                    .ExecuteUpdate();
            });

            await TaskExtensions.CompletedTask;
        }

        public abstract Task StoreAsync(string key, T value);
    }
}
