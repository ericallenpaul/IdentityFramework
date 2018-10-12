using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityFramework.API.Controllers
{
    /// <summary>
    /// Test Controller
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [ApiController]
    public class ValuesController : Controller
    {
        public ValuesController()
        {
            
        }

        /// <summary>
        /// Gets some values.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(string[]), 200)]
        [ProducesResponseType(typeof(NotFoundObjectResult), 400)]
        [ProducesResponseType(500)]
        [SwaggerOperation(OperationId = "GetSomeValues")]
        [HttpGet]
        [Route("api/v1/GetSomeValues", Name = "GetSomeValues")]
        public ActionResult<IEnumerable<string>> GetSomeValues()
        {
            
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet]
        [Authorize]
        [Route("api/v1/GetById", Name = "GetById")]
        public ActionResult<string> GetById(int id)
        {
            return "You must be logged in to see this message";
        }

        // POST api/values
        [HttpPost]
        [Route("api/v1/Post", Name = "Post")]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut]
        [Route("api/v1/Put", Name = "Put")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete]
        [Route("api/v1/Delete", Name = "Delete")]
        public void Delete(int id)
        {
        }
    }
}
