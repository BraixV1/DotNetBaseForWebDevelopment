using App.BLL.DTO;
using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using Base.Contracts.DAL;
using APPDomain = App.Domain;
using DALDTO = App.DAL.DTO;

namespace App.BLL.Services;

public class TokenService : BaseEntityService<App.DAL.DTO.AppRefreshToken, App.BLL.DTO.AppRefreshToken, IRefreshTokenRepository>, IRefreshTokenService
{
    public TokenService(IAppUnitOfWork uoW, IRefreshTokenRepository repository, IMapper mapper) : base(uoW, repository, new BllDalMapper<DALDTO.AppRefreshToken, AppRefreshToken>(mapper))
    {
    }

    public async Task<bool> isValid(string refreshToken)
    {
        var all = await Repository.GetAllAsync();
        var found = all.FirstOrDefault(token => token.RefreshToken == refreshToken);
        if (found == null) return false;
        return found.ExpirationDT > DateTime.Now;
    }
    
}