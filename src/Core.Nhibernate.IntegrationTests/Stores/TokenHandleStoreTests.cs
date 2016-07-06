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
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Contrib.Nhibernate.Entities;
using IdentityServer3.Contrib.Nhibernate.Enums;
using IdentityServer3.Contrib.Nhibernate.Stores;
using NHibernate;
using NHibernate.Linq;
using Xunit;

namespace Core.Nhibernate.IntegrationTests.Stores
{
    public class TokenHandleStoreTests : BaseStoreTests
    {
        public TokenHandleStoreTests()
        {
        }

        [Fact]
        public async Task StoreAsync()
        {
            //Arrange
            var sut = new TokenHandleStore(NhibernateSession, ScopeStoreMock.Object, ClientStoreMock.Object);

            var testKey = Guid.NewGuid().ToString();
            var testCode = ObjectCreator.GetTokenHandle();

            //Act
            await sut.StoreAsync(testKey, testCode);

            ExecuteInTransaction(session =>
            {
                //Assert
                var token = session.Query<Token>()
                    .SingleOrDefault(t => t.TokenType == TokenType.TokenHandle &&
                                          t.Key == testKey);

                Assert.NotNull(token);

                //CleanUp
                session.Delete(token);
            });
        }

        #region BaseTokenStore

        [Fact]
        public async Task GetAsync()
        {
            //Arrange
            var sut = new TokenHandleStore(NhibernateSession, ScopeStoreMock.Object, ClientStoreMock.Object);
            var testKey = Guid.NewGuid().ToString();
            var testCode = ObjectCreator.GetTokenHandle();

            var tokenHandle = new Token
            {
                Key = testKey,
                SubjectId = testCode.SubjectId,
                ClientId = testCode.ClientId,
                JsonCode = ConvertToJson(testCode),
                Expiry = DateTime.UtcNow.AddSeconds(testCode.Client.AuthorizationCodeLifetime),
                TokenType = TokenType.TokenHandle
            };

            SetupScopeStoreMock();

            ExecuteInTransaction(session =>
            {
                session.Save(tokenHandle);
            });

            //Act
            var token = await sut.GetAsync(testKey);

            //Assert
            Assert.NotNull(token);

            //CleanUp
            ExecuteInTransaction(session =>
            {
                session.Delete(tokenHandle);
            });

        }

        [Fact]
        public async Task RemoveAsync()
        {
            //Arrange
            var sut = new TokenHandleStore(NhibernateSession, ScopeStoreMock.Object, ClientStoreMock.Object);
            var testKey = Guid.NewGuid().ToString();
            var testCode = ObjectCreator.GetTokenHandle();

            var tokenHandle = new Token
            {
                Key = testKey,
                SubjectId = testCode.SubjectId,
                ClientId = testCode.ClientId,
                JsonCode = ConvertToJson(testCode),
                Expiry = DateTime.UtcNow.AddSeconds(testCode.Client.AuthorizationCodeLifetime),
                TokenType = TokenType.TokenHandle
            };

            ExecuteInTransaction(session =>
            {
                session.Save(tokenHandle);

            });

            //Act
            await sut.RemoveAsync(testKey);

            ExecuteInTransaction(session =>
            {
                //Assert
                var token = session.Query<Token>()
                    .SingleOrDefault(t => t.TokenType == TokenType.TokenHandle &&
                                          t.Key == testKey);

                Assert.Null(token);
            });
        }

