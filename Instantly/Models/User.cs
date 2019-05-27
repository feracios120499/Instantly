using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Instantly.Models
{
    public class User
    {
        [Key]
        public long Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool? IsFree { get; set; }
        public DateTime? DateEndSub { get; set; }


        public virtual ICollection<Account> Accounts { get; set; }
    }
}