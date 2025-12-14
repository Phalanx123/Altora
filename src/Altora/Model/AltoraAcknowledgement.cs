using System.Text.Json.Serialization;

namespace Altora.Model
{
    /// <summary>
    /// Altora Acknowledgment
    /// </summary>
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public class AltoraAcknowledgement
    {
        /// <summary>
        /// Acknowledgement ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of Acknowledgement
        /// </summary>
        [JsonPropertyName("Acknowledgement_Name")]
        public string? AcknowledgementName { get; set; }
    }
}
