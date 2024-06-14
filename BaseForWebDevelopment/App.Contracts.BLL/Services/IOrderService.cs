using App.Contracts.DAL.Repositories;
using Base.Contracts.DAL;

namespace App.Contracts.BLL.Services;

public interface IOrderService : IEntityRepository<App.BLL.DTO.Order>, IOrderRepositoryCustom<App.BLL.DTO.Order>
{
    // You need to also reference these methods here
    
    // public Task<App.BLL.DTO.Order> AddNewOrder(App.BLL.DTO.Order order);
}