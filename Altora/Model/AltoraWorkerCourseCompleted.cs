using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Altora.Model
{
    /// <summary>
    /// Courses the worker has completed
    /// </summary>
    public class AltoraWorkerCourseCompleted
    {
        /// <summary>
        /// Course ID
        /// </summary>
        [JsonPropertyName("Course_Id")]
        public string? CourseId{ get; set; }
        /// <summary>
        /// Course Name
        /// </summary>
        [JsonPropertyName("Course_Name")]
        public string? CourseName { get; set; }
        /// <summary>
        /// Course Issue Date
        /// </summary>
        [JsonPropertyName("Issue_Date")]
        public DateTime? IssueDate { get; set; }
        /// <summary>
        /// Course Expiry Date
        /// </summary>
        [JsonPropertyName("Expiry_Date")]
        public DateTime? ExpiryDate { get; set; }
        /// <summary>
        /// Certificate Number
        /// </summary>
        [JsonPropertyName("Certificate_No")]
        public string? CertificateNo { get; set; }
    }
}