        [Fact]
        public async Task GetAllAsync()
        {
            //Arrange
            var sut = new TokenHandleStore(NhibernateSession, ScopeStoreMock.Object, ClientStoreMock.Object);
            var subjectId1 = Guid.NewGuid().ToString();
            var subjectId2 = Guid.NewGuid().ToString();

            var testKey1 = Guid.NewGuid().ToString();
            var testCode1 = ObjectCreator.GetTokenHandle(subjectId1);
            var tokenHandle1 = new Token
            {
                Key = testKey1,
                SubjectId = testCode1.SubjectId,
                ClientId = testCode1.ClientId,
                JsonCode = ConvertToJson(testCode1),
                Expiry = DateTime.UtcNow.AddSeconds(testCode1.Client.AuthorizationCodeLifetime),
                TokenType = TokenType.TokenHandle
            };

            var testKey2 = Guid.NewGuid().ToString();
            var testCode2 = ObjectCreator.GetTokenHandle(subjectId1);
            var tokenHandle2 = new Token
            {
                Key = testKey2,
                SubjectId = testCode2.SubjectId,
                ClientId = testCode2.ClientId,
                JsonCode = ConvertToJson(testCode2),
                Expiry = DateTime.UtcNow.AddSeconds(testCode2.Client.AuthorizationCodeLifetime),
                TokenType = TokenType.TokenHandle
            };

            var testKey3 = Guid.NewGuid().ToString();
            var testCode3 = ObjectCreator.GetTokenHandle(subjectId2);
            var tokenHandle3 = new Token
            {
                Key = testKey3,
                SubjectId = testCode3.SubjectId,
                ClientId = testCode3.ClientId,
                JsonCode = ConvertToJson(testCode3),
                Expiry = DateTime.UtcNow.AddSeconds(testCode3.Client.AuthorizationCodeLifetime),
                TokenType = TokenType.TokenHandle
            };

            var testKey4 = Guid.NewGuid().ToString();
            var testCode4 = ObjectCreator.GetTokenHandle(subjectId2);
            var tokenHandle4 = new Token
            {
                Key = testKey4,
                SubjectId = testCode4.SubjectId,
                ClientId = testCode4.ClientId,
                JsonCode = ConvertToJson(testCode4),
                Expiry = DateTime.UtcNow.AddSeconds(testCode4.Client.AuthorizationCodeLifetime),
                TokenType = TokenType.TokenHandle
            };

            SetupScopeStoreMock();

            ExecuteInTransaction(session =>
            {
                session.SaveOrUpdate(tokenHandle1);
                session.SaveOrUpdate(tokenHandle2);
                session.SaveOrUpdate(tokenHandle3);
                session.SaveOrUpdate(tokenHandle4);
            });

            //Act
            var tokens = (await sut.GetAllAsync(subjectId1)).ToList();

            //Assert
            Assert.True(tokens.Count == 2);
            Assert.True(tokens.All(t => t.SubjectId == subjectId1));

            //CleanUp
            ExecuteInTransaction(session =>
            {
                session.Delete(tokenHandle1);
                session.Delete(tokenHandle2);
                session.Delete(tokenHandle3);
                session.Delete(tokenHandle4);
            });

        }

        [Fact]
        public async Task RevokeAsync()
        {
            //Arrange
            var sut = new TokenHandleStore(NhibernateSession, ScopeStoreMock.Object, ClientStoreMock.Object);
            var subjectIdToRevoke = Guid.NewGuid().ToString();
            var clientIdToRevoke = Guid.NewGuid().ToString();

            var testKey = Guid.NewGuid().ToString();
            var testCode = ObjectCreator.GetTokenHandle();

            var tokenHandle = new Token
            {
                Key = testKey,
                SubjectId = testCode.SubjectId,
                ClientId = testCode.ClientId,
                JsonCode = ConvertToJson(testCode),
                Expiry = DateTime.UtcNow.AddSeconds(testCode.Client.AuthorizationCodeLifetime),
                TokenType = TokenType.TokenHandle
            };

            var testKeyToRevoke = Guid.NewGuid().ToString();
            var testCodeToRevoke = ObjectCreator.GetTokenHandle(subjectIdToRevoke, clientIdToRevoke);

            var tokenHandleToRevoke = new Token
            {
                Key = testKeyToRevoke,
                SubjectId = testCodeToRevoke.SubjectId,
                ClientId = testCodeToRevoke.ClientId,
                JsonCode = ConvertToJson(testCodeToRevoke),
                Expiry = DateTime.UtcNow.AddSeconds(testCodeToRevoke.Client.AuthorizationCodeLifetime),
                TokenType = TokenType.TokenHandle
            };

            ExecuteInTransaction(session =>
            {
                session.Save(tokenHandle);
                session.Save(tokenHandleToRevoke);
            });

            //Act
            await sut.RevokeAsync(subjectIdToRevoke, clientIdToRevoke);

            ExecuteInTransaction(session =>
            {
                //Assert
                var tokenRevoked = session.Query<Token>()
                    .SingleOrDefault(t => t.TokenType == TokenType.TokenHandle &&
                                          t.Key == testKeyToRevoke);

                var tokenNotRevoked = session.Query<Token>()
                    .SingleOrDefault(t => t.TokenType == TokenType.TokenHandle &&
                                          t.Key == testKey);

                Assert.Null(tokenRevoked);
                Assert.NotNull(tokenNotRevoked);

                //CleanUp
                session.Delete(tokenNotRevoked);
            });
        }

        #endregion
    }
}