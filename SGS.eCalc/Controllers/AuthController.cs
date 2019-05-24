using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SGS.eCalc.DTO;
using SGS.eCalc.Models;
using SGS.eCalc.Repository;

namespace SGS.eCalc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AuthController(IAuthRepository authRepository, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _authRepository = authRepository;
            _config = config;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO userForRegister)
        {


            userForRegister.UserName = userForRegister.UserName.ToLower();
            if (await _authRepository.UserExists(userForRegister.UserName))
                return BadRequest("User already exist");

            var userToCreate = _mapper.Map<User>(userForRegister);

            var createdUser = await _authRepository.Register(userToCreate, userForRegister.Password);
            var userToReturn = _mapper.Map<UserForDetailedDto>(createdUser);

            // Name of route
            return CreatedAtRoute("GetUser", new {Controller = "Users", id=createdUser.Id}, userToReturn);
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDTO userLoginDTO)
        {

            var userFromRepository = await _authRepository.Login(userLoginDTO.UserName.ToLower(), userLoginDTO.Password);
            if (userFromRepository == null)
                return Unauthorized();

            var claims = new[]{
                    new Claim(ClaimTypes.NameIdentifier, userFromRepository.Id.ToString()),
                    new Claim(ClaimTypes.Name, userFromRepository.UserName)
                };
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

            var user = _mapper.Map<UserForListDTO>(userFromRepository);

                return Ok(new
                {
                    Token = tokenHandler.WriteToken(token),
                    User = user
                });

        }
    }
}