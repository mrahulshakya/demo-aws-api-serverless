using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : Controller
    {
        public CountriesController(IDataProvider provider)
        {
            Provider = provider;
        }

        public IDataProvider Provider { get; }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            //var countries = Countries;
            var countries = await Provider.GetAll();

            //Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            return new OkObjectResult(countries);
        }

        // GET api/values/5
        [HttpGet("{code}")]
        public async Task<ActionResult> Get(string code)
        {
            var result = await Provider.Get(code);
            //Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //Response.Headers.Add("Access-Control-Allow-Credentials", "true");

            return new OkObjectResult(result);
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CountryDetail country)
        {
            try
            {
                var result = await Provider.Add(country);

                //Response.Headers.Add("Access-Control-Allow-Origin", "*");
                //Response.Headers.Add("Access-Control-Allow-Credentials", "true");

                return new CreatedResult("CountriesTable", result);

            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message);
            }
        }
    }
}