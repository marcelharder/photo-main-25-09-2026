

namespace fotoservice.api.helpers;

    public class AutoMapperProfiles : Profile
    {

        public AutoMapperProfiles()
        {
            CreateMap<fotoservice.data.models.Image, ImageDto>().ReverseMap();
            
            
            CreateMap<AppUser, UserDto>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            
          
            CreateMap<UserForUpdateDto, AppUser>().ReverseMap();
            CreateMap<CategoryDto, Category>().ReverseMap();
          
            
             CreateMap<AppUser, UserForReturnDto>().ReverseMap();
        }

    }
