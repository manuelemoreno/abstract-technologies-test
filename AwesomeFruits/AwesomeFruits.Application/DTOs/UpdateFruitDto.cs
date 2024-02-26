using System;

namespace AwesomeFruits.Application.DTOs;

public class UpdateFruitDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}