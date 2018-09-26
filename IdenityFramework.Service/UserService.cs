using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using IdentityFramework.Models;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;

namespace IdentityFramework.Service
{
    public class UserService
    {
        private readonly ApplicationDbContext _Context;
        private readonly NLog.ILogger _Logger;

        public UserService(IApplicationDbContext Context, NLog.ILogger Logger = null)
        {
            _Context = Context;
            if (Logger != null)
            {
                ServiceUtility._Logger = Logger;
            }
        }

        public User Validate(string loginTypeCode, string identifier, string secret)
        {
            CredentialType credentialType = ApplicationDbContext.CredentialTypes.FirstOrDefault(ct => string.Equals(ct.Code, loginTypeCode, StringComparison.OrdinalIgnoreCase));

            if (credentialType == null)
                return null;

            Credential credential = this.storage.Credentials.FirstOrDefault(
                c => c.CredentialTypeId == credentialType.Id && string.Equals(c.Identifier, identifier, StringComparison.OrdinalIgnoreCase) && c.Secret == MD5Hasher.ComputeHash(secret)
            );

            if (credential == null)
                return null;

            return this.storage.Users.Find(credential.UserId);
        }

        public async void SignIn(HttpContext httpContext, User user, bool isPersistent = false)
        {
            ClaimsIdentity identity = new ClaimsIdentity(this.GetUserClaims(user), CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await httpContext.Authentication.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties() { IsPersistent = isPersistent }
            );
        }

        private IEnumerable<Claim> GetUserClaims(User user)
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.AddRange(this.GetUserRoleClaims(user));
            return claims;
        }
    }
}
