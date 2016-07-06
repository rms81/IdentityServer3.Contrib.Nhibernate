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
using System.Security.Claims;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Consent = IdentityServer3.Contrib.Nhibernate.Entities.Consent;

namespace Core.Nhibernate.IntegrationTests
{
    public class ObjectCreator
    {
        private static readonly IFixture AFixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        public static AuthorizationCode GetAuthorizationCode(string subjectId = null, string clientId = null)
        {
            var codeBuilder = AFixture.Build<AuthorizationCode>()
                .Without(ac => ac.Client)
                .Without(ac => ac.Subject)
                .Without(ac => ac.CodeChallengeMethod);

            var code = codeBuilder.Create();

            code.Client = GetClient(clientId);
            code.Subject = GetSubject(subjectId);
            code.CodeChallengeMethod = Constants.CodeChallengeMethods.Plain;

            return code;
        }

        public static Token GetTokenHandle(string subjectId = null, string clientId = null)
        {
            var tokenBuilder = AFixture.Build<Token>()
                .Without(t => t.Client)
                .With(t => t.Claims, new List<Claim>()
                {
                    new Claim(Constants.ClaimTypes.Subject, subjectId ?? Guid.NewGuid().ToString())
                });

            var token = tokenBuilder.Create();

            token.Client = GetClient(clientId);

            return token;
        }

        public static RefreshToken GetRefreshToken(string subjectId = null, string clientId = null)
        {
            var tokenBuilder = AFixture.Build<RefreshToken>()
                .Without(rt => rt.Subject)
                .With(rt => rt.CreationTime, DateTimeOffset.UtcNow)
                .With(rt => rt.AccessToken, GetAccessToken(subjectId, clientId));

            var token = tokenBuilder.Create();

            token.Subject = GetSubject(subjectId);

            return token;
        }

        private static Token GetAccessToken(string subjectId, string clientId)
        {
            var tokenBuilder = AFixture.Build<Token>()
               .Without(t => t.Client)
               .With(t => t.Claims, new List<Claim>()
               {
                    new Claim(Constants.ClaimTypes.Subject, subjectId ?? Guid.NewGuid().ToString())
               });

            var token = tokenBuilder.Create();

            token.Client = GetClient(clientId);

            return token;
        }

        private static ClaimsPrincipal GetSubject(string subjectId = null)
        {
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(Constants.ClaimTypes.Subject, subjectId ?? Guid.NewGuid().ToString()));

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return claimsPrincipal;
        }

        public static Client GetClient(string clientId = null)
        {
            var clientBuiler = AFixture.Build<Client>()
                .Without(c => c.Claims)
                .With(c => c.ClientId, clientId ?? Guid.NewGuid().ToString());

            var client = clientBuiler.Create();

            client.Claims = new List<Claim>(GetClaims(3));

            return client;
        }

        public static IEnumerable<Claim> GetClaims(int nClaimsToGet)
        {
            var claims = new List<Claim>();

            for (var i = 0; i < nClaimsToGet; i++)
            {
                claims.Add(GetClaim());
            }

            return claims;
        }

        public static Claim GetClaim()
        {
            var claim = new Claim(AFixture.Create<string>(), AFixture.Create<string>());

            return claim;
        }


        public static IEnumerable<Scope> GetScopes(int nScopesToGet)
        {
            var scopes = AFixture.CreateMany<Scope>(nScopesToGet);

            return scopes;
        }

        public static Scope GetScope()
        {
            var scope = AFixture.Create<Scope>();

            return scope;
        }

        public static Consent GetConsent(string clientId = null, string subject = null)
        {
            var consent = AFixture.Create<Consent>();

            if (clientId != null)
                consent.ClientId = clientId;

            if (subject != null)
                consent.Subject = subject;

            return consent;
        }
    }
}