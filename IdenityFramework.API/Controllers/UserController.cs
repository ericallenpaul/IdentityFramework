using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityFramework.Models;
using IdentityFramework.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace IdentityFramework.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _Mapper;
        private readonly ApplicationDbContext _Context;
        private readonly IdentityFrameworkSettings _Settings;
        private readonly IMemoryCache _Cache;

        public UserController(
            ApplicationDbContext Context,
            IMapper Mapper,
            IOptions<IdentityFrameworkSettings> Settings,
            IMemoryCache MemoryCache,
            NLog.ILogger Logger = null
        )
        {
            _Context = Context;
            _Mapper = Mapper;
            _Settings = Settings.Value;
            _Cache = MemoryCache ?? new MemoryCache(new MemoryCacheOptions());
        }

        [ProducesResponseType(typeof(ApiOkResponse), 200)]
        [ProducesResponseType(typeof(ApiBadRequestResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        [SwaggerOperation(OperationId = "GetNewCertificateLog")]
        [HttpPost]
        [Route("api/v1/GetNewCertificateLog", Name = "GetNewCertificateLog")]
        public async Task<IActionResult> GetNewCertificateLog(int Page, int NumberOfRecords)
        {
            _Logger.Info($"Getting {NumberOfRecords} of the certificate log: Page {Page} ");
            List<NewCertificateLog> returnValue = null;

            #region Validate Parameters
            if (NumberOfRecords == 0)
                ModelState.AddModelError($"GetNewCertificateLog Error", $"The {nameof(NumberOfRecords)} cannot be zero");

            if (!ModelState.IsValid)
            {
                LogInvalidState();
                return BadRequest(new ApiBadRequestResponse(ModelState));
            }
            else
            {
                _Logger.Debug($"ModelState is valid.");
            }
            #endregion Validate Parameters

            try
            {
                returnValue = _NewCertificateLogService.GetAllEntries(Page, NumberOfRecords);
            }
            catch (Exception ex)
            {
                _Logger.Error($"GetNewCertificateLog Error: {ex}");
                return StatusCode(500, new ApiResponse(500, $"GetNewCertificateLog Error: {ex}"));
            }

            //return the new certificate
            _Logger.Info($"GetNewCertificateLog complete.");
            return Ok(new ApiOkResponse(returnValue));
        }



    }
}