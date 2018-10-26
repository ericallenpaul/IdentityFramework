using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IdentityFramework.Models;
using IdentityFramework.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog;
using Swashbuckle.AspNetCore.Annotations;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdentityFramework.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        //private readonly IMapper _Mapper;
        private readonly ApplicationDbContext _Context;
        private readonly IdentityFrameworkSettings _Settings;
        private readonly IMemoryCache _Cache;
        private readonly IUserService _UserService;
        private readonly ILogger<UserController> _Logger;
        private readonly UserManager<IdentityUser> _UserManager;
        private readonly IEmailService _emailService;
        private readonly IIdentityFramework_JWT _TokenOptions;
        
        //private NLog.ILogger _Logger => LogManager.GetLogger(this.GetType().FullName);

        public UserController(
            ApplicationDbContext Context,
            //IMapper Mapper,
            IOptions<IdentityFrameworkSettings> Settings,
            IMemoryCache MemoryCache,
            IUserService UserService,
            UserManager<IdentityUser> userManager,
            ILogger<UserController> logger,
            IOptions<IdentityFramework_JWT> TokenOptions,
            IEmailService emailService
        )
        {
            _Context = Context;
            //_Mapper = Mapper;
            _UserService = UserService;
            _UserManager = userManager;
            _Settings = Settings.Value;
            _Cache = MemoryCache ?? new MemoryCache(new MemoryCacheOptions());
            _Logger = logger;
            _emailService = emailService;
            _TokenOptions = TokenOptions.Value;
        }

        [AllowAnonymous]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [ProducesResponseType(typeof(BadRequestResult), 400)]
        [ProducesResponseType(500)]
        [SwaggerOperation(OperationId = "Register")]
        [HttpPost]
        [Route("v1/Register", Name = "Register")]
        public async Task<IActionResult> Register(string Email, string Password)
        {
            _Logger.LogInformation($"Refgistering user {Email}...");
            IdentityResult returnValue = null;

            #region Validate Parameters
            if (String.IsNullOrEmpty(Email))
                ModelState.AddModelError($"Register Error", $"The {nameof(Email)} cannot be zero");

            if (!ModelState.IsValid)
            {
                LogInvalidState();
                return BadRequest(ModelState);
            }
            else
            {
                _Logger.LogDebug($"ModelState is valid.");
            }
            #endregion Validate Parameters

            try
            {
                 returnValue = await _UserService.Register(Email,Password);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"Register Unexpected Error: {ex}");
                return StatusCode(500, $"Register Unexpected Error: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")} {ex}");
            }

            //return the new certificate
            _Logger.LogInformation($"Register complete.");
            return Ok(returnValue);
        }

        [AllowAnonymous]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [ProducesResponseType(typeof(BadRequestResult), 400)]
        [ProducesResponseType(500)]
        [SwaggerOperation(OperationId = "Login")]
        [HttpPost]
        [Route("v1/Login", Name = "Login")]
        public async Task<IActionResult> Login(string Email, string Password, bool RemeberMe)
        {
            _Logger.LogInformation($"Login for user: {Email}...");
            string returnValue = null;

            #region Validate Parameters
            if (String.IsNullOrEmpty(Email))
                ModelState.AddModelError($"Login Error", $"The {nameof(Email)} cannot be empty");

            if (String.IsNullOrEmpty(Password))
                ModelState.AddModelError($"Login Error", $"The {nameof(Password)} cannot be empty");

            if (!ModelState.IsValid)
            {
                LogInvalidState();
                return BadRequest(ModelState);
            }
            else
            {
                _Logger.LogDebug($"ModelState is valid.");
            }
            #endregion Validate Parameters

            try
            {
                returnValue = await _UserService.Login(Email, Password, RemeberMe);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"Login Unexpected Error: {ex}");
                return StatusCode(500, $"Login Unexpected Error: {ex}");
            }

            //return the new certificate
            _Logger.LogInformation($"Login complete.");
            return Ok(returnValue);
        }

        

        private void LogInvalidState()
        {
            string errors = "";

            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    errors += error + " ";
                }
            }

            _Logger.LogError($"Invalid ModelState: {errors}");
        }


    }
}