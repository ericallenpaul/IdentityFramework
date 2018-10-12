using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AutoMapper;
using IdentityFramework.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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

        Task<SignInResult> Login(string Email, string Password, bool RememberMe);
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


        private ModelStateDictionary _modelState;

        public UserService(
            ApplicationDbContext Context,
            //IMapper Mapper,
            IOptions<IdentityFrameworkSettings> Settings,
            IMemoryCache MemoryCache,
            UserManager<IdentityUser> UserManager,
            SignInManager<IdentityUser> SignInManager,
            ILogger<UserService> Logger,
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
        }

        protected bool ValidateUser(string Email, string Password)
        {
            if (Email.Trim().Length == 0)
                throw new Exception("Email is required.");
            if (Password.Trim().Length == 0)
                throw new Exception("Password is required.");
            return true;
        }

        public async Task<SignInResult> Login(string Email, string Password, bool RememberMe)
        {
            SignInResult returnvalue = null;
            if (ValidateUser(Email, Password))
            {
                var user = _UserManager.FindByEmailAsync(Email);
                if (user != null && !user.Result.EmailConfirmed)
                {
                    _Logger.LogInformation("Email is not confirmed");
                    return returnvalue;
                }

                returnvalue = await _SignInManager.PasswordSignInAsync(Email, Password, RememberMe, lockoutOnFailure: _Settings.LockoutOnFailure);
                if (returnvalue.Succeeded)
                {
                    _Logger.LogInformation("User logged in.");
                    return returnvalue;
                }
                if (returnvalue.RequiresTwoFactor)
                {
                    _Logger.LogInformation("User requires 2FA.");
                }
                if (returnvalue.IsLockedOut)
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

    }
}
