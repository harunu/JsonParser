using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

public static class AttributeFirstJsonTransformer
{
    public static Stream Transform(Stream source)
    {
        IAttributeFirstJsonTransformer transformer = new AttributeFirstJsonTransformerImpl(new JsonWriterStrategy());
        return transformer.Transform(source);
    }
}

public interface IAttributeFirstJsonTransformer
{
    Stream Transform(Stream source);
}

public class AttributeFirstJsonTransformerImpl : IAttributeFirstJsonTransformer
{
    private readonly IJsonWriterStrategy _jsonWriterStrategy;

    public AttributeFirstJsonTransformerImpl(IJsonWriterStrategy jsonWriterStrategy)
    {
        _jsonWriterStrategy = jsonWriterStrategy;
    }
    public Stream Transform(Stream source)
    {
        var output = new MemoryStream();
        using (var streamReader = new StreamReader(source))
        using (var streamWriter = new StreamWriter(output, leaveOpen: true))
        {
            var jsonReader = new JsonTextReader(streamReader);
            var jsonWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.None };

            try
            {
                JToken jsonStructure = JToken.ReadFrom(jsonReader);
                WriteJsonStructure(jsonWriter, jsonStructure);
                jsonWriter.Flush();
            }
            catch (JsonReaderException)
            {
                throw new InvalidOperationException("Invalid JSON document.");
            }
        }

        output.Seek(0, SeekOrigin.Begin);
        return output;
    }

    private void WriteJsonStructure(JsonWriter jsonWriter, JToken jsonStructure)
    {
        switch (jsonStructure.Type)
        {
            case JTokenType.Object:
                JObject obj = (JObject)jsonStructure;
                _jsonWriterStrategy.WriteObjectWithAttributesFirst(jsonWriter, obj);
                break;

            case JTokenType.Array:
                JArray array = (JArray)jsonStructure;
                _jsonWriterStrategy.WriteArray(jsonWriter, array);
                break;

            default:
                throw new InvalidOperationException("Invalid JSON document.");
        }
    }
}

public interface IJsonWriterStrategy
{
    void WriteObjectWithAttributesFirst(JsonWriter jsonWriter, JObject obj);
    void WriteArray(JsonWriter jsonWriter, JArray array);
}

public class JsonWriterStrategy : IJsonWriterStrategy
{
    public void WriteObjectWithAttributesFirst(JsonWriter jsonWriter, JObject obj)
    {
        jsonWriter.WriteStartObject();

        WriteAttributes(jsonWriter, obj);
        WriteNestedObjectsAndArrays(jsonWriter, obj);

        jsonWriter.WriteEndObject();
    }

    private void WriteAttributes(JsonWriter jsonWriter, JObject obj)
    {
        foreach (var property in obj.Properties())
        {
            if (property.Value.Type != JTokenType.Object && property.Value.Type != JTokenType.Array)
            {
                property.WriteTo(jsonWriter, new JsonConverter[0]);
            }
        }
    }

    private void WriteNestedObjectsAndArrays(JsonWriter jsonWriter, JObject obj)
    {
        foreach (var property in obj.Properties())
        {
            if (property.Value.Type == JTokenType.Object)
            {
                jsonWriter.WritePropertyName(property.Name);
                WriteObjectWithAttributesFirst(jsonWriter, (JObject)property.Value);
            }
            else if (property.Value.Type == JTokenType.Array)
            {
                jsonWriter.WritePropertyName(property.Name);
                WriteArray(jsonWriter, (JArray)property.Value);
            }
        }
    }

    public void WriteArray(JsonWriter jsonWriter, JArray array)
    {
        jsonWriter.WriteStartArray();

        foreach (var item in array)
        {
            switch (item.Type)
            {
                case JTokenType.Object:
                    WriteObjectWithAttributesFirst(jsonWriter, (JObject)item);
                    break;
                case JTokenType.Array:
                    WriteArray(jsonWriter, (JArray)item);
                    break;
                default:
                    item.WriteTo(jsonWriter, new JsonConverter[0]);
                    break;
            }
        }

        jsonWriter.WriteEndArray();
    }
}