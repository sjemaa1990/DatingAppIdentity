using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SGS.eCalc.DTO;
using SGS.eCalc.Helpers;
using SGS.eCalc.Models;
using SGS.eCalc.Repository;

namespace SGS.eCalc.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository datingRepository, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _mapper = mapper;
            _datingRepository = datingRepository;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(
                cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _datingRepository.GetPhoto(id);

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto PhotoForCreationDto)
        {
             if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

             var userFromRepo = await _datingRepository.GetUser(userId);

             var file = PhotoForCreationDto.File;
             var uploadResult = new ImageUploadResult();

             if (file.Length >0 ){
                 using(var stream = file.OpenReadStream()){
                     // dispose file on memory after read
                        var uploadParams = new ImageUploadParams(){
                            File = new FileDescription(file.Name, stream),
                            Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                        };

                        uploadResult = _cloudinary.Upload(uploadParams);
                 }
             }
             PhotoForCreationDto.Url = uploadResult.Uri.ToString();
             PhotoForCreationDto.PublicId = uploadResult.PublicId;
             var photo = _mapper.Map<Photo>(PhotoForCreationDto);
            
            
            if(!userFromRepo.Photos.Any(p => p.IsMain))
                photo.IsMain = true;

            userFromRepo.Photos.Add(photo);

            if(await _datingRepository.SaveAll()){
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto",new { id = photo.Id},photoToReturn);
            }

            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id){
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _datingRepository.GetUser(userId);
            if (!userFromRepo.Photos.Any(p => p.Id == id))
             return Unauthorized();

             var photoFromRepo = userFromRepo.Photos.FirstOrDefault(p => p.Id == id);
            if(photoFromRepo.IsMain)
             return BadRequest("This already the main photo");

             var currenMainPhoto = userFromRepo.Photos.FirstOrDefault(p => p.IsMain);

             currenMainPhoto.IsMain = false;
             photoFromRepo.IsMain = true;
             if(await _datingRepository.SaveAll())
                 return NoContent();

             return BadRequest("Could not set photo to main");
        }

       [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id){

            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _datingRepository.GetUser(userId);
            if (!userFromRepo.Photos.Any(p => p.Id == id))
             return Unauthorized();

             var photoFromRepo = userFromRepo.Photos.FirstOrDefault(p => p.Id == id);

            if(photoFromRepo.IsMain)
             return BadRequest("You cannot delete your main photo main photo");

            if(photoFromRepo.PublicId != null){
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);

                if(result.Result == "ok"){
                    _datingRepository.Delete(photoFromRepo);
                }
               
            }
            if(photoFromRepo.PublicId == null){
                 _datingRepository.Delete(photoFromRepo);
            }

            if(await _datingRepository.SaveAll())
                    return Ok();

             return BadRequest("Failed to delete the photo");
        }


    }
}