using System.Text.Json.Serialization;

namespace Altora.Model
{
    /// <summary>
    /// Gets Worker/User Documents
    /// </summary>
    public class AltoraWorkerDocument
    {
        /// <summary>
        /// Document Issue Date
        /// </summary>
        [JsonPropertyName("Issue_Date")]
        public DateTime? IssueDate { get; set; }
        /// <summary>
        /// Document Expiry Date
        /// </summary>
        [JsonPropertyName("Expiry_Date")]
        public DateTime? ExpiryDate { get; set; }
        /// <summary>
        /// Document Number
        /// </summary>
        [JsonPropertyName("Document_Number")]
        public string? DocumentNumber { get; set; }
        /// <summary>
        /// File Location
        /// </summary>
        [JsonPropertyName("File")]
        public string? FileUrl { get; set; }
        /// <summary>
        /// Document Issuer
        /// </summary>
        [JsonPropertyName("Issuer")]
        public string? Issuer { get; set; }

    }
}
