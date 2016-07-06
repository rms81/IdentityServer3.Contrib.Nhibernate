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
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Contrib.Nhibernate.Stores;
using NHibernate;
using NHibernate.Linq;
using Xunit;

namespace Core.Nhibernate.IntegrationTests.Stores
{
    public class ConsentStoreTests : BaseStoreTests
    {
        public ConsentStoreTests()
        {
        }

        [Fact]
        public async Task LoadAsync()
        {
            string subjectToGet = "subject1";
            string clientToGet = "client1";

            //Arrange
            var sut = new ConsentStore(NhibernateSession);
            var testConsent1 = ObjectCreator.GetConsent(clientToGet, subjectToGet);
            var testConsent2 = ObjectCreator.GetConsent();
            var testConsent3 = ObjectCreator.GetConsent();

            ExecuteInTransaction(session =>
            {
                session.Save(testConsent1);
                session.Save(testConsent2);
                session.Save(testConsent3);
            });

            //Act
            var result = await sut.LoadAsync(testConsent1.Subject, testConsent1.ClientId);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.ClientId.Equals(testConsent1.ClientId) &&
                        result.Subject.Equals(testConsent1.Subject));

            //CleanUp
            ExecuteInTransaction(session =>
            {
                session.Delete(testConsent1);
                session.Delete(testConsent2);
                session.Delete(testConsent3);
            });
        }

        [Fact]
        public async Task LoadAllAsync()
        {
            string subjectToGet = "subject1";

            //Arrange
            var sut = new ConsentStore(NhibernateSession);
            var testConsent1 = ObjectCreator.GetConsent(null, subjectToGet);
            var testConsent2 = ObjectCreator.GetConsent(null, subjectToGet);
            var testConsent3 = ObjectCreator.GetConsent();

            ExecuteInTransaction(session =>
            {
                session.Save(testConsent1);
                session.Save(testConsent2);
                session.Save(testConsent3);
            });

            //Act
            var result = (await sut.LoadAllAsync(subjectToGet))
                .ToList();

            //Assert
            Assert.NotNull(result);
            Assert.True(result.All(c => c.Subject.Equals(subjectToGet)));

            //CleanUp
            ExecuteInTransaction(session =>
            {
                session.Delete(testConsent1);
                session.Delete(testConsent2);
                session.Delete(testConsent3);
            });
        }

        [Fact]
        public async Task UpdateAsync_WithNewId()
        {
            var updatedClientId = "updatedClientId";

            //Arrange
            var sut = new ConsentStore(NhibernateSession);
            var testConsent1 = ObjectCreator.GetConsent();
            var testConsent2 = ObjectCreator.GetConsent();
            var testConsent3 = ObjectCreator.GetConsent();

            ExecuteInTransaction(session =>
            {
                session.Save(testConsent1);
                session.Save(testConsent2);
                session.Save(testConsent3);
            });

            var modelToUpdate = await sut.LoadAsync(testConsent1.Subject, testConsent1.ClientId);

            //Act
            modelToUpdate.ClientId = updatedClientId;
            await sut.UpdateAsync(modelToUpdate);

            ExecuteInTransaction(session =>
            {
                //Assert
                var updatedEntity = session.Query<IdentityServer3.Contrib.Nhibernate.Entities.Consent>()
                    .SingleOrDefault(c => c.ClientId == modelToUpdate.ClientId && c.Subject == modelToUpdate.Subject);

                Assert.NotNull(updatedEntity);

                //CleanUp
                session.Delete(testConsent1);
                session.Delete(testConsent2);
                session.Delete(testConsent3);
                session.Delete(updatedEntity);
            });
        }

        [Fact]
        public async Task UpdateAsync_WithUpdatedScopes()
        {
            //Arrange
            var sut = new ConsentStore(NhibernateSession);
            var testConsent1 = ObjectCreator.GetConsent();
            var testConsent2 = ObjectCreator.GetConsent();
            var testConsent3 = ObjectCreator.GetConsent();

            ExecuteInTransaction(session =>
            {
                session.Save(testConsent1);
                session.Save(testConsent2);
                session.Save(testConsent3);
            });

            var modelToUpdate = await sut.LoadAsync(testConsent1.Subject, testConsent1.ClientId);
            modelToUpdate.Scopes = ObjectCreator.GetScopes(5).Select(s => s.Name);

            //Act
            await sut.UpdateAsync(modelToUpdate);

            //Assert
            var updatedModel = await sut.LoadAsync(modelToUpdate.Subject, modelToUpdate.ClientId);

            Assert.NotNull(updatedModel);
            Assert.True(updatedModel.Scopes.Count() == 5);

            //CleanUp
            ExecuteInTransaction(session =>
            {
                session.Delete(testConsent1);
                session.Delete(testConsent2);
                session.Delete(testConsent3);
            });
        }

        [Fact]
        public async Task RevokeAsync()
        {
            //Arrange
            var sut = new ConsentStore(NhibernateSession);
            var testConsent1 = ObjectCreator.GetConsent();
            var testConsent2 = ObjectCreator.GetConsent();
            var testConsent3 = ObjectCreator.GetConsent();
            var consentToRevoke = ObjectCreator.GetConsent();

            ExecuteInTransaction(session =>
            {
                session.Save(testConsent1);
                session.Save(testConsent2);
                session.Save(testConsent3);
                session.Save(consentToRevoke);
            });

            //Act
            await sut.RevokeAsync(consentToRevoke.Subject, consentToRevoke.ClientId);

            //Assert
            var revokedConsent = await sut.LoadAsync(consentToRevoke.Subject, consentToRevoke.ClientId);

            Assert.Null(revokedConsent);

            //CleanUp
            ExecuteInTransaction(session =>
            {
                session.Delete(testConsent1);
                session.Delete(testConsent2);
                session.Delete(testConsent3);
            });
        }
    }
}