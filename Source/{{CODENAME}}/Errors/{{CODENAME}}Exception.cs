using System;

namespace {{CODENAME}}.Errors
{
    public class {{CODENAME}}Exception : Exception
    {
        public {{CODENAME}}Exception(string message) : base(message) { }
        public {{CODENAME}}Exception(string message, Exception innerException) : base(message, innerException) { }
    }
}
