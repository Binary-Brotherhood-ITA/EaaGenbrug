using Core;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/shoppinglist")]
public class ShoppingListController : ControllerBase
{
    // Reference til vores ShoppingListRepository
    private readonly ShoppingListRepository _shoppingListRepository;

    // Constructor modtager ShoppingListRepository
    public ShoppingListController(ShoppingListRepository shoppingListRepository)
    {
        _shoppingListRepository = shoppingListRepository;
    }
    // UC-04: Tilføjer en annonce til brugerens indkøbsliste
    [HttpPost("{userId}/ads/{adId}")]
    public IActionResult AddToShoppingList(int userId, int adId)
    {
        // Kontrollerer om ID'erne er gyldige
        if (userId <= 0 || adId <= 0)
        {
            return BadRequest("Ugyldigt bruger- eller annonce-ID.");
        }

        // Tilføjer annoncen gennem repository
        bool success = _shoppingListRepository.AddToShoppingList(userId, adId);

        // Kontrollerer om tilføjelsen lykkedes
        if (success == false)
        {
            return BadRequest("Annoncen kunne ikke tilføjes.");
        }

        // Returnerer en succesbesked
        return Ok("Annoncen er tilføjet til indkøbslisten.");
    }
    
    // UC-02, UC-04, UC-05 og UC-06: Henter brugerens indkøbsliste
    [HttpGet("{userId}")]
    public IActionResult GetUsersShoppingList(int userId)
    {
        // Henter indkøbslisten gennem repository
        ShoppingList? shoppingList = _shoppingListRepository.GetUsersShoppingList(userId);

        // Hvis indkøbslisten ikke findes: Returner en tom List med BuyerId==userId og AdIds en tom Liste af int.
        if (shoppingList == null)
        {
            shoppingList = new ShoppingList
            {
                BuyerId = userId,
                AdIds = new List<int>()
            };
        }

        // Returnerer indkøbslisten
        return Ok(shoppingList);
    }
    
    // UC-06: Fjerner en annonce fra brugerens indkøbsliste
    [HttpDelete("{userId}/ads/{adId}")]
    public IActionResult RemoveFromShoppingList(int userId, int adId)
    {
        // Kontrollerer om ID'erne er gyldige
        if (userId <= 0 || adId <= 0)
        {
            return BadRequest("Ugyldigt bruger- eller annonce-ID.");
        }

        // Fjerner annoncen gennem repository
        bool success = _shoppingListRepository.RemoveFromShoppingList(userId, adId);

        // Kontrollerer om annoncen blev fjernet
        if (success == false)
        {
            return NotFound("Annoncen findes ikke i indkøbslisten.");
        }

        // Returnerer en succesbesked
        return Ok("Annoncen er fjernet fra indkøbslisten.");
    }
}