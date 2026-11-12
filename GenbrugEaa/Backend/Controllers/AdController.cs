using Core;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/ad")]
public class AdController : ControllerBase
{
    // Reference til vores AdRepository
    private readonly AdRepository _adRepository;

    // Constructor - modtager AdRepository
    public AdController(AdRepository adRepository)
    {
        _adRepository = adRepository;
    }
    
    // UC-02: Henter alle aktive annoncer til Marked-siden
    [HttpGet("active")]
    public ActionResult<List<Ad>> GetAllActiveAds()
    {
        // Kalder repository, som henter alle aktive annoncer fra MongoDB
        List<Ad> ads = _adRepository.GetAllActiveAds();

        // Returnerer annoncerne til frontend
        return Ok(ads);
    }

    // UC-02: Henter en bestemt annonce ud fra ID
    [HttpGet("{adId}")]
    public ActionResult<Ad> GetAd(int adId)
    {
        // Kalder repository for at finde annoncen
        Ad? ad = _adRepository.GetAdById(adId);

        // Tjekker om annoncen findes
        if (ad == null)
        {
            return NotFound("Annoncen findes ikke.");
        }

        // Returnerer annoncen til frontend
        return Ok(ad);
    }

    // UC-09 og UC-11: Henter alle annoncer fra en bestemt sælger
    [HttpGet("seller/{sellerId}")]
    public ActionResult<List<Ad>> GetAdsBySeller(int sellerId)
    {
        // Tjekker om sælger-ID er gyldigt
        if (sellerId <= 0)
        {
            return BadRequest("Sælger-ID er ugyldigt.");
        }

        // Henter sælgerens annoncer fra repository
        List<Ad> ads = _adRepository.GetAdsBySeller(sellerId);

        // Returnerer sælgerens annoncer til frontend
        return Ok(ads);
    }


    
    // UC-08: Opretter en ny annonce
    [HttpPost]
    public IActionResult CreateAd([FromBody] Ad ad)
    {
        // Tjekker om sælger-ID er gyldigt
        if (ad.SellerId <= 0)
        {
            return BadRequest("Sælger-ID er ugyldigt.");
        }

        // Sætter datoen, når annoncen oprettes
        // UtcNow henter det nuværende tidspunkt i utc.
        ad.CreatedAt = DateTime.UtcNow;

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