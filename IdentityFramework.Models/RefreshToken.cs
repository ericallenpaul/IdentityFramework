using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityFramework.Models
{
    public class RefreshToken
    {
        public int RefreshTokenId { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
