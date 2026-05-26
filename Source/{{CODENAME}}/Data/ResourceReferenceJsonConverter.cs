using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace {{CODENAME}}.Data
{
    public class ResourceReferenceJsonConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
                return false;

            if (typeToConvert.GetGenericTypeDefinition() != typeof(ResourceReference<>))
                return false;

            return true;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type itemType = typeToConvert.GetGenericArguments()[0];
            Type converterType = typeof(ResourceReferenceJsonConverterInner<>).MakeGenericType(itemType);
            return (JsonConverter)Activator.CreateInstance(converterType);
        }

        private class ResourceReferenceJsonConverterInner<[MustBeVariant] T> : JsonConverter<ResourceReference<T>> where T : Resource
        {
            public override ResourceReference<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                string name = reader.GetString();
                return new ResourceReference<T>(name);
            }

            public override void Write(Utf8JsonWriter writer, ResourceReference<T> value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.Name);
            }
        }
    }
}
