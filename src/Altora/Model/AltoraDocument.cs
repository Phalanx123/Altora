using System.Text.Json.Serialization;

namespace Altora.Model
{
    /// <summary>
    /// Altora Document
    /// </summary>
    public class AltoraDocument
    {
        /// <summary>
        /// Document ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of document
        /// </summary>
        [JsonPropertyName("Document_Name")]
        public string? DocumentName { get; set; }
    }
}
