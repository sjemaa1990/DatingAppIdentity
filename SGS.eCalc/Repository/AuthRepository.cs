using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SGS.eCalc.Data;
using SGS.eCalc.Models;

namespace SGS.eCalc.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string username, string password)
        {
            var currentUser = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.UserName == username);
            if (currentUser == null) return null;

            if(!VerifyPasswordHash(password,currentUser.PasswordHash,currentUser.PasswordSalt ))
            return null;

            return currentUser;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
             using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
              
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                 for (int i=0; i< computeHash.Length;i++) {
                     if(computeHash[i] != passwordHash[i]) return false;
                     
                 }               
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordSalt, passwordHash;

            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string userName)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == userName)) return true;
            return false;
        }
    }
}