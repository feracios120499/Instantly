using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Instantly.Models
{
    public class TwoFactor
    {
        public string Login { get; set; }
        public string Code { get; set; }
    }
}