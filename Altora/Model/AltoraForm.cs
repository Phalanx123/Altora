using System.Text.Json.Serialization;

namespace Altora.Model
{
    /// <summary>
    /// Altora Forms
    /// </summary>
    public class AltoraForm
    {
        /// <summary>
        /// Form ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of form
        /// </summary>
        [JsonPropertyName("Form_Name")]
        public string? FormName { get; set; }
    }
}
