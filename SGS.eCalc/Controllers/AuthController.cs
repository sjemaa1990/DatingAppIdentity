using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SGS.eCalc.DTO;
using SGS.eCalc.Models;
using SGS.eCalc.Repository;

namespace SGS.eCalc.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(IConfiguration config,
                                 IMapper mapper, 
                                 UserManager<User> userManager,
                                 SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _config = config;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO userForRegister)
        {
            var userToCreate = _mapper.Map<User>(userForRegister);
            var result = await _userManager.CreateAsync(userToCreate, userForRegister.Password);
            
            var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);

            if(result.Succeeded)
            {
                // Name of route
            return CreatedAtRoute("GetUser", new { Controller = "Users", id = userToCreate.Id }, userToReturn);
            }
            return BadRequest(result.Errors);
        }
        [HttpPost("login")]

        public async Task<IActionResult> Login(UserLoginDTO userLoginDTO)
        {

            //var userFromRepository = await _authRepository.Login(userLoginDTO.UserName.ToLower(), userLoginDTO.Password);

            var user = await _userManager.FindByNameAsync(userLoginDTO.UserName);

            var result = await _signInManager.CheckPasswordSignInAsync(user,userLoginDTO.Password,false);

            if (result.Succeeded)
            {
                 var appUser = await _userManager.Users.Include(p => p.Photos)
                                                        .FirstOrDefaultAsync(u => u.NormalizedUserName == userLoginDTO.UserName.ToUpper());
                 var userToReturn =  _mapper.Map<UserForListDTO>(appUser);
                 return Ok(new
                    {
                        Token = GenerateJwtToken(appUser).Result,
                        User = userToReturn
                    });
            }
            return Unauthorized();

        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>{
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName)
                };
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}