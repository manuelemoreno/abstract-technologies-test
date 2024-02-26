using System;

namespace AwesomeFruits.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
}