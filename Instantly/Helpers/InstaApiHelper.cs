using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Instantly.Helpers
{
    public class InstaApiHelper
    {
        public static IInstaApi GetInstaApi(UserSessionData user,string filePath)
        {
            return InstaApiBuilder.CreateBuilder()
                    .SetUser(user)
                    .UseLogger(new DebugLogger(LogLevel.All))
                    .SetRequestDelay(RequestDelay.FromSeconds(0, 1))
                    // Session handler, set a file path to save/load your state/session data
                    .SetSessionHandler(new FileSessionHandler() { FilePath = filePath })
                    .Build();
        }

        public static void LoadSession(IInstaApi api)
        {
            api?.SessionHandler?.Load();
        }
        public static void SaveSession(IInstaApi api)
        {
            if (api == null)
                return;
            if (!api.IsUserAuthenticated)
                return;
            api.SessionHandler.Save();
        }
    }
}