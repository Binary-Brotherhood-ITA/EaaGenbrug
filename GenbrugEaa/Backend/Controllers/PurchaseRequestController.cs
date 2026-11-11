using Core;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseRequestController : ControllerBase
{
    // Reference til PurchaseRequestRepository
    private readonly PurchaseRequestRepository _purchaseRequestRepository;

    // Reference til AdRepository
    private readonly AdRepository _adRepository;

    // Constructor modtager begge repositories
    public PurchaseRequestController(
        PurchaseRequestRepository purchaseRequestRepository,
        AdRepository adRepository)
    {
        _purchaseRequestRepository = purchaseRequestRepository;
        _adRepository = adRepository;
    }
    
    // UC-05: Sender en købsanmodning
    [HttpPost("send/{adId}/{buyerId}")]
    public IActionResult SendRequest(int adId, int buyerId)
    {
        // Kontrollerer om ID'erne er gyldige
        if (adId <= 0 || buyerId <= 0)
        {
            return BadRequest("Ugyldigt annonce- eller køber-ID.");
        }

        // Finder annoncen gennem AdRepository
        Ad? ad = _adRepository.GetAdById(adId);

        // Kontrollerer om annoncen findes
        if (ad == null)
        {
            return NotFound("Annoncen findes ikke.");
        }
        
        // UC-05: Kontrollerer om annoncen er aktiv
        if (ad.Status != "Active")
        {
            return Conflict("Annoncen er ikke længere tilgængelig for køb.");
        }
        
        // UC-05: Kontrollerer at køberen ikke er sælgeren
        if (ad.SellerId == buyerId)
        {
            return BadRequest("Du kan ikke anmode om at købe din egen annonce.");
        }
        
        // UC-05: Kontrollerer om køberen allerede har en aktiv købsanmodning
        bool requestExists = _purchaseRequestRepository.ActiveRequestExists(buyerId, adId);

        if (requestExists == true)
        {
            return Conflict("Du har allerede en aktiv købsanmodning på denne annonce.");
        }
        
        // UC-05: Opretter en ny købsanmodning
        PurchaseRequest request = new PurchaseRequest
        {
            BuyerId = buyerId,
            SellerId = ad.SellerId,
            AdId = adId
        };

        // UC-05: Gemmer købsanmodningen i MongoDB
        bool success = _purchaseRequestRepository.SaveRequest(request);

        // Kontrollerer om købsanmodningen blev gemt
        if (success == false)
        {
            return StatusCode(500, "Købsanmodningen kunne ikke gemmes.");
        }

        // Returnerer en succesbesked
        return Ok("Din købsanmodning er sendt.");
    }

    // UC-11: Henter sælgerens afventende købsanmodninger
    [HttpGet("seller/{sellerId}/pending")]
    public IActionResult GetPendingForSeller(int sellerId)
    {
        // Kontrollerer om sælger-ID er gyldigt
        if (sellerId <= 0)
        {
            return BadRequest("Ugyldigt sælger-ID.");
        }

        // Henter sælgerens afventende købsanmodninger
        List<PurchaseRequest> requests =
            _purchaseRequestRepository.GetPendingForSeller(sellerId);

        // Returnerer købsanmodningerne
        return Ok(requests);
    }
    
    // UC-11: Accepterer en købsanmodning
    [HttpPut("{requestId}/accept")]
    public IActionResult AcceptRequest(int requestId, [FromQuery] int sellerId)
    {
        // Kontrollerer om ID'erne er gyldige
        if (requestId <= 0 || sellerId <= 0)
        {
            return BadRequest("Ugyldigt anmodnings- eller sælger-ID.");
        }

        // Finder købsanmodningen gennem repository
        PurchaseRequest? request = _purchaseRequestRepository.GetRequest(requestId);

        // Kontrollerer om købsanmodningen findes
        if (request == null)
        {
            return NotFound("Købsanmodningen findes ikke.");
        }

        // Kontrollerer om anmodningen tilhører sælgeren
        if (request.SellerId != sellerId)
        {
            return StatusCode(403, "Du har ikke tilladelse til at behandle denne købsanmodning.");
        }

        // Kontrollerer om anmodningen stadig afventer
        if (request.Status != "Pending")
        {
            return Conflict("Købsanmodningen er allerede behandlet.");
        }


        // UC-11: Reserverer annoncen
        bool adReserved = _adRepository.SetAdStatus(request.AdId, "Reserved");

        // Kontrollerer om annoncen blev reserveret
        if (adReserved == false)
        {
            return Conflict("Annoncen kunne ikke reserveres.");
        }

        // UC-11: Accepterer købsanmodningen
        bool requestAccepted = _purchaseRequestRepository.SetRequestStatus(requestId, "accepted");

        // Kontrollerer om købsanmodningen blev accepteret
        if (requestAccepted == false)
        {
            return StatusCode(500, "Annoncen blev reserveret, men købsanmodningen kunne ikke opdateres.");
        }

        // Returnerer en succesbesked
        return Ok("Købsanmodningen er accepteret.");
    }
    
    // UC-11: Afviser en købsanmodning
    [HttpPut("{requestId}/reject")]
    public IActionResult RejectRequest(int requestId, [FromQuery] int sellerId)
    {
        // Kontrollerer om ID'erne er gyldige
        if (requestId <= 0 || sellerId <= 0)
        {
            return BadRequest("Ugyldigt anmodnings- eller sælger-ID.");
        }

        // Finder købsanmodningen gennem repository
        PurchaseRequest? request = _purchaseRequestRepository.GetRequest(requestId);

        // Kontrollerer om købsanmodningen findes
        if (request == null)
        {
            return NotFound("Købsanmodningen findes ikke.");
        }

        // Kontrollerer om anmodningen tilhører sælgeren
        if (request.SellerId != sellerId)
        {
            return StatusCode(403, "Du har ikke tilladelse til at afvise denne købsanmodning.");
        }

        // Kontrollerer om anmodningen stadig afventer
        if (request.Status != "Pending")
        {
            return Conflict("Købsanmodningen er allerede behandlet.");
        }

        // UC-11: Ændrer købsanmodningens status til declined
        bool success = _purchaseRequestRepository.SetRequestStatus(requestId, "declined");

        // Kontrollerer om ændringen lykkedes
        if (success == false)
        {
            return Conflict("Købsanmodningen kunne ikke afvises.");
        }

        // Returnerer en succesbesked
        return Ok("Købsanmodningen er afvist.");
    }


}
