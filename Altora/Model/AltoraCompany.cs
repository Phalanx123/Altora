using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Altora.Model
{
    public class AltoraCompany
    {
        /// <summary>
        /// Company ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Trading Name
        /// </summary>
        [JsonPropertyName("Trading_Name")]
        public string? Name { get; set; }

        /// <summary>
        /// Active
        /// </summary>
        public int Active { get; set; }
    }
}
