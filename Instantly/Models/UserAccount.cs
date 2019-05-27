using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Instantly.Models
{
    public class UserAccount
    {

        public IInstaApi Api { get; set; }
        public Dictionary<long, InstaUserShort> Followers { get; set; }
        public InstaCurrentUser CurrentUser { get; set; }
        public bool IsActive { get; set; } = true;
        public string TextMessage { get; set; }
        public long CountSendMessage { get; set; }
        private bool IsWork { get; set; } = true;
        public async Task StartObservable()
        {
            while (true)
            {
                while (IsActive)
                {
                    try
                    {
                        if (!IsWork) return;
                        var newSubscribeQuery = (await Api.UserProcessor.GetCurrentUserFollowersAsync(PaginationParameters.MaxPagesToLoad(int.MaxValue)));
                        if (newSubscribeQuery.Succeeded)
                        {
                            var newSubscribe = newSubscribeQuery.Value.ToDictionary(p => p.Pk);
                            var exceptKeys = newSubscribe.Keys.Except(Followers.Keys);
                            foreach (var item in exceptKeys)
                            {
                                if (exceptKeys.Count() >= 2)
                                {
                                    Thread.Sleep(20000);
                                }
                                await SendMessage(newSubscribe[item]);
                            }
                            Followers = newSubscribe;
                            Thread.Sleep(2000);
                        }
                        else
                        {
                            Logger.Error(JsonConvert.SerializeObject(newSubscribeQuery.Info));
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message + ex.StackTrace);
                    }

                }
                Thread.Sleep(5000);
            }
        }
        public void StopObserveble()
        {
            IsActive = false;
        }
        public void Delete()
        {
            IsWork = false;
        }
        private async Task SendMessage(InstaUserShort UserFollow)
        {
            try
            { 
                var text = ParseText(this.TextMessage, UserFollow);

                var message = (await this.Api.MessagingProcessor.SendDirectTextAsync(UserFollow.Pk.ToString(), null, text));

                if (message.Succeeded)
                {
                    this.CountSendMessage++;
                }
                else
                {
                    Logger.Error(JsonConvert.SerializeObject(message.Info));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + ex.StackTrace);
            }
        }
        private string ParseText(string oldText, InstaUserShort user)
        {
            return oldText.Replace("{UserName}", user.UserName);
        }
    }
}