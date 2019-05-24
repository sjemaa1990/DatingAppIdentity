using System;
using Microsoft.AspNetCore.Http;

namespace SGS.eCalc.DTO
{
    public class PhotoForCreationDto
    {
        public string Url { get; set; }

        public IFormFile File { get; set; }
        public string Description { get; set; }
        public DateTime AddedDate { get; set; }
        public string PublicId { get; set; }
        public PhotoForCreationDto()
        {
            AddedDate = DateTime.Now;
        }
    }
}