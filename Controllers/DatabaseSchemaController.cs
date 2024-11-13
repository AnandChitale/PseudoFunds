using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API Test for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Barclays.GenAIHackathon.OpenAIWrapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseSchemaController : ControllerBase
    {
        // GET: api/<DatabaseSchemaController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<DatabaseSchemaController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DatabaseSchemaController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<DatabaseSchemaController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DatabaseSchemaController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
