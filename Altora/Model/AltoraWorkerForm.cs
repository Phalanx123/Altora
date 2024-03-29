﻿using System.Text.Json.Serialization;

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
        public IEnumerable<AltoraWorkerFormItem>? Fields { get; set; }
    }

    /// <summary>
    /// Headings and Values of form
    /// </summary>
    public class AltoraWorkerFormItem
    {
        /// <summary>
        /// Form Heading
        /// </summary>
        [JsonPropertyName("label")]
        public string? Heading { get; set; }

        /// <summary>
        /// Form Value
        /// </summary>
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}
