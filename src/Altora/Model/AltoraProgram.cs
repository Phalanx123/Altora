using System.Text.Json.Serialization;

namespace Altora.Model
{
    /// <summary>
    /// Altora Programs
    /// </summary>
    public class AltoraProgram
    {
        /// <summary>
        /// Program ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of Program
        /// </summary>
        [JsonPropertyName("Program_Name")]
        public string? ProgramName { get; set; }
    }
}
