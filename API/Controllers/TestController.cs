using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        // GET: TestController
        [HttpGet]
        public ActionResult Index()
        {

            return Ok("Sve radi bravo!");
        }

     
    }
}
