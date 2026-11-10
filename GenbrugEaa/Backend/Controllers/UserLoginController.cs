using Core;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("/api/login")]
    public class UserLoginController : ControllerBase
    {
        private readonly UserLoginRepository ulr;

        public UserLoginController(UserLoginRepository loginRepo)
        {
            this.ulr = loginRepo;
        }

        /* HttpPost:
         * Kalder loginRepo.UserLogin
         * Modtager bool retur, som indikerer om brugerlogin er gyldigt eller ej
         */
        [HttpPost]
        public IActionResult UserLogin(string username, string password)
        {
            bool isValid = ulr.UserLogin(username, password);
            if(isValid)
            {
                return Ok(true);
            }
            else
            {
                return Unauthorized(false);
            }
        }

    }
}
