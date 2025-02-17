using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Avalanche.Domain;
using Avalanche.Models;
using System.Xml.Linq;

namespace Avalanche.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var facade = new TestsFacade();
            return Ok(facade.GetAvailiableTests());
        }

        //[HttpGet]
        //[Route("active")]
        //public IActionResult GetActive()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
