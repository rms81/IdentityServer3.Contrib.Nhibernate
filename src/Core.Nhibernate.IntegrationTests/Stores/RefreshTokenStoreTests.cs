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
    public class RefreshTokenStoreTests : BaseStoreTests
    {
        public RefreshTokenStoreTests()
        {
        }

        [Fact]
        public async Task StoreAsync()
        {
            //Arrange
            var sut = new RefreshTokenStore(NhibernateSession, ScopeStoreMock.Object, ClientStoreMock.Object);

            var testKey = Guid.NewGuid().ToString();
            var testCode = ObjectCreator.GetRefreshToken();

            //Act
            await sut.StoreAsync(testKey, testCode);

            ExecuteInTransaction(session =>
            {
                //Assert
                var token = session.Query<Token>()
                    .SingleOrDefault(t => t.TokenType == TokenType.RefreshToken &&
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
            var sut = new RefreshTokenStore(NhibernateSession, ScopeStoreMock.Object, ClientStoreMock.Object);
            var testKey = Guid.NewGuid().ToString();
            var testCode = ObjectCreator.GetRefreshToken();

            var tokenHandle = new Token
            {
                Key = testKey,
                SubjectId = testCode.SubjectId,
                ClientId = testCode.ClientId,
                JsonCode = ConvertToJson(testCode),
                Expiry = testCode.CreationTime.UtcDateTime.AddSeconds(30),
                TokenType = TokenType.RefreshToken
            };

            SetupScopeStoreMock();

            ExecuteInTransaction(session =>
            {
                session.SaveOrUpdate(tokenHandle);
            });

            //Act
            var resultToken = await sut.GetAsync(testKey);

            //Assert
            Assert.NotNull(resultToken);

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
            var sut = new RefreshTokenStore(NhibernateSession, ScopeStoreMock.Object, ClientStoreMock.Object);
            var testKey = Guid.NewGuid().ToString();
            var testCode = ObjectCreator.GetRefreshToken();

            var tokenHandle = new Token
            {
                Key = testKey,
                SubjectId = testCode.SubjectId,
                ClientId = testCode.ClientId,
                JsonCode = ConvertToJson(testCode),
                Expiry = testCode.CreationTime.UtcDateTime.AddSeconds(testCode.LifeTime),
                TokenType = TokenType.RefreshToken
            };

            ExecuteInTransaction(session =>
            {
                session.SaveOrUpdate(tokenHandle);
            });

            //Act
            await sut.RemoveAsync(testKey);

            //Assert
            ExecuteInTransaction(session =>
            {
                var token = session.Query<Token>()
                    .SingleOrDefault(t => t.TokenType == TokenType.RefreshToken &&
                                          t.Key == testKey);

                Assert.Null(token);
            });
        }

        [Fact]
        public async Task GetAllAsync()
        {
            //Arrange
            var sut = new RefreshTokenStore(NhibernateSession, ScopeStoreMock.Object, ClientStoreMock.Object);
            var subjectId1 = Guid.NewGuid().ToString();
            var subjectId2 = Guid.NewGuid().ToString();

            var testKey1 = Guid.NewGuid().ToString();
            var testCode1 = ObjectCreator.GetRefreshToken(subjectId1);
            var tokenHandle1 = new Token
            {
                Key = testKey1,
                SubjectId = testCode1.SubjectId,
                ClientId = testCode1.ClientId,
                JsonCode = ConvertToJson(testCode1),
                Expiry = testCode1.CreationTime.UtcDateTime.AddSeconds(testCode1.LifeTime),
                TokenType = TokenType.RefreshToken
            };

            var testKey2 = Guid.NewGuid().ToString();
            var testCode2 = ObjectCreator.GetRefreshToken(subjectId1);
            var tokenHandle2 = new Token
            {
                Key = testKey2,
                SubjectId = testCode2.SubjectId,
                ClientId = testCode2.ClientId,
                JsonCode = ConvertToJson(testCode2),
                Expiry = testCode2.CreationTime.UtcDateTime.AddSeconds(testCode2.LifeTime),
                TokenType = TokenType.RefreshToken
            };

            var testKey3 = Guid.NewGuid().ToString();
            var testCode3 = ObjectCreator.GetRefreshToken(subjectId2);
            var tokenHandle3 = new Token
            {
                Key = testKey3,
                SubjectId = testCode3.SubjectId,
                ClientId = testCode3.ClientId,
                JsonCode = ConvertToJson(testCode3),
                Expiry = testCode3.CreationTime.UtcDateTime.AddSeconds(testCode3.LifeTime),
                TokenType = TokenType.RefreshToken
            };

            var testKey4 = Guid.NewGuid().ToString();
            var testCode4 = ObjectCreator.GetRefreshToken(subjectId2);
            var tokenHandle4 = new Token
            {
                Key = testKey4,
                SubjectId = testCode4.SubjectId,
                ClientId = testCode4.ClientId,
                JsonCode = ConvertToJson(testCode4),
                Expiry = testCode4.CreationTime.UtcDateTime.AddSeconds(testCode4.LifeTime),
                TokenType = TokenType.RefreshToken
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
            var sut = new RefreshTokenStore(NhibernateSession, ScopeStoreMock.Object, ClientStoreMock.Object);
            var subjectIdToRevoke = Guid.NewGuid().ToString();
            var clientIdToRevoke = Guid.NewGuid().ToString();

            var testKey = Guid.NewGuid().ToString();
            var testCode = ObjectCreator.GetRefreshToken();

            var tokenHandle = new Token
            {
                Key = testKey,
                SubjectId = testCode.SubjectId,
                ClientId = testCode.ClientId,
                JsonCode = ConvertToJson(testCode),
                Expiry = testCode.CreationTime.UtcDateTime.AddSeconds(testCode.LifeTime),
                TokenType = TokenType.RefreshToken
            };

            var testKeyToRevoke = Guid.NewGuid().ToString();
            var testCodeToRevoke = ObjectCreator.GetRefreshToken(subjectIdToRevoke, clientIdToRevoke);

            var tokenHandleToRevoke = new Token
            {
                Key = testKeyToRevoke,
                SubjectId = testCodeToRevoke.SubjectId,
                ClientId = testCodeToRevoke.ClientId,
                JsonCode = ConvertToJson(testCodeToRevoke),
                Expiry = testCodeToRevoke.CreationTime.UtcDateTime.AddSeconds(testCodeToRevoke.LifeTime),
                TokenType = TokenType.RefreshToken
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
                    .SingleOrDefault(t => t.TokenType == TokenType.RefreshToken &&
                                          t.Key == testKeyToRevoke);

                var tokenNotRevoked = session.Query<Token>()
                    .SingleOrDefault(t => t.TokenType == TokenType.RefreshToken &&
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