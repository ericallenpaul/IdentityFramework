using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AutoMapper;
using IdentityFramework.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NLog;

namespace IdentityFramework.Service
{
    public interface IUserService
    {
        Task<IdentityResult> Register(string Email, string Password);

        Task<string> Login(string Email, string Password, bool RememberMe);
    }

    public class UserService : IUserService
    {
        //private readonly IMapper _Mapper;
        private readonly ApplicationDbContext _Context;
        private readonly IdentityFrameworkSettings _Settings;
        private readonly IMemoryCache _Cache;
        private ILogger<UserService> _Logger;
        private readonly UserManager<IdentityUser> _UserManager;
        private readonly SignInManager<IdentityUser> _SignInManager;
        private readonly IEmailService _emailService;
        private readonly IIdentityFramework_JWT _TokenOptions;


        private ModelStateDictionary _modelState;

        public UserService(
            ApplicationDbContext Context,
            //IMapper Mapper,
            IOptions<IdentityFrameworkSettings> Settings,
            IMemoryCache MemoryCache,
            UserManager<IdentityUser> UserManager,
            SignInManager<IdentityUser> SignInManager,
            ILogger<UserService> Logger,
            IOptions<IdentityFramework_JWT> TokenOptions,
            IEmailService emailService
        )
        {
            _Context = Context;
            //_Mapper = Mapper;
            _Settings = Settings.Value;
            _Cache = MemoryCache ?? new MemoryCache(new MemoryCacheOptions());
            _Logger = Logger;
            _UserManager = UserManager;
            _SignInManager = SignInManager;
            _emailService = emailService;
            this._TokenOptions = TokenOptions.Value;
        }

        protected bool ValidateUser(string Email, string Password)
        {
            if (Email.Trim().Length == 0)
                throw new Exception("Email is required.");
            if (Password.Trim().Length == 0)
                throw new Exception("Password is required.");
            return true;
        }

        private string GenerateToken(string username)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_TokenOptions.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _TokenOptions.Issuer,
                audience: _TokenOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> Login(string Email, string Password, bool RememberMe)
        {
            string returnvalue = null;
            if (ValidateUser(Email, Password))
            {
                var user = _UserManager.FindByEmailAsync(Email);
                if (user != null && !user.Result.EmailConfirmed)
                {
                    _Logger.LogInformation("Email is not confirmed");
                    return returnvalue;
                }

                var result = await _SignInManager.PasswordSignInAsync(Email, Password, RememberMe, lockoutOnFailure: _Settings.LockoutOnFailure);
                if (result.Succeeded)
                {
                    _Logger.LogInformation("User logged in.");
                    return GenerateToken(Email);
                }
                if (result.RequiresTwoFactor)
                {
                    _Logger.LogInformation("User requires 2FA.");
                }
                if (result.IsLockedOut)
                {
                    _Logger.LogWarning("User account locked out.");
                }
                else
                {
                    string errorMessage = $"Login failed. Please check your username and password and try again.";
                    throw new Exception(errorMessage);
                }
            }
            
            return returnvalue;
        }

        public async Task<IdentityResult> Register(string Email, string Password)
        {
            IdentityResult returnvalue = null;

            if (ValidateUser(Email, Password))
            {
                var user = new IdentityUser { UserName = Email, Email = Email };
                returnvalue = await _UserManager.CreateAsync(user, Password);
                if (returnvalue.Succeeded)
                {
                    _Logger.LogInformation("User created a new account with password.");

                    var code = await _UserManager.GenerateEmailConfirmationTokenAsync(user);

                    //build the url
                    var callbackUrl = $"{_Settings.ConfirmEmailUrl}?userId={user.Id}&code={code}";

                    await _emailService.SendEmailAsync(Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                }
                else
                {
                    string errorMessage = $"Registration Error: {System.Environment.NewLine}";

                    foreach (var error in returnvalue.Errors)
                    {
                        errorMessage += error.Description + System.Environment.NewLine;
                    }

                    throw new Exception(errorMessage);
                }

                return returnvalue;
            }
            else
            {
                throw new Exception("Error: Model state is invalid, username and password are required.");
            }
        }

        // Initialize some test roles. In the real world, these would be setup explicitly by a role manager
        private string[] roles = new[] { "User", "Manager", "Administrator" };
        private async Task InitializeRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var newRole = new IdentityRole(role);
                    await roleManager.CreateAsync(newRole);
                    // In the real world, there might be claims associated with roles
                    // _roleManager.AddClaimAsync(newRole, new )
                }
            }
        }




    }
}
