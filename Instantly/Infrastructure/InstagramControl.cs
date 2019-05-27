using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Instantly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Instantly.Infrastructure
{
    public class InstagramControl
    {
        private InstagramControl(){}
        private static readonly InstagramControl instance = new InstagramControl();
        public static InstagramControl GetInstance()
        {
            return instance;
        }
        private InstaApiList apiList = new InstaApiList();

        public async Task AddAndStartObservable(string instagram,IInstaApi api)
        {
            if (apiList.Any(p => p.Key == instagram)) return;

            var currentUser = (await api.GetCurrentUserAsync()).Value;
            var followers = (await api.UserProcessor.GetCurrentUserFollowersAsync(PaginationParameters.MaxPagesToLoad(int.MaxValue))).Value.ToDictionary(p=>p.Pk);

            var user = new UserAccount
            {
                Api=api,
                CountSendMessage=0,
                CurrentUser=currentUser,
                Followers=followers,
                IsActive=true,
                TextMessage="Привет {UserName} спасибо что подписался"
            };

            apiList.AddAndStartObserveble(instagram, user);
        }

        public async Task AddAndStartObservable(string instagram,IInstaApi api, Account account)
        {
            if (apiList.Any(p => p.Key == instagram)) return;

            var currentUser = (await api.GetCurrentUserAsync()).Value;
            var followers = (await api.UserProcessor.GetCurrentUserFollowersAsync(PaginationParameters.MaxPagesToLoad(int.MaxValue))).Value.ToDictionary(p => p.Pk);

            var user = new UserAccount
            {
                Api = api,
                CountSendMessage = account.CountSendMessage,
                CurrentUser = currentUser,
                Followers = followers,
                IsActive = account.IsActive,
                TextMessage = account.TextMessage
            };
            apiList.AddAndStartObserveble(instagram, user);
        }
        public void StartObservable(string instagram)
        {
            if (!apiList.Any(p => p.Key == instagram)) return;

            apiList[instagram].IsActive = true; 
        }

        public void StopObservable(string instagram)
        {
            if (!apiList.Any(p => p.Key == instagram)) return;

            apiList[instagram].IsActive = false;
        }

        public List<InstaCurrentUser> GetListAccounts(List<string> listLogins)
        {
            var result = new List<InstaCurrentUser>();

            foreach(var item in listLogins)
            {
                result.Add(apiList[item].CurrentUser);
            }
            return result;
        }

        public void UpdateText(Account account)
        {
            apiList[account.Login].TextMessage = account.TextMessage;
            
        }

        public void DeleteAccount(Account account)
        {
            apiList[account.Login].Delete();
            apiList.Remove(account.Login);
        }
    }
}