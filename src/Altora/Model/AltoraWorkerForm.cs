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
            if (Fields == null)
                return new Dictionary<string, string>();

            // Using a default label if the original is null, ensuring all keys are non-null
            return Fields.Where(field => field.Label != null) // Optionally filter out null labels
                .ToDictionary(
                    field => field.Label!,
                    field => field.Value ?? string.Empty // Providing a default value for null field values
                );
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