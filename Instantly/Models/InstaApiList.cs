using InstagramApiSharp.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Instantly.Models
{
    public class InstaApiList:Dictionary<string,UserAccount>
    {
        public void AddAndStartObserveble(string instagram, UserAccount user)
        {
            base.Add(instagram, user);
            Task.Factory.StartNew(user.StartObservable);
        }

        public void StartObservable(string instagram)
        {
            this[instagram].IsActive = true;
        }

        public void StopObservable(string instagram)
        {
            this[instagram].IsActive = false;
        }
    }
}