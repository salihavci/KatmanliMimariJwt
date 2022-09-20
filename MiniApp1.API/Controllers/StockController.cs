using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniApp1.API.Controllers
{
    [Authorize(Policy = "AgePolicy")]
    [Authorize(Roles = "Admin",Policy = "CityPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStock()
        {
            var userName = HttpContext.User.Identity.Name;
            var userIdClaim = User.Claims.FirstOrDefault(m=> m.Type == ClaimTypes.NameIdentifier);
            return Ok($"Stock işlemleri - Username : {userName} - Userid : {userIdClaim.Value}");
        }

    }
}
