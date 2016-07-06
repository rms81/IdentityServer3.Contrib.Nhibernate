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
using NHibernate;

namespace IdentityServer3.Contrib.Nhibernate.Stores
{
    public abstract class NhibernateStore
    {
        private readonly ISession _nhSession;

        protected NhibernateStore(ISession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            _nhSession = session;
        }

        protected void ExecuteInTransaction(Action<ISession> actionToExecute, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_nhSession.Transaction != null && _nhSession.Transaction.IsActive)
            {
                actionToExecute.Invoke(_nhSession);
            }
            else
            {
                using (var tx = _nhSession.BeginTransaction(isolationLevel))
                {
                    try
                    {
                        actionToExecute.Invoke(_nhSession);
                        tx.Commit();
                    }
                    catch (Exception)
                    {
                        tx.Rollback();
                        throw;
                    }
                }

            }
        }

        protected T ExecuteInTransaction<T>(Func<ISession, T> actionToExecute, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_nhSession.Transaction != null && _nhSession.Transaction.IsActive)
            {
                var result = actionToExecute.Invoke(_nhSession);
                return result;
            }
            else
            {
                using (var tx = _nhSession.BeginTransaction(isolationLevel))
                {
                    try
                    {
                        var result = actionToExecute.Invoke(_nhSession);
                        tx.Commit();
                        return result;
                    }
                    catch (Exception)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

    }
}
