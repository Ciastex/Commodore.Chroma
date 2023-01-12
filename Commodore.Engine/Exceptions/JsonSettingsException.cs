using System;

namespace Commodore.Engine.Exceptions
{
    public class JsonSettingsException : Exception
    {
        public string Key { get; }
        public bool IsJsonFailure { get; }

        public JsonSettingsException(string message, string key, bool isJsonFailure, Exception innerException) : base(message, innerException)
        {
            Key = key;
            IsJsonFailure = isJsonFailure;
        }

        public JsonSettingsException(string message, string key, bool isJsonFailure) : this(message, key, isJsonFailure, null) { }
    }
}
