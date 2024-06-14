using App.Contracts.DAL.Repositories;
using App.Domain.Identity;
using Base.Contracts.DAL;

namespace App.Contracts.DAL;

public interface IAppUnitOfWork : IUnitOfWork
{
    // list your repos here

    IOrderRepository Orders { get; }
    
    IEntityRepository<AppUser> Users { get; }
    
    IRefreshTokenRepository RefreshTokens { get; }
}