namespace Altora.Model
{
    /// <summary>
    /// Contains Worker Form Items
    /// </summary>
    public class AltoraWorkerForm
    {
        /// <summary>
        /// A list of fields
        /// </summary>
        public List<FieldItem>? Fields { get; set; }
        
        // Method to convert to Dictionary
        public Dictionary<string, string> ToDictionary()
        {
            return Fields != null ? Fields.ToDictionary(field => field.Label, field => field.Value) : new Dictionary<string, string>();
        }
        
        /// <summary>
        /// Form ID
        /// </summary>
        public int FormID { get; set; }
        
    }
    
    public class FieldItem
    {
        public string? Label { get; set; }
        public string? Value { get; set; }

        // Constructor and any other necessary methods can go here
    }
    
}
