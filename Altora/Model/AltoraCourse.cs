using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
