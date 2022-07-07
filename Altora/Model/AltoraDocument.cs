using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
