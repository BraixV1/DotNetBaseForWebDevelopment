
// This is an example repository


using App.Contracts.DAL.Repositories;
using App.Domain;
using AutoMapper;
using Base.Contracts.DAL;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;
using APPDomain = App.Domain;
using DALDTO = App.DAL.DTO;

namespace App.DAL.EF.Repositories;

public class OrderRepository : BaseEntityRepository<DALDTO.Order, App.DAL.DTO.Order, AppDbContext>,  IOrderRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    public OrderRepository(AppDbContext dbContext, IMapper mapper) : 
        base(dbContext, new DalDomainMapper<DALDTO.Order, DALDTO.Order>(mapper))
    {
        _context = dbContext;
        _mapper = mapper;
    }
    
    // This is example. WARNING don't use these types on methods they will kill your database in the long run
    
    // public async Task<IEnumerable<DTO.Order>> GetAllOrderIncludingEverythingAsync()
    // {
    //     var found = await _context.Orders
    //         .Include(order => order.AppUser)
    //         .Include(order => order.OrderItemsCollection)
    //         .ThenInclude(item => item.Keyboard)
    //         .Include(order => order.OrderItemsCollection)
    //         .ThenInclude(item => item.Part)
    //         .ToListAsync();
    //
    //     var result = found.Select(de => Mapper.Map(de));
    //     return result != null
    //         ? _mapper.Map<IEnumerable<App.DAL.DTO.Order>>(result)
    //         : Enumerable.Empty<App.DAL.DTO.Order>();
    // }
}