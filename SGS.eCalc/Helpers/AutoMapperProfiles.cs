using System.Linq;
using AutoMapper;
using SGS.eCalc.DTO;
using SGS.eCalc.Models;

namespace SGS.eCalc.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            // source class to destination class for mapping
            CreateMap<User, UserForListDTO>()
            .ForMember(des => des.PhotoUrl ,opt =>{
                opt.MapFrom(src => src.Photos.FirstOrDefault( p => p.IsMain).Url);
            })
            .ForMember(dest => dest.Age, option =>  {
                option.ResolveUsing(d => d.DateOfBirth.CalculateAge());
            });
            CreateMap<User, UserForDetailedDto>()
            .ForMember(des => des.PhotoUrl ,opt =>{
                opt.MapFrom(src => src.Photos.FirstOrDefault( p => p.IsMain).Url);
            })
            .ForMember(dest => dest.Age, option =>  {
                option.ResolveUsing(d => d.DateOfBirth.CalculateAge());
            });
            CreateMap<Photo, PhotosForDetailedDto>();

            CreateMap<UserForUpdateDto,User>();
            CreateMap<Photo,PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto,Photo>();
            CreateMap<UserRegisterDTO,User>();

            CreateMap<MessageForCreationDto,Message>().ReverseMap();  // to use this map for reverse way too 
            CreateMap<Message,MessageToReturnDto>()
                                    .ForMember(des => des.SenderPhotoUrl, opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault( p => p.IsMain).Url))
                                    .ForMember(des => des.RecipientPhotoUrl, opt => opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault( p => p.IsMain).Url));



        }
    }
}