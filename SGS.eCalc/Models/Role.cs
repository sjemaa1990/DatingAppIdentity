using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace SGS.eCalc.Models
{
    public class Role: IdentityRole<int>
    {
        public ICollection<UserRole> UserRoles { get; set; }
    }
}