using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Instantly.Helpers
{
    public static class TokenHelper
    {
        private const string TokenType = "Bearer";
        private static string ParseToken(string token)
        {
            int index = token.IndexOf(TokenType);
            if (index >= 0)
            {
                token = token.Substring(TokenType.Length + 1);
            }
            return token;
        }
        public static string GetEmailFromToken(string token)
        {
            token = ParseToken(token);
            Microsoft.Owin.Security.AuthenticationTicket ticket = Startup.OAuthOption.AccessTokenFormat.Unprotect(token);
            if (ticket == null)
                throw new Exception("Bad Token");
            var email = ticket.Properties.Dictionary["userdisplayname"];
            return email;
        }

        public static long GetUserIdFromToken(string token)
        {
            token = ParseToken(token);
            Microsoft.Owin.Security.AuthenticationTicket ticket = Startup.OAuthOption.AccessTokenFormat.Unprotect(token);
            if (ticket == null)
                throw new Exception("Bad Token");
            var id = Convert.ToInt64(ticket.Properties.Dictionary["UserId"]);
            return id;
        }
    }
}