using System;

namespace AwesomeFruits.Application.DTOs;

public class FruitDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime LastUpdatedAtUtc { get; set; }
}