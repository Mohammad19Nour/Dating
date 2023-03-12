using AutoMapper;
using Dating_App.DTOs;
using Dating_App.Entities;

namespace Dating_App.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>()
            .ForMember(dest => dest.PictureUrl, opt =>
                opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url));
        CreateMap<Photo, PhotoDto>();
        CreateMap<MemberUpdateDto , AppUser>();
        CreateMap<RegisterDto, AppUser>();
        CreateMap<DateTime, DateTime>()
            .ConvertUsing(d => DateTime.SpecifyKind(d,DateTimeKind.Utc));
    }
}