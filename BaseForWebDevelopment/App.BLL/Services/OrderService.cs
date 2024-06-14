using App.BLL.DTO;
using App.BLL.DTO.HelperEnums;
using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.Domain.Identity;
using AutoMapper;
using Base.BLL;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace App.BLL.Services;

public class OrderService : BaseEntityService<App.DAL.DTO.Order, App.BLL.DTO.Order, IOrderRepository>,
    IOrderService
{
    private readonly IMapper _mapper;
    private readonly IAppUnitOfWork _uow;
    public OrderService(IAppUnitOfWork uoW, IOrderRepository repository, IMapper mapper) : base(uoW,
        repository, new BllDalMapper<App.DAL.DTO.Order, App.BLL.DTO.Order>(mapper))
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _uow = uoW;
    }

    
    /*
     
     Here you can add custom method for services to use. Don't forget to return mapped result.
     
     
    public async Task<IEnumerable<App.BLL.DTO.Order>> GetAllOrderIncludingEverythingAsync()
    {
        var Orders = await Repository.GetAllOrderIncludingEverythingAsync();
        return Orders.Select(order => _mapper.Map<App.BLL.DTO.Order>(order));
    }

    public async Task<Order> AddNewOrder(Order order)
    {
        
        foreach (var item in order.OrderItemsCollection!)
        {
            if (item.KeyboardId == default)
            {
                item.Part = null;
                item.Price = (await _uow.Parts.FirstOrDefaultAsync(item.PartId.Value))!.Price * item.Quantity;
            }

            if (item.PartId == default)
            {
                item.Keyboard = null;
                item.Price = (await _uow.Keyboards.FirstOrDefaultAsync(item.KeyboardId.Value))!.Price * item.Quantity;
            }
        }
        
        order.OrderDate = DateTime.UtcNow;

        // var added = new App.DAL.DTO.Order()
        // {
        //     AppUserId = order.AppUserId,
        //     OrderItemsCollection = x.Select(item => _mapper.Map<App.DAL.DTO.OrderItemForOrder>(item)),
        //     OrderStatus = App.DAL.DTO.HelperEnums.OrderStatus.Pending,
        //     OrderDate = DateTime.Now,
        //     OrderNumber = order.OrderNumber,
        //     firstName = order.firstName,
        //     lastName = order.lastName,
        //     
        // };

        var result = Repository.Add(_mapper.Map<App.DAL.DTO.Order>(order));

        return _mapper.Map<App.BLL.DTO.Order>(result);
    }
    */
}