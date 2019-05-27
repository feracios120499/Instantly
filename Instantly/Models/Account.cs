using Instantly.ParametersModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Instantly.Models
{
    public class Account
    {
        public Account()
        {

        }
        public Account(InstagramAccount account)
        {
            this.Login = account.Login;
            this.Password = account.Login;
            this.UserId = account.UserId;
        }
        [Key]
        public string Login { get; set; }
        public string Password { get; set; }

        public string TextMessage { get; set; }
        public long CountSendMessage { get; set; }
        public bool IsActive { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
    }

}