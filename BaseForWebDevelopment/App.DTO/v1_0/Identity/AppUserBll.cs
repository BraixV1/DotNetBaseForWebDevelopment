namespace App.DTO.v1_0.Identity;

public class AppUserBll
{
    public Guid Id { get; set; }

    public string Email { get; set; } = default!;
    
    public string FirstName { get; set; } = default!;
    
    public string LastName { get; set; } = default!;
}