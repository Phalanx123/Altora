using System.Text.Json.Serialization;

namespace Altora.Model
{
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
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
