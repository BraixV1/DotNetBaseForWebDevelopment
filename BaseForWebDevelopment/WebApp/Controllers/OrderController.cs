/*
This is an example controller
 
 
 
 
using System.Net;
using System.Security.Claims;
using App.Contracts.BLL;
using App.Domain.Identity;
using App.DTO.v1_0;
using App.DTO.v1_0.HelperEnums;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers;

namespace WebApp.Controllers;


[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/Order")]
public class OrderController : Controller
{
    private readonly IAppBLL _bll;
    private readonly UserManager<AppUser> _userManager;
    private readonly PublicDTOBllMapper<App.DTO.v1_0.Order, App.BLL.DTO.Order> _mapper;

    public OrderController(IAppBLL bll, UserManager<AppUser> userManager, IMapper mapper)
    {
        _bll = bll;
        _userManager = userManager;
        _mapper = new PublicDTOBllMapper<Order, App.BLL.DTO.Order>(mapper);
    }
    
    
    /// <summary>
    /// Returns all Orders from the database
    /// </summary>
    /// <returns>List of orders</returns>
    [HttpGet]
    [Route("AllOrders")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(IEnumerable<Order>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    [Produces("application/json")]
    [Consumes("application/json")]

    public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
    {
        var userEmailClaim = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        if (userEmailClaim == null)
        {
            return NotFound(new RestApiErrorResponse {Error = "JWT does not contain email claim", Status = HttpStatusCode.NotFound});
        }
    
        var userEmail = userEmailClaim.Value;
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound(new RestApiErrorResponse {Error = "User does not exist that was inside JWT", Status = HttpStatusCode.NotFound});
        }
        
        var rolesClaim = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        if (rolesClaim.FirstOrDefault(claim => claim.Value == "Admin") == null)
        {
            return NotFound(new RestApiErrorResponse { Error = "JWT does not have required claims", Status = HttpStatusCode.NotFound });
        }
        
        
        var found = (await _bll.Orders.GetAllOrderIncludingEverythingAsync())
            .Select(order => _mapper.Map(order));
      
        return Ok(found);
    }
    
    /// <summary>
    /// Returns specific order from the database
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Specific order or not found</returns>
    [HttpGet]
    [Route("GetOrder/{id}")]
    [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]

    public async Task<ActionResult<Order>> GetOrder(Guid id)
    {
        
        if (id == default) return BadRequest(id);
        var found = (await _bll.Orders.GetAllOrderIncludingEverythingAsync()).FirstOrDefault(kb => kb.Id == id);

        if (found == null) return NotFound(new RestApiErrorResponse {Error = $"Order with id: {id} not found", Status = HttpStatusCode.NotFound});
        return Ok(found);
    }
    
    
    /// <summary>
    /// Get jwt owners orders
    /// </summary>
    /// <param name="id"></param>
    /// <returns>list of orders</returns>
    [HttpGet]
    [Route("GetUserOrders")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]

    public async Task<ActionResult<Keyboard>> GetUserOrder()
    {
        
        var userEmailClaim = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        if (userEmailClaim == null)
        {
            return NotFound(new RestApiErrorResponse {Error = "JWT does not contain email claim", Status = HttpStatusCode.NotFound});
        }
    
        var userEmail = userEmailClaim.Value;
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound(new RestApiErrorResponse {Error = "User does not exist that was inside JWT", Status = HttpStatusCode.NotFound});
        }
        
        var found = (await _bll.Orders.GetAllOrderIncludingEverythingAsync()).Where(order => order.AppUserId == user.Id);
        
        return Ok(found);
    }
    
    

    /// <summary>
    /// Add new order to the database
    /// </summary>
    /// <returns>Order that was made</returns>
    [HttpPost]
    [Route("AddOrder")]
    [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.Conflict)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<Order>> AddNewOrder([FromBody] Order model)
    {
        try
        {
            // Input Validation
            if (model.OrderNumber < 0)
            {
                return BadRequest(new RestApiErrorResponse
                {
                    Error = "Order number must be greater than or equal to 0",
                    Status = HttpStatusCode.BadRequest
                });
            }

            if (string.IsNullOrEmpty(model.firstName) ||
                string.IsNullOrEmpty(model.lastName) ||
                string.IsNullOrEmpty(model.email) ||
                int.IsNegative(model.phoneNumber) ||
                string.IsNullOrEmpty(model.addressLine) ||
                string.IsNullOrEmpty(model.city) ||
                string.IsNullOrEmpty(model.state) ||
                int.IsNegative(model.zipCode))
            {
                return BadRequest(new RestApiErrorResponse()
                {
                    Error = "One or more order details is emppty please fulfill all the fields",
                    Status = HttpStatusCode.BadRequest
                });
            }
            
            var added = await _bll.Orders.AddNewOrder(_mapper.Map(model));

            await _bll.SaveChangesAsync();
            
            

            return Ok(added);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            return StatusCode((int)HttpStatusCode.InternalServerError, new RestApiErrorResponse
            {
                Error = "An error occurred while processing the request please contact development",
                Status = HttpStatusCode.InternalServerError
            });
        }
    }

    /// <summary>
    /// Update order status
    /// </summary>
    /// <returns>Updated order</returns>
    [HttpPatch]
    [Route("UpdateOrderStatus")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.Unauthorized)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<Order>> UpdateOrder([FromBody] Order model)
    {
        
        var userEmailClaim = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        if (userEmailClaim == null)
        {
            return NotFound(new RestApiErrorResponse {Error = "JWT does not contain email claim", Status = HttpStatusCode.NotFound});
        }
    
        var userEmail = userEmailClaim.Value;
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound(new RestApiErrorResponse {Error = "User does not exist that was inside JWT", Status = HttpStatusCode.NotFound});
        }
        
        var rolesClaim = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        if (rolesClaim.Count == 0)
        {
            return NotFound(new RestApiErrorResponse { Error = "JWT does not contain any role claims", Status = HttpStatusCode.NotFound });
        }

        if (rolesClaim.FirstOrDefault(claim => claim.Value == "Admin") == null)
        {
            return NotFound(new RestApiErrorResponse { Error = "JWT does not have required claims", Status = HttpStatusCode.NotFound });
        }

        if (model.Id == default)
        {
            return BadRequest(new RestApiErrorResponse()
                { Error = "Id field empty", Status = HttpStatusCode.NotFound });
        }

        var found = await _bll.Orders.FirstOrDefaultAsync(model.Id);

        if (found == null)
        {
            return NotFound(new RestApiErrorResponse()
                { Error = $"Order with id: {model.Id} not found", Status = HttpStatusCode.NotFound });
        }

        switch (model.OrderStatus)
        {
            case OrderStatus.Pending:
                found.OrderStatus = App.BLL.DTO.HelperEnums.OrderStatus.Pending;
                break;
            case OrderStatus.Done:
                found.OrderStatus = App.BLL.DTO.HelperEnums.OrderStatus.Done;
                break;
            case OrderStatus.Canceled:
                found.OrderStatus = App.BLL.DTO.HelperEnums.OrderStatus.Canceled;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var added = _bll.Orders.Update(found);
        await _bll.SaveChangesAsync();

        return Ok(_mapper.Map(added));
    }
    
    /// <summary>
    /// Delete Order from database
    /// </summary>
    /// <returns>true or false depending on if it did its job</returns>
    [HttpDelete]
    [Route("DeleteOrder/{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<bool>> DeleteOrder(Guid id)
    {
        var userEmailClaim = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        if (userEmailClaim == null)
        {
            return NotFound(new RestApiErrorResponse {Error = "JWT does not contain email claim", Status = HttpStatusCode.NotFound});
        }
    
        var userEmail = userEmailClaim.Value;
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound(new RestApiErrorResponse {Error = "User does not exist that was inside JWT", Status = HttpStatusCode.NotFound});
        }
        
        var rolesClaim = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        if (rolesClaim.Count == 0)
        {
            return NotFound(new RestApiErrorResponse { Error = "JWT does not contain any role claims", Status = HttpStatusCode.NotFound });
        }

        if (rolesClaim.FirstOrDefault(claim => claim.Value == "Admin") == null)
        {
            return NotFound(new RestApiErrorResponse { Error = "JWT does not have required claims", Status = HttpStatusCode.NotFound });
        }
        
        
        if (id == default) return BadRequest(new RestApiErrorResponse() { Error = "Id field not fulfilled", Status = HttpStatusCode.BadRequest});
        var found = await _bll.Orders.FirstOrDefaultAsync(id);
        if (found == null) return NotFound(new RestApiErrorResponse() { Error = $"item with id: {id} was not found", Status = HttpStatusCode.NotFound});
        await _bll.Orders.RemoveAsync(found);
        await _bll.SaveChangesAsync();
        return Ok(true);
    }


}
*/