using System.Linq;
using AutoMapper;
using SGS.eCalc.DTO;
using SGS.eCalc.Models;

namespace SGS.eCalc.Helpers
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiSecret { get; set; }
        public string ApiKey { get; set; }
    }
}