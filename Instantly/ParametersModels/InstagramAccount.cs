using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Instantly.ParametersModels
{
    public class InstagramAccount
    {
        [Required(ErrorMessage = "Login is required")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "UserId is required")]
        [Range(1,long.MaxValue, ErrorMessage = "UserId invalid")]
        public long UserId { get; set; }
    }
}