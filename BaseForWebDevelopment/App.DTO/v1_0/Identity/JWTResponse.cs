namespace App.DTO.v1_0.Identity;

public class JwtResponse
{
    public string Jwt { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string Email { get; set; } = default!;

    
    public string UserId { get; set; } = default!;

    public IEnumerable<string> Roles { get; set; } = default!;
    
}