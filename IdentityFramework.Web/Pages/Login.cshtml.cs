using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IdentityFramework.Models;
using IdentityFramework.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace IdentityFramework.Web
{
    public class Login : PageModel
    {
        private readonly ILogger<Login> _logger;
        private readonly string _BaseUrl;
        private readonly IdentityFrameworkSettings _Settings;
        private readonly HttpClient _Client;
        public Login(ILogger<Login> logger, IOptions<IdentityFrameworkSettings> Settings, HttpClient client)
        {
            _logger = logger;
            _Settings = Settings.Value;
            _Client = client;
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                
                string url = $"/api/user/{_Settings.ApiVersion}/login";
                _Client.DefaultRequestHeaders.Accept.Clear();
                _Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _Client.Timeout = new TimeSpan(9, 9, 9, 9);

                var json = JsonConvert.SerializeObject(Input);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                var apiResult = _Client.PostAsync(url, stringContent).Result;

                if (!apiResult.IsSuccessStatusCode)
                {

                }

                ////set up the client
                //client.BaseAddress = new Uri(_Settings.BaseUrl);
                //client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.Timeout = new TimeSpan(9, 9, 9, 9);

                //// This doesn't count login failures towards account lockout
                //// To enable password failures to trigger account lockout, set lockoutOnFailure: true
                //var jwt = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

                //var json = JsonConvert.SerializeObject(Request);
                //var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                //var apiResult = client.PostAsync(url, stringContent).Result;

                //if (!apiResult.IsSuccessStatusCode)
                //{
                //}

                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddMinutes(60);
                option.HttpOnly = true;
                //Response.Cookies.Append("token", jwt, option);

                //read cookie from Request object  
                //string cookieValueFromReq = Request.Cookies["Key"];


                //client.DefaultRequestHeaders.Accept.Add("Authorization", "Bearer " + bearerToken);
                



                //if (result.Succeeded)
                //{
                //    _logger.LogInformation($"User: {Input.Email} logged in.");
                //    return LocalRedirect(returnUrl);
                //}
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                //}
                //if (result.IsLockedOut)
                //{
                //    _logger.LogWarning("User account locked out.");
                //    return RedirectToPage("./Lockout");
                //}
                //else
                //{
                //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                //    return Page();
                //}
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}