using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Core
{
    public class ShoppingList
    {
        [Key]//betyder primary key og den er unik
        public int Id { get; set; }

        [Required] //Feltet må ikke være NULL
        public int BuyerId { get; set; }

        [Required] //Feltet må ikke være NULL
        public List<int> AdIds { get; set; } = new List<int>();
    }
}
