using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using System.Web.Http.Cors;
using System.Security.Claims;
using Instantly.Context;
using Instantly.Infrastructure.Dl;
using Instantly.Interfaces;
using System.Data.SQLite;
using Ninject;

namespace Instantly
{
    public class Provider
    {
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
        {
            public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
            {
                context.Validated(); //   
            }

            public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
            {

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                
                var _kernel = NinjectConfig.CreateKernel();
                using (var db = _kernel.Get<IDatabaseFactory<UserDbContext>>().Get())
                {
                    if (db != null)
                    {
                        var users = db.Users.ToList();
                        if (users != null)
                        {
                            var user = users.FirstOrDefault(u => u.Email == context.UserName && u.Password == context.Password);
                            if (user!=null)
                            {
                                identity.AddClaim(new Claim("Age", "16"));

                                var props = new AuthenticationProperties(new Dictionary<string, string>
                            {
                                {
                                    "userdisplayname", context.UserName
                                },
                                {
                                     "role", "admin"
                                },
                                    {
                                        "UserId",user.Id.ToString()
                                    }
                             });

                                var ticket = new AuthenticationTicket(identity, props);
                                context.Validated(ticket);
                            }
                            else
                            {
                                context.SetError("invalid_grant", "Provided username and password is incorrect");
                                context.Rejected();
                            }
                        }
                    }
                    else
                    {
                        context.SetError("invalid_grant", "Provided username and password is incorrect");
                        context.Rejected();
                    }
                    return;
                }
            }
        }
    }
}