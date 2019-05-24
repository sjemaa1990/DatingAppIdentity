using System.Collections.Generic;
using Newtonsoft.Json;
using SGS.eCalc.Models;

namespace SGS.eCalc.Data
{
    public class Seed
    {
        private readonly DataContext _context;
        public Seed(DataContext context)
        {
            _context = context;
        }

        public void SeedUsers(){
            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            byte[] passwordHash, passwordSalt;
            foreach( var user in users){
               CreatePasswordHash("password", out passwordHash, out passwordSalt);
               user.PasswordHash = passwordHash;
               user.PasswordSalt =passwordSalt;
               user.UserName = user.UserName.ToLower();

               _context.Add(user);
            }
            _context.SaveChanges();

        }

         private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}