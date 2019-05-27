using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Instantly.Models
{
    public class Logger
    {

        public static void Error(string message)
        {
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"App_Data\" + "error.txt", message);
        }
    }
}