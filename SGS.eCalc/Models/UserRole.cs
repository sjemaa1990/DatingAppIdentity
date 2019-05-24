using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace SGS.eCalc.Models
{
    public class UserRole: IdentityUserRole<int>
    {
        public User User { get; set; }
        public Role Role { get; set; }
    }
}