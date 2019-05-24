using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGS.eCalc.DTO;
using SGS.eCalc.Helpers;
using SGS.eCalc.Models;
using SGS.eCalc.Repository;

namespace SGS.eCalc.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository datingRepository, IMapper mapper)
        {
            _mapper = mapper;
            _datingRepository = datingRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var connectUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            var connectedUserFromRepo = await _datingRepository.GetUser(connectUserId);
            userParams.UserId = connectUserId;
            
            if(string.IsNullOrEmpty(userParams.Gender)){
                    userParams.Gender = connectedUserFromRepo.Gender == "male" ? "female" : "male";
            }

            var users = await _datingRepository.GetUsers(userParams);

            
            var result =_mapper.Map<IEnumerable<UserForListDTO>>(users);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            // Does not affect performance using async 
            return Ok(result);
        }

        [HttpGet("{id}", Name ="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            return Ok( _mapper.Map<UserForDetailedDto>(await _datingRepository.GetUser(id)));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var userFromRepo = await _datingRepository.GetUser(id);
            _mapper.Map(userForUpdateDto, userFromRepo);
            if(await _datingRepository.SaveAll())
                return NoContent();
            throw new Exception($"Updating user {id} failed on save");

        }
        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var like = await _datingRepository.GetLike(id,recipientId);
            if(like != null){
                return BadRequest("You already liked this user");
            }
            if(await _datingRepository.GetUser(recipientId) == null){
                return NotFound();
            }
            like = new Models.Like(){
                LikerId = id,
                LikeeId = recipientId

            };
            _datingRepository.Add<Like>(like);
            if(await _datingRepository.SaveAll())
                return Ok();
                return BadRequest("Failed to like user");
        }

        // POST api/values
        // [HttpPost]
        // public void Post([FromBody] string value)
        // {
        //     await  _datingRepository.(id)
        //     Return Ok();
        // }

        // // PUT api/values/5
        // [HttpPut("{id}")]
        // public void Put(int id, [FromBody] string value)
        // {
        // }
    }
}