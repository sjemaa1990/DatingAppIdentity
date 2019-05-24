using System;
using System.Collections.Generic;
using SGS.eCalc.Models;

namespace SGS.eCalc.DTO
{
    public class UserForDetailedDto
    {
        public int Id { get; set; }
        
        public string UserName { get; set; }
        public string Gender { get; set; }
        public string KnownAs { get; set; }
        public int Age { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<PhotosForDetailedDto> Photos { get; set; }
    }
}