using System.Text.Json.Serialization;

namespace Altora.Model.Responses;

/// <summary>
/// Response payload after creating a worker in Altora.
/// </summary>
public class CreateWorkerResponse
{
    /// <summary>
    /// Success is indicated by "Success", failure by "Failed".
    /// </summary>
   [JsonPropertyName("status")]
    public required string Status { get; set; } = string.Empty;
    
    public bool IsSuccess => Status.Equals("Success", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Username assigned to the newly created worker in Altora.
    /// </summary>
   [JsonPropertyName("username")]
    public string? Username { get; set; } = string.Empty;

    /// <summary>
    /// Password assigned to the newly created worker in Altora.
    /// </summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; } = string.Empty;

}