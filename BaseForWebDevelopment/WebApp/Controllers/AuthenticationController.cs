using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using App.BLL.DTO;
using App.Contracts.BLL;
using App.Domain.Identity;
using App.DTO.v1_0;
using App.DTO.v1_0.Identity;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApp.Helpers;

namespace WebApp.Controllers;

[ApiVersion( "1.0" )]
[ApiController]
[Route("api/v{version:apiVersion}/Authentication")]// Corr
public class AuthenticationController : ControllerBase
{
    private readonly IAppBLL _bll;
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private const int ExpiresInSeconds = 3600;

    public AuthenticationController(IAppBLL bll, UserManager<AppUser> usermanager, IConfiguration configuration)
    {
        _bll = bll;
        _userManager = usermanager;
        _configuration = configuration;
    }

    /// <summary>
    /// Log in method
    /// </summary>
    /// <param name="model"></param>
    /// <returns>Jwt string it manages to log in</returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("Login")]
    [ProducesResponseType<string>((int)HttpStatusCode.OK)]
    [ProducesResponseType<RestApiErrorResponse>((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType<RestApiErrorResponse>((int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<JwtResponse>> AuthoriseUser([FromBody] LoginInfo model)
    {
        // Validate email and password inputs
        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
        {
            return BadRequest(new RestApiErrorResponse {Error = "Password or Email field is empty", Status = HttpStatusCode.BadRequest});
        }
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return NotFound(new RestApiErrorResponse {Error = "Email unknown", Status = HttpStatusCode.NotFound});

        
        // Authenticate user
        var isAuthenticated = await _userManager.CheckPasswordAsync(user, model.Password);

        
        
        if (isAuthenticated)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email)
            };
            
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            
            
            
            var refreshToken = await JwtTokenHelper.GiveNewRefreshToken(_bll, user, 1);
            var response = new JwtResponse()
            {
                Jwt = JwtTokenHelper.GenerateJwt(claims, _configuration, ExpiresInSeconds),
                RefreshToken = refreshToken,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = await _userManager.GetRolesAsync(user),
                Email = model.Email,
                UserId = user.Id.ToString()
            };
            // Authentication successful
            // Generate JWT token
            return Ok(response);
        }
        else
        {
            // Authentication failed
            return Unauthorized(new RestApiErrorResponse { Error = "Wrong password or email" , Status = HttpStatusCode.Unauthorized});
        }
    }


    /// <summary>
    /// This method lets you register new account
    /// </summary>
    /// <param name="model"></param>
    /// <returns>Jwt token after account was created</returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("Register")]
    [ProducesResponseType<string>((int)HttpStatusCode.OK)]
    [ProducesResponseType<RestApiErrorResponse>((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType<RestApiErrorResponse>((int)HttpStatusCode.Conflict)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<JwtResponse>> RegisterUser([FromBody] RegisterInfo model)
    {
        // Validate email and password inputs
        if (string.IsNullOrEmpty(model.Email) || 
            string.IsNullOrEmpty(model.Password) ||
            string.IsNullOrEmpty(model.Firstname) || 
            string.IsNullOrEmpty(model.Lastname))
        {
            return BadRequest("Email and password are required.");
        }

        // Check if a user with the provided email already exists
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return Conflict(new RestApiErrorResponse { Error = "Email is already in use.", Status = HttpStatusCode.Conflict});
        }

        // Create a new user object
        var newUser = new AppUser { UserName = model.Email, Email = model.Email, FirstName = model.Firstname, LastName = model.Lastname};

        // Register the user with the provided password
        var result = await _userManager.CreateAsync(newUser, model.Password);
        if (result.Succeeded)
        {
            return await AuthoriseUser(new LoginInfo { Email = model.Email, Password = model.Password });
        }
        else
        {
            // Registration failed
            // Return validation errors
            return BadRequest(new RestApiErrorResponse() {Error = "User creation failed", Status = HttpStatusCode.BadRequest});
        }
    }

    
    /// <summary>
    /// This method lets user to refresh their jwt
    /// </summary>
    /// <param name="model"></param>
    /// <returns>New Jwt with their refresh token</returns>
    [HttpPatch]
    [Route("RefreshJwt")]
    [ProducesResponseType<JwtResponse>((int)HttpStatusCode.OK)]
    [ProducesResponseType<RestApiErrorResponse>((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType<RestApiErrorResponse>((int)HttpStatusCode.NotFound)]
    [ProducesResponseType<RestApiErrorResponse>((int)HttpStatusCode.Unauthorized)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<TokenRefreshInfo>> RefreshJwt([FromBody] TokenRefreshInfo model)
{
    try
    {
        string[] parts = model.Jwt.Split(".");
        var payload = parts[1];
        payload = payload.PadRight(payload.Length + (payload.Length * 3) % 4, '=');
        byte[] payloadBytes = Convert.FromBase64String(payload);
        string decodedPayload = Encoding.UTF8.GetString(payloadBytes);
        
        var jwtPayload = JwtPayload.Deserialize(decodedPayload);
        
        // Check if the email claim exists in the JWT payload
        if (!jwtPayload.Keys.Contains("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"))
        {
            return NotFound(new RestApiErrorResponse { Error = "JWT does not contain email claim", Status = HttpStatusCode.NotFound });
        }
    
        var userEmail = jwtPayload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"].ToString();
        
        // Find user based on email
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound(new RestApiErrorResponse { Error = "User does not exist", Status = HttpStatusCode.NotFound });
        }

        // Validate refresh token
        if (!await _bll.AppRefreshTokens.isValid(model.RefreshToken))
        {
            return BadRequest(new RestApiErrorResponse { Error = "Refresh token is not valid", Status = HttpStatusCode.BadRequest });
        }

        // Check if JWT is expired
        if (!await JwtTokenHelper.CheckJwtExpired(model.Jwt, _configuration))
        {
            return Unauthorized(new RestApiErrorResponse { Error = "JWT is invalid", Status = HttpStatusCode.Unauthorized });
        }
        
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email)
        };
        
        // Include roles in JWT claims if user has roles
        if (roles.Any())
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var updatedResponse = new TokenRefreshInfo()
        {
            Jwt = JwtTokenHelper.GenerateJwt(claims, _configuration, ExpiresInSeconds),
            RefreshToken = model.RefreshToken,
        };

        return Ok(updatedResponse);
    }
    catch (Exception ex)
    {
        // Log the exception
        Console.WriteLine("An error occurred during JWT refresh: " + ex.Message);
        return StatusCode(StatusCodes.Status500InternalServerError, new RestApiErrorResponse { Error = "Internal server error", Status = HttpStatusCode.InternalServerError });
    }
}


    /// <summary>
    /// This method will set refresh token expirery date
    /// </summary>
    /// <param name="model"></param>
    /// <returns>That it did it</returns>
    [HttpPatch]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("LogOut")]
    [ProducesResponseType<JwtResponse>((int)HttpStatusCode.OK)]
    [ProducesResponseType<RestApiErrorResponse>((int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<bool>> LogOut([FromBody] LogoutInfo model)
    {
        var result = await JwtTokenHelper.ExpireRefreshToken(model.RefreshToken, _bll);
        if (!result) return NotFound(new RestApiErrorResponse {Error = "Token couldn't be expired", Status = HttpStatusCode.NotFound});
        return Ok(result);
    }
}
