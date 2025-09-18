using System.Text.Json.Serialization;

namespace Altora.Model
{
    /// <summary>
    /// Altora Course
    /// </summary>
    public class AltoraCourse
    {
        /// <summary>
        /// Course ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of course
        /// </summary>
        [JsonPropertyName("Course_Name")]
        public string? CourseName { get; set; }
    }
}
