using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Altora.Model
{
    /// <summary>
    /// Altora Acknowledgment
    /// </summary>
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
