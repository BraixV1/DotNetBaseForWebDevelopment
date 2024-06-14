using App.BLL.Services;
using App.Contracts.BLL;
using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.DAL.EF;
using AutoMapper;
using Base.BLL;
using Microsoft.EntityFrameworkCore.Metadata;

namespace App.BLL;

public class AppBll: BaseBll<AppDbContext>, IAppBLL
{
    private readonly IMapper _mapper;
    private readonly IAppUnitOfWork _uow;
    
    public AppBll(IAppUnitOfWork uoW, IMapper mapper) : base(uoW)
    {
        _mapper = mapper;
        _uow = uoW;
    }
    

    private IRefreshTokenService? _tokenService;

    public IRefreshTokenService AppRefreshTokens => _tokenService ?? new TokenService(_uow, _uow.RefreshTokens, _mapper);
    

    private IOrderService? _orders;

    public IOrderService Orders => _orders ?? new OrderService(_uow, _uow.Orders, _mapper);
    
    
}