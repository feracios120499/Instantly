using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using static Instantly.Provider;
using System.Web.Http;
using Microsoft.Owin.Builder;
using System.Web.Http.Dispatcher;
using Instantly.Infrastructure.Handlers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Instantly.Infrastructure.Dl;
using Instantly.Interfaces;
using Instantly.Context;
using Ninject;
using Instantly.Infrastructure;
using System.Web.Hosting;
using Instantly.Helpers;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;


[assembly: OwinStartup(typeof(Instantly.Startup))]
namespace Instantly
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOption { get; set; }
        public async Task ConfigureAuth(IAppBuilder app)
        {

            var OAuthOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(365),
                Provider = new SimpleAuthorizationServerProvider(),
                AccessTokenFormat= new TicketDataFormat(app.CreateDataProtector(
               typeof(OAuthAuthorizationServerMiddleware).Namespace,
               "Access_Token", "v1"))
            };
            OAuthOption = OAuthOptions;
            
            

            app.UseOAuthBearerTokens(OAuthOptions);
            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            HttpConfiguration config = new HttpConfiguration();
            
            //config.MessageHandlers.Add(new ResponseWrappingHandler());
            WebApiConfig.Register(config);
            var _kernel = NinjectConfig.CreateKernel();
            var instaControl = InstagramControl.GetInstance();
            using (var db = _kernel.Get<IDatabaseFactory<UserDbContext>>().Get())
            {
                foreach (var account in db.Accounts)
                {
                    var path = AppDomain.CurrentDomain.BaseDirectory+@"App_Data\" + account.Login + ".bin";
                    var api = InstaApiHelper.GetInstaApi(new InstagramApiSharp.Classes.UserSessionData { UserName = account.Login, Password = account.Password }, path);
                    InstaApiHelper.LoadSession(api);
                    await instaControl.AddAndStartObservable(account.Login, api, account);
                }
            }
        }

        public async void Configuration(IAppBuilder app)
        {
            await ConfigureAuth(app);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.EnsureInitialized();
        }

        
    }
}