using AutoMapper;

namespace App.DAL.EF;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<App.Domain.Identity.AppRefreshToken, App.DAL.DTO.AppRefreshToken>().ReverseMap();
        
        CreateMap<App.Domain.HelperEnums.OrderStatus, App.DAL.DTO.HelperEnums.OrderStatus>().ReverseMap();
        
        CreateMap<App.Domain.Identity.AppUser, App.DAL.DTO.Identity.AppUser>().ReverseMap();
        
    }
}