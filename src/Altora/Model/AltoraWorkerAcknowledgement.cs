using System.Text.Json.Serialization;
using Altora.Converters;

namespace Altora.Model
{
    
    /// <summary>
    /// Altora Worker Acknowledgement
    /// </summary>
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public class AltoraWorkerAcknowledgement
    {
        /// <summary>
        /// Acknowledgement Id
        /// </summary>
        [JsonPropertyName("Acknowledgement_Number")]
        public string? AcknowledgementNumber { get; set; }
        /// <summary>
        /// File URL
        /// </summary>
        [JsonPropertyName("File")]
        public string? FileUrl { get; set; }
        /// <summary>
        /// Worker who acknowledged
        /// </summary>
        [JsonPropertyName("Acknowledged_By")]
        public string? AcknowledgedBy { get; set; }
        /// <summary>
        /// Time Acknowledged
        /// </summary>
        [JsonPropertyName("Time_Acknowledged")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? AcknowledgedTime { get; set; }
        
        /// <summary>
        /// Acknowledgement ID
        /// </summary>
        public int AcknowledgementId { get; set; }
    }
}
