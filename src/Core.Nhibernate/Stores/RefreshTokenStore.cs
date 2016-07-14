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


using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Contrib.Nhibernate.Enums;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using NHibernate;
using NHibernate.Linq;
using Token = IdentityServer3.Contrib.Nhibernate.Entities.Token;

namespace IdentityServer3.Contrib.Nhibernate.Stores
{
    public class RefreshTokenStore : BaseTokenStore<RefreshToken>, IRefreshTokenStore
    {
        public RefreshTokenStore(ISession session, IScopeStore scopeStore, IClientStore clientStore)
            : base(session, TokenType.RefreshToken, scopeStore, clientStore)
        {

        }

        public override async Task StoreAsync(string key, RefreshToken value)
        {
            ExecuteInTransaction(session =>
            {
                var token = session
                       .Query<Token>()
                       .SingleOrDefault(t => t.Key == key && t.TokenType == TokenType);

                if (token == null)
                {
                    token = new Token
                    {
                        Key = key,
                        SubjectId = value.SubjectId,
                        ClientId = value.ClientId,
                        TokenType = TokenType
                    };

                    session.Save(token);
                }

                token.JsonCode = ConvertToJson(value);
                token.Expiry = value.CreationTime.UtcDateTime.AddSeconds(value.LifeTime);

                session.Update(token);
            });

            await TaskExtensions.CompletedTask;
        }
    }
}
