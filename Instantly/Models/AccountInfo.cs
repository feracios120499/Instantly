using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Instantly.Models
{
    public class AccountInfo : InstaCurrentUser
    {
        public AccountInfo(InstaUserShort instaUserShort) : base(instaUserShort)
        {
        }
        public AccountInfo(InstaCurrentUser instaCurrentUser) : base(instaCurrentUser)
        {

        }
        public InstaCurrentUser UserInfo { get; set; }
        public long CountFollowers { get; set; }
        public long CountFollowing { get; set; }
    }
}