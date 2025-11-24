using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetRadar.Core.Data;

namespace PetRadar.Web.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {

        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "Login")]
        public string Get()
        {
            string name = "Super Admin";
            
            return name;
        }
    }
}
