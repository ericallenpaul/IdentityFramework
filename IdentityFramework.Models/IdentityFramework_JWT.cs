using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityFramework.Models
{
    public interface IIdentityFramework_JWT
    {
        string SecretKey { get; set; }
        string Issuer { get; set; }
        string Audience { get; set; }
        string Username { get; set; }
    }

    public class IdentityFramework_JWT : IIdentityFramework_JWT
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Username { get; set; }
    }
}
