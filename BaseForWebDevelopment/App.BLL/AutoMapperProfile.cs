using AutoMapper;

namespace App.BLL;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {

        CreateMap<App.DAL.DTO.AppRefreshToken, App.BLL.DTO.AppRefreshToken>().ReverseMap();

        CreateMap<App.DAL.DTO.HelperEnums.OrderStatus, App.BLL.DTO.HelperEnums.OrderStatus>();
        
        CreateMap<App.DAL.DTO.Order, App.BLL.DTO.Order>().ReverseMap();
        
        CreateMap<App.DAL.DTO.Identity.AppUser, App.BLL.DTO.Identity.AppUser>().ReverseMap();
        
        
    }
}