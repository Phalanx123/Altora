using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Altora.Model
{
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    
    public class AltoraWorker
    {
        public int? Id { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public string? Mobile { get; set; }

        public string? Phone { get; set; }
        public string? Position { get; set; }
        public string? Email { get; set; }
        public string? Dob { get; set; }
        public string? Active { get; set; }
        public string? Username { get; set; }
        public int? CompanyId { get; set; }
       
        [JsonPropertyName("Custom_Fields")]
        public Dictionary<string, string>? CustomFields { get; set; }
        public AltoraCompany? AltoraCompany { get; set; }
    }
    public class AltoraAddWorkerResponse
    {
        public bool IsSuccessful => Status == "success";

        /// <summary>
        /// Success or Error
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }
        /// <summary>
        /// Created Username
        /// </summary>
        [JsonPropertyName("username")]
        public string? Username { get; set; }
        /// <summary>
        /// System Generated password
        /// </summary>
        [JsonPropertyName("password")]
        public string? Password { get; set; }

        
    }
}
