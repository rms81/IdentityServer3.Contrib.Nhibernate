﻿/*MIT License
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
using System.Threading.Tasks;
using IdentityServer3.Contrib.Nhibernate.Enums;
using IdentityServer3.Core.Services;
using NHibernate;
using Token = IdentityServer3.Core.Models.Token;

namespace IdentityServer3.Contrib.Nhibernate.Stores
{
    public class TokenHandleStore : BaseTokenStore<Token>, ITokenHandleStore
    {
        public TokenHandleStore(ISession session, IScopeStore scopeStore, IClientStore clientStore)
            : base(session, TokenType.TokenHandle, scopeStore, clientStore)
        {

        }

        public override async Task StoreAsync(string key, Token value)
        {
            ExecuteInTransaction(session =>
            {
                var nhCode = new IdentityServer3.Contrib.Nhibernate.Entities.Token
                {
                    Key = key,
                    SubjectId = value.SubjectId,
                    ClientId = value.ClientId,
                    JsonCode = ConvertToJson(value),
                    Expiry = DateTime.UtcNow.AddSeconds(value.Client.AuthorizationCodeLifetime),
                    TokenType = this.TokenType
                };

                session.Save(nhCode);
            });

            await TaskExtensions.CompletedTask;
        }
    }
}