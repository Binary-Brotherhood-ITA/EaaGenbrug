using System;
using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class PurchaseRequests
    {
        [Key] //betyder primary key og den er unik
        public int Id { get; set; }

        [Required] //Feltet må ikke være NULL
        public int BuyerId { get; set; }

        [Required]
        public int SellerId { get; set; }

        [Required]
        public int AdId { get; set; }

        public string Status { get; set; }

        [Required]
        public DateTime CreateAt { get; set; } = DateTime.Now;

        //(DateTime?) betyder Dato kan være Null
        public DateTime? StatusChangeDate { get; set; }
    }
}