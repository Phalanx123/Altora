using System.ComponentModel;
using System.Text.Json.Serialization;
using Altora.Converters;
using DateOnlyConverter = Altora.Converters.DateOnlyConverter;

namespace Altora.Model.Requests;

/// <summary>
/// Request payload for creating a worker in Altora.
/// </summary>
public class CreateWorkerRequest
{
    /// <summary>
    /// The worker’s first name.
    /// </summary>
  [JsonPropertyName("Firstname")]
    public required string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// The worker’s last name.
    /// </summary>
    [JsonPropertyName("Lastname")]
    public required string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The worker’s email address.
    /// </summary>
    public string? Email { get; set; } = string.Empty;

    /// <summary>
    /// Optional phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Mobile phone number.
    /// </summary>
    public string? Mobile { get; set; }
    
    /// <summary>
    /// Date of Birthday.
    /// </summary>
    [JsonConverter(typeof(DateOnlyConverter))]
    [JsonPropertyName("Dob")]
    public DateOnly? DateOfBirth { get; set; }
    

    /// <summary>
    /// Optional role, trade, or job title.
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Company Id
    /// </summary>
    public int? CompanyId { get; set; }
    
    /// <summary>
    /// Custom fields to include with the worker.
    /// </summary>
    public Dictionary<string, string>? CustomFields { get; set; }
}