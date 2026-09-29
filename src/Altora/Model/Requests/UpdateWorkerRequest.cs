using System.Text.Json.Serialization;
using Altora.Converters;

namespace Altora.Model.Requests;

/// <summary>
/// Request payload for <c>PATCH /v1/{clientid}/users/{userid}</c>.
/// Altora only changes the fields that are sent, so every field is optional: leave a field null to keep
/// the value Altora already holds.
/// </summary>
public class UpdateWorkerRequest
{
    /// <summary>
    /// The Altora user id being updated. It goes into the URL, never into the body.
    /// </summary>
    [JsonIgnore]
    public required int Id { get; set; }

    /// <summary>
    /// The worker's first name.
    /// </summary>
    [JsonPropertyName("Firstname")]
    public string? FirstName { get; set; }

    /// <summary>
    /// The worker's last name.
    /// </summary>
    [JsonPropertyName("Lastname")]
    public string? LastName { get; set; }

    /// <summary>
    /// The worker's email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Mobile phone number.
    /// </summary>
    public string? Mobile { get; set; }

    /// <summary>
    /// Date of birth.
    /// </summary>
    [JsonConverter(typeof(DateOnlyConverter))]
    [JsonPropertyName("Dob")]
    public DateOnly? DateOfBirth { get; set; }

    /// <summary>
    /// Role, trade or job title.
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Whether the user may still log in. Altora carries it as "1" or "0".
    /// </summary>
    [JsonConverter(typeof(NullableJsonBoolConverter))]
    public bool? Active { get; set; }

    /// <summary>
    /// The worker's company object id. Used when the Company module is enabled.
    /// </summary>
    public int? CompanyId { get; set; }

    /// <summary>
    /// The worker's company name. Used only when the Company module is not enabled.
    /// </summary>
    [JsonPropertyName("Company_Name")]
    public string? CompanyName { get; set; }

    /// <summary>
    /// Custom fields to write alongside the built-in ones.
    /// </summary>
    public Dictionary<string, string>? CustomFields { get; set; }
}
