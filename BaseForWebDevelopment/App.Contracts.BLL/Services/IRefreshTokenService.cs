using App.Contracts.DAL.Repositories;
using Base.Contracts.DAL;

namespace App.Contracts.BLL.Services;

public interface IRefreshTokenService : IEntityRepository<App.BLL.DTO.AppRefreshToken>, IRefreshTokenRepositoryCustom<Guid>
{
    
}