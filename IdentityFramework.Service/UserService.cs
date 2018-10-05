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
using Microsoft.Extensions.Options;
using NLog;

namespace IdentityFramework.Service
{
    public class UserService
    {
        private readonly IMapper _Mapper;
        private readonly ApplicationDbContext _Context;
        private readonly IdentityFrameworkSettings _Settings;
        private readonly IMemoryCache _Cache;
        private NLog.ILogger _Logger;
        private readonly UserManager<IdentityUser> _UserManager;
        private readonly IEmailSender _EmailSender;
        private readonly SignInManager<IdentityUser> _SignInManager;


        private ModelStateDictionary _modelState;

        public UserService(
            ApplicationDbContext Context,
            IMapper Mapper,
            IOptions<IdentityFrameworkSettings> Settings,
            IMemoryCache MemoryCache,
            UserManager<IdentityUser> UserManager,
            SignInManager<IdentityUser> SignInManager,
            NLog.ILogger Logger = null
        )
        {
            _Context = Context;
            _Mapper = Mapper;
            _Settings = Settings.Value;
            _Cache = MemoryCache ?? new MemoryCache(new MemoryCacheOptions());
            _Logger = Logger;
            _UserManager = UserManager;
            _SignInManager = SignInManager;
        }

        protected bool ValidateUser(string Email, string Password)
        {
            if (Email.Trim().Length == 0)
                _modelState.AddModelError("Email", "Email is required.");
            if (Password.Trim().Length == 0)
                _modelState.AddModelError("Password", "Password is required.");
            return _modelState.IsValid;
        }

        public async Task<IdentityResult> Register(string Email, string Password)
        {
            IdentityResult returnvalue = null;

            if (!ValidateUser(Email, Password))
            {
                var user = new IdentityUser { UserName = Email, Email = Email };
                returnvalue = await _UserManager.CreateAsync(user, Password);
                if (returnvalue.Succeeded)
                {
                    _Logger.Info("User created a new account with password.");

                    var code = await _UserManager.GenerateEmailConfirmationTokenAsync(user);

                    //build the url
                    var callbackUrl = "/Account/ConfirmEmail";

                    await _EmailSender.SendEmailAsync(Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _SignInManager.SignInAsync(user, isPersistent: false);
                }

                return returnvalue;
            }
            else
            {
                throw new Exception("");
            }
        }

    }
}
