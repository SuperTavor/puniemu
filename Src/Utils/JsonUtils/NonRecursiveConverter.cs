using Newtonsoft.Json;



//Taken from https://github.com/JamesNK/Newtonsoft.Json/issues/719#issuecomment-2103805140
//Credit to peitschie


namespace Puniemu.Src.Utils.JsonUtils
{
    /// <summary>
    /// Converts an object to and from JSON, with guarding to prevent recursive calls into the custom converter.
    /// Useful for converters that want to slightly modify the default converter behaviour (e.g., add or remove a key).
    /// </summary>
    /// <remarks>
    /// Inspiration for this approach was taken from https://stackoverflow.com/a/76705937
    /// Further discussion of this issue is at https://github.com/JamesNK/Newtonsoft.Json/issues/719
    /// </remarks>
    /// <typeparam name="TConverter">Final derived converter this hierarchy represents.</typeparam>
    public abstract class NonRecursiveConverter<TConverter> : JsonConverter
    {
        [ThreadStatic]
        private static bool s_reading;

        [ThreadStatic]
        private static bool s_writing;

        /// <inheritdoc/>
        public override bool CanRead => !s_reading;

        /// <inheritdoc/>
        public override bool CanWrite => !s_writing;

        /// <inheritdoc/>
        public override sealed object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (s_reading)
            {
                // Protect against any changes to Newtonsoft that somehow cause concurrent access in the same thread recursively
                throw new InvalidOperationException($"Concurrent read detected on {nameof(NonRecursiveConverter<TConverter>)}");
            }

            s_reading = true;
            try
            {
                return OnReadJson(reader, objectType, existingValue, serializer);
            }
            finally
            {
                s_reading = false;
            }
        }

        /// <inheritdoc/>
        public override sealed void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (s_writing)
            {
                // Protect against any changes to Newtonsoft that somehow cause concurrent access in the same thread recursively
                throw new InvalidOperationException($"Concurrent write detected on {nameof(NonRecursiveConverter<TConverter>)}");
            }

            s_writing = true;
            try
            {
                OnWriteJson(writer, value, serializer);
            }
            finally
            {
                s_writing = false;
            }
        }

        /// <inheritdoc cref="ReadJson(JsonReader, Type, object?, JsonSerializer)"/>
        protected abstract object? OnReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer);

        /// <inheritdoc cref="WriteJson(JsonWriter, object?, JsonSerializer)"/>
        protected abstract void OnWriteJson(JsonWriter writer, object? value, JsonSerializer serializer);
    }
}
