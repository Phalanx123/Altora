using System.Text.Json.Serialization;

namespace Altora.Model
{
    /// <summary>
    /// Altora Worker Acknowledgement
    /// </summary>
    public class AltoraWorkerAcknowledgement
    {
        /// <summary>
        /// Ackowledgement Id
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
        public DateTime? AcknowledgedTime { get; set; }
    }
}
