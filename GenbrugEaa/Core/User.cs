using System.ComponentModel.DataAnnotations;

namespace Core;

    public class User
    {
        [Key] //betyder primary key og den er unik
        public int Id { get; set; }

        [Required] //Feltet må ikke være NULL
        [MaxLength(100)] 
        public string Name { get; set; }

        [Required] //Feltet må ikke være NULL
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required] [MaxLength(50)] 
        public string Phone { get; set; }

        [Required] [MaxLength(50)] 
        public string Username { get; set; }

        [Required] [MinLength(8)] 
        public string Password { get; set; }
    }
