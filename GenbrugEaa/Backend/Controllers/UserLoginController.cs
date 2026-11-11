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
         * Modtager User-objekt eller null, som indikerer om brugerlogin er gyldigt eller ej
         * returnerer User-objekt eller null
         */
        [HttpPost]
        public IActionResult UserLogin(User user)
        {
            User? userFound = ulr.UserLogin(user.Username, user.Password);
            if(userFound != null)
            {
                return Ok(userFound);
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}
