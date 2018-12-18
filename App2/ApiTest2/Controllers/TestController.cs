using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiTest2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        [Route("Authorized")]
        public IActionResult Authorized()
        {
            return new JsonResult("Authorized");
        }

        [HttpGet]
        [Route("Anonymous")]
        public IActionResult Anonymous()
        {
            return new JsonResult("Anonymous");
        }
    }
}