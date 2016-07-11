using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Nhibernate.Logging;
using IdentityServer3.Contrib.Nhibernate.Entities;
using NHibernate;

namespace IdentityServer3.Contrib.Nhibernate
{
    public class TokenCleanup
    {
        private readonly ISession _session;
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        CancellationTokenSource _source;
        readonly TimeSpan _interval;

        public TokenCleanup(ISession session, int interval = 60)
        {
            _session = session;
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (interval < 1) throw new ArgumentException("interval must be more than 1 second");

            _interval = TimeSpan.FromSeconds(interval);
        }

        public void Start()
        {
            if (_source != null) throw new InvalidOperationException("Already started. Call Stop first.");

            _source = new CancellationTokenSource();
            Task.Factory.StartNew(() => Start(_source.Token));
        }

        public void Stop()
        {
            if (_source == null) throw new InvalidOperationException("Not started. Call Start first.");

            _source.Cancel();
            _source = null;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Logger.Info("CancellationRequested");
                    break;
                }

                try
                {
                    await Task.Delay(_interval, cancellationToken);
                }
                catch
                {
                    Logger.Info("Task.Delay exception. exiting.");
                    break;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    Logger.Info("CancellationRequested");
                    break;
                }

                await ClearTokens();
            }
        }

        private async Task ClearTokens()
        {
            try
            {
                Logger.Info("Clearing tokens");

                _session.CreateQuery($"DELETE {nameof(Token)} t WHERE t.{nameof(Token.Expiry)} < :refDate")
                    .SetParameter("refDate", DateTimeOffset.UtcNow)
                    .ExecuteUpdate();

                await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Exception cleaning tokens", ex);
            }
        }
    }
}
