using System.ComponentModel.DataAnnotations;
using App.DAL.DTO.HelperEnums;
using App.DAL.DTO.Identity;
using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class Order : IDomainEntityId
{
    [DataType(DataType.Date)] public DateTime OrderDate { get; set; }

    public int OrderNumber { get; set; } = new();

    public OrderStatus OrderStatus { get; set; } = default!;

    public Guid? AppUserId { get; set; }
    public AppUser? AppUser { get; set; } = default!;
    public Guid Id { get; set; }
    
    public string firstName { get; set; } = default!;

    public string lastName { get; set; } = default!;

    public string email { get; set; } = default!;

    public int phoneNumber { get; set; } = default!;

    public string addressLine { get; set; } = default!;

    public string city { get; set; } = default!;

    public string state { get; set; } = default!;

    public int zipCode { get; set; } = default!;
}