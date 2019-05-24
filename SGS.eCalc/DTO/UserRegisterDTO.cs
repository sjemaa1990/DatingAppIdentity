using System;
using System.ComponentModel.DataAnnotations;

namespace SGS.eCalc.DTO
{
    public class UserRegisterDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(8, MinimumLength =4, ErrorMessage = "You must specify password between 4 and 8.")]
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public UserRegisterDTO (){
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
 
    }
}