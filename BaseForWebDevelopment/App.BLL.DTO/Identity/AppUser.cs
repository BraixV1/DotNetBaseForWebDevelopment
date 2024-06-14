using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;
using Microsoft.AspNetCore.Identity;

namespace App.BLL.DTO.Identity;

public class AppUser
{
    [MinLength(1)]
    [MaxLength(64)]
    public string FirstName { get; set; } = default!;

    [MinLength(1)]
    [MaxLength(64)]
    public string LastName { get; set; } = default!;
    
    public string Email { get; set; } = default!;
    
    public Guid Id { get; set; }
    
}