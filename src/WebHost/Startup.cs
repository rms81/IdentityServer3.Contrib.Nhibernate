using System;
using System.Threading.Tasks;
using IdentityServer3.Core.Configuration;
using Microsoft.Owin;
using Owin;
using Serilog;
using WebHost.Config;

[assembly: OwinStartup(typeof(WebHost.Startup))]

namespace WebHost
{
    internal class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Trace()
               .WriteTo.RollingFile(@"C:\Users\RicardoSantos\Repos\Projects\IdentityServer3.Contrib.Nhibernate\src\WebHost\log\Log-{Date}.log")
               .CreateLogger();

            appBuilder.Map("/core", core =>
            {
                var options = new IdentityServerOptions
                {
                    SiteName = "IdentityServer3 (Nhibernate)",
                    SigningCertificate = Certificate.Get(),
                    Factory = Factory.Configure("IdSvr3Config")
                };

                core.UseIdentityServer(options);
            });
        }
    }
}
