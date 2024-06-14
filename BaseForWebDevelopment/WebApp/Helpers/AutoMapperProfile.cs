using AutoMapper;

namespace WebApp.Helpers;

public class AutoMapperProfile: Profile
{
    public AutoMapperProfile()
    {

        CreateMap<App.BLL.DTO.AppRefreshToken, App.DTO.v1_0.Identity.AppRefreshToken>().ReverseMap();
        
        CreateMap<App.BLL.DTO.Identity.AppUser, App.DTO.v1_0.Identity.AppUserBll>().ReverseMap();

        CreateMap<App.BLL.DTO.HelperEnums.OrderStatus, App.DTO.v1_0.HelperEnums.OrderStatus>().ReverseMap();
    }
}