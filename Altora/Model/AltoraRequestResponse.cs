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
        public string? Status { get; set; }
        /// <summary>
        /// Message response
        /// </summary>
        public string? Message { get; set; }
    }
}
