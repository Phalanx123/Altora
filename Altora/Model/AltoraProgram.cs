using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
