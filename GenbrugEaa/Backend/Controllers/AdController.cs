using Core;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdController : ControllerBase
{
    // Reference til vores AdRepository
    private readonly AdRepository _adRepository;

    // Constructor - modtager AdRepository
    public AdController(AdRepository adRepository)
    {
        _adRepository = adRepository;
    }

    // UC-08: Opretter en ny annonce
    [HttpPost]
    public IActionResult CreateAd([FromBody] Ad ad)
    {
        // Tjekker om annoncen har en sælger
        if (ad.SellerId <= 0)
        {
            return BadRequest("Sælger-ID er ugyldigt.");
        }

        // Gemmer annoncen gennem repository
        bool success = _adRepository.CreateAd(ad);

        // Tjekker om annoncen blev gemt
        if (success == false)
        {
            return StatusCode(500, "Annoncen kunne ikke gemmes.");
        }

        // Returnerer en succesbesked
        return Ok("Annoncen er oprettet.");
    }
    
    // UC-09: Sletter en annonce
    [HttpDelete("{adId}")]
    public IActionResult DeleteAd(int adId, [FromQuery] int sellerId)
    {
        // Tjekker om sælger-ID er gyldigt
        if (sellerId <= 0)
        {
            return BadRequest("Sælger-ID er ugyldigt.");
        }

        // Tjekker om annoncen findes
        Ad? ad = _adRepository.GetAdById(adId);

        if (ad == null)
        {
            return NotFound("Annoncen findes ikke.");
        }

        // Tjekker om brugeren ejer annoncen
        if (ad.SellerId != sellerId)
        {
            return Forbid();
        }

        // Sletter annoncen gennem repository
        bool success = _adRepository.DeleteAd(adId, sellerId);

        // Tjekker om sletningen lykkedes
        if (success == false) 
            
            // 500 står for databasefejl/ server error
        {
            return StatusCode(500, "Annoncen kunne ikke slettes.");
        }

        // Returnerer en succesbesked
        return Ok("Annoncen er slettet.");
    }
    
    // UC-10: Redigerer en eksisterende annonce
   
    [HttpPut("{adId}")]
    public IActionResult UpdateAd(int adId, [FromBody] Ad updatedAd)
    {
        // Finder annoncen i databasen
        Ad? ad = _adRepository.GetAdById(adId);

        // Tjekker om annoncen findes
        if (ad == null)
        {
            return NotFound("Annoncen findes ikke.");
        }

        // Tjekker om sælgeren ejer annoncen
        if (ad.SellerId != updatedAd.SellerId)
        {
            return Forbid();
        }

        // Opdaterer annoncen gennem repository
        bool success = _adRepository.UpdateAd(adId, updatedAd);

        // Tjekker om opdateringen lykkedes
        if (success == false)
        {
            return StatusCode(500, "Annoncen kunne ikke opdateres.");
        }

        // Returnerer en succesbesked
        return Ok("Annoncen er opdateret.");
    }


}