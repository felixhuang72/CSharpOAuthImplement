using System.Collections.Generic;
using System.Web.Http;

namespace WebApiJWT.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [Authorize]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost, Route("api/valuesWithRole")]
        [Authorize(Roles = "SysAdmin")] //指定具有 SysAdmin 角色的帳號能存取
        public IEnumerable<string> GetWithRole()
        {
            return new string[] { "s_value1", "s_value2" };
        }

        // GET api/values/5        
        public string Get(int id)
        {            
            return id.ToString();
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}