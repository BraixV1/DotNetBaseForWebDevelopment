using App.Contracts.BLL.Services;
using App.Contracts.DAL.Repositories;
using Base.Contracts.BLL;

namespace App.Contracts.BLL;

public interface IAppBLL : Base.Contracts.BLL.IBll
{
    
    IRefreshTokenService  AppRefreshTokens { get; }
    
    IOrderService Orders { get; }
    
}