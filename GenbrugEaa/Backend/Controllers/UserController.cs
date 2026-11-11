
using Core;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    // Reference til vores UserRepository
    private readonly UserRepository _userRepository;

    // Constructor - modtager UserRepository
    public UserController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    // =====================================
    // GET-METODER
    // =====================================

    // Hjælpemetode: Henter alle brugere
    [HttpGet]
    public ActionResult<List<User>> GetAllUsers()
    {
        // Henter alle brugere gennem repository
        List<User> users = _userRepository.GetAllUsers();

        // Returnerer listen med brugere
        return Ok(users);
    }


    // Hjælpemetode: Henter en bestemt bruger ud fra ID
    [HttpGet("{userId}")]
    public ActionResult<User> GetUserById(int userId)
    {
        // Finder brugeren gennem repository
        User? user = _userRepository.GetUserById(userId);

        // Tjekker om brugeren findes
        if (user == null)
        {
            return NotFound("Brugeren findes ikke.");
        }

        // Returnerer brugeren
        return Ok(user);
    }
}
