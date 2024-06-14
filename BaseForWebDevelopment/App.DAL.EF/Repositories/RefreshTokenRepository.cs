using App.Contracts.DAL.Repositories;
using AutoMapper;
using Base.DAL.EF;
using APPDomain = App.Domain;
using DALDTO = App.DAL.DTO;

namespace App.DAL.EF.Repositories;

public class RefreshTokenRepository : BaseEntityRepository<App.Domain.Identity.AppRefreshToken, App.DAL.DTO.AppRefreshToken, AppDbContext>, 
    IRefreshTokenRepository 
    
{
    public RefreshTokenRepository(AppDbContext dbContext, IMapper mapper) :
        base(dbContext, new DalDomainMapper<App.Domain.Identity.AppRefreshToken, App.DAL.DTO.AppRefreshToken>(mapper))
    {
    }

    public async Task<bool> isValid(string refreshToken)
    {
        var query = GetAll();
        var found = query.FirstOrDefault(token => token.RefreshToken == refreshToken);
        if (found == null) return false;
        return await Task.Run(() => found.ExpirationDT > DateTime.Now);
    }
}