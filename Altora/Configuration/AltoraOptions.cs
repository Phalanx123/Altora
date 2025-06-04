using System.ComponentModel.DataAnnotations;

namespace Altora.Configuration;

public class AltoraOptions
{
    [Required]
    public required string ClientId { get; set; } 
    [Required]
    public required string ApiKey { get; set; }
    [Required]
    public required string ApiSecret { get; set; } 
}