using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityFramework.Models
{
    public class IdentityFrameworkSettings
    {
        public string SmtpServer { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string ConfirmEmailUrl { get; set; }
        public bool LockoutOnFailure { get; set; }

    }
}
