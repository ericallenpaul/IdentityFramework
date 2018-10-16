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
        public string BaseUrl { get; set; }
        public string ApiVersion { get; set; }
        public string BackupDirectory { get; set; }
        public int KeepLogs { get; set; } = 60;
        public string RequestBackupFileName { get; set; } = "_Request";
        public string ResponseBackupFileName { get; set; } = "_Response";
    }
}
