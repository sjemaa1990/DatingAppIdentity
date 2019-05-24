using System;
using System.Collections.Generic;
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
using SGS.eCalc.Helpers;
using SGS.eCalc.Models;
using SGS.eCalc.Repository;

namespace SGS.eCalc.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
         private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;
        public MessageController(IDatingRepository datingRepository, IMapper mapper)
        {
            _mapper = mapper;
            _datingRepository = datingRepository;
        }

        [HttpGet("{id}", Name ="GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

                var messageFromRepo = await _datingRepository.GetMessage(id);
                if (messageFromRepo == null)
                    return NotFound();
                return Ok(messageFromRepo);

        }
        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams messageParams)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageParams.UserId = userId;
            var messageFromRepo = await _datingRepository.GetMessagesForUser(messageParams);

            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);

            Response.AddPagination(messageFromRepo.CurrentPage,messageFromRepo.PageSize,
                                     messageFromRepo.TotalCount, messageFromRepo.TotalPages);

            return Ok(messages);
        }
        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            
            var messagesFromRepo = await _datingRepository.GetMessageThread(userId, recipientId);

            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            var sender = await _datingRepository.GetUser(userId);

            if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDto.SenderId = userId;
            var recipient = await _datingRepository.GetUser(messageForCreationDto.RecipientId);

            if(recipient == null)
                return BadRequest("Could not find user");

            var message = _mapper.Map<Message>(messageForCreationDto);
            _datingRepository.Add(message);

            

            if(await _datingRepository.SaveAll()){

                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new {id = message.Id},messageToReturn);
            }
                

            throw new Exception("Creating message failed on save");

        }

         [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var messageFromRepo = await _datingRepository.GetMessage(id);

            if(messageFromRepo.SenderId == userId)
                messageFromRepo.SenderDeleted = true;

            if(messageFromRepo.RecipientId == userId)
                messageFromRepo.RecipientDeleted = true;
            
            if(messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
                _datingRepository.Delete(messageFromRepo);

            if(await _datingRepository.SaveAll())
                return NoContent();

            throw new Exception("Error deleting the message");
            
        }
         [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var messageFromRepo = await _datingRepository.GetMessage(id);

            if(messageFromRepo.RecipientId != userId)
                return Unauthorized();
            
            messageFromRepo.IsRead = true;
            messageFromRepo.DateRead = DateTime.Now;

            await _datingRepository.SaveAll();

            return NoContent();
        }


    }
}