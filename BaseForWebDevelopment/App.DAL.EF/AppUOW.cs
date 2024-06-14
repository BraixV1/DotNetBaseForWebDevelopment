using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.DAL.EF.Repositories;
using App.Domain.Identity;
using AutoMapper;
using Base.Contracts.DAL;
using Base.DAL.EF;

namespace App.DAL.EF;

public class AppUOW : BaseUnitOfWork<AppDbContext>, IAppUnitOfWork
{
    private readonly IMapper _mapper;
    public AppUOW(AppDbContext dbContext, IMapper mapper) : base(dbContext)
    {
        _mapper = mapper;
    }
    
    private IOrderRepository? _orders;
    
    
    private IRefreshTokenRepository? _refreshTokens;

    private IEntityRepository<AppUser>? _users;

    

    public IOrderRepository Orders => _orders ?? new OrderRepository(UowDbContext, _mapper);

    public IEntityRepository<AppUser> Users => _users ??
                                               new BaseEntityRepository<AppUser, AppUser, AppDbContext>(UowDbContext,
                                                   new DalDomainMapper<AppUser, AppUser>(_mapper));

    public IRefreshTokenRepository RefreshTokens => _refreshTokens ?? new RefreshTokenRepository(UowDbContext, _mapper);
}