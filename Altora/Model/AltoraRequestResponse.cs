using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altora.Model
{
    /// <summary>
    /// Responses from Put operations
    /// </summary>
    public class AltoraRequestResponse
    {
        /// <summary>
        /// Either success or error
        /// </summary>
        public string? Status { get; set;}
        /// <summary>
        /// Message response
        /// </summary>
        public string? Message { get; set; }
    }
}
