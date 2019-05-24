using System;
using System.Collections.Generic;
using SGS.eCalc.Models;

namespace SGS.eCalc.DTO
{
    public class UserForUpdateDto
    {
        public string introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}