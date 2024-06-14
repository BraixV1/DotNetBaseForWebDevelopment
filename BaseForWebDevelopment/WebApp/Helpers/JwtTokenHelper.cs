using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using App.BLL.DTO;
using App.Contracts.BLL;
using App.Contracts.DAL;
using App.DAL.EF;
using App.Domain.Identity;
using App.DTO.v1_0.Identity;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;

namespace WebApp.Helpers;


public class JwtTokenHelper
{

    private IConfiguration _configuration = new ConfigurationManager();
    


    public static string GenerateJwt(IEnumerable<Claim> claims, IConfiguration config, int expiresInSeconds)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("JWT:key")));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddSeconds(expiresInSeconds);
        var token = new JwtSecurityToken(
            issuer: config.GetValue<string>("JWT:issuer"),
            audience: config.GetValue<string>("JWT:issuer"),
            claims: claims,
            expires: expires,
            signingCredentials: signingCredentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public static async Task<string> GiveNewRefreshToken(IAppBLL bll, AppUser user, int expiresInDays)
    {
        var refreshToken = Guid.NewGuid().ToString();
        var expirationDate = DateTime.UtcNow.AddDays(expiresInDays);

        var entry = new App.BLL.DTO.AppRefreshToken()
        {
            AppUserId = user.Id,
            ExpirationDT = expirationDate,
            RefreshToken = refreshToken,
            Id = Guid.NewGuid()
        };
        
        bll.AppRefreshTokens.Add(entry);
        await bll.SaveChangesAsync();
    
        return refreshToken;
    }

    public static async Task<bool> ExpireRefreshToken(string refreshToken, IAppBLL bll)
    {
        var tokens = await bll.AppRefreshTokens.GetAllAsync();
        var found = tokens.FirstOrDefault(tokens => tokens.RefreshToken == refreshToken);
        if (found == null) return false;
        found.ExpirationDT = DateTime.UtcNow;
        bll.AppRefreshTokens.Update(found);
        await bll.SaveChangesAsync();
        return true;
    }

    public static async Task<bool> CheckJwtExpired(string token, IConfiguration configuration)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["JWT:key"]);
            var issuer = configuration["JWT:issuer"];

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = false, // Modify according to your requirements
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            SecurityToken validatedToken;
            var principal = await tokenHandler.ValidateTokenAsync(token, validationParameters);

            // Token is valid and not expired
            return true;
        }
        catch (SecurityTokenExpiredException)
        {
            // Token is expired
            return false;
        }
        catch (Exception)
        {
            // Token validation failed
            return false;
        }
    }
}