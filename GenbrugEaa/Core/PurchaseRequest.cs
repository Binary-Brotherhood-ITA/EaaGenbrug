using System;
using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class PurchaseRequest
    {
        [Key] //betyder primary key og den er unik
        public int Id { get; set; }

        [Required] //Feltet må ikke være NULL
        public int BuyerId { get; set; }

        [Required]
        public int SellerId { get; set; }

        [Required]
        public int AdId { get; set; }
       
        // En ny købsanmodning starter med status awaiting
        public string Status { get; set; } = "Pending";

        // Gemmer tidspunktet hvor købsanmodningen oprettes
        public DateTime CreatedAt { get; set; }

        // Er null indtil status ændres første gang
        public DateTime? StatusChangeDate { get; set; }
    }
}