using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace JsonManipulator
{
    /// <summary>
    /// Represents a helper class for JSON operations.
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Converts a JSON string to a dictionary of strings, where the keys are the property names
        /// and the values are the corresponding values in the JSON object.
        /// </summary>
        /// <param name="json">The input JSON string to be converted.</param>
        /// <returns>A dictionary of strings, where the keys are the property names and the values are the corresponding values in the JSON object.</returns>
        public static Dictionary<string, string?> GetValuesFromJsonString(this string json)
        {
            var jsonProperties = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            var values = new Dictionary<string, string?>();
            if (jsonProperties == null) return values;
#if NETSTANDARD2_0_OR_GREATER
            foreach (var prop in jsonProperties)
            {
                ExtractValuesFromJsonElement(prop.Value, values, prop.Key);
            }
#elif NET5_0_OR_GREATER
            foreach (var (key, value) in jsonProperties)
            {
                ExtractValuesFromJsonElement(value, values, key);
            }
#endif


            return values;
        }

        /// <summary>
        /// Extracts the values from a JSON element and adds them to a dictionary of strings.
        /// </summary>
        /// <param name="element">The JSON element to extract the values from.</param>
        /// <param name="values">The dictionary of strings to add the extracted values to.</param>
        /// <param name="parentKey">The key for the extracted values, constructed from the parent property names separated by colons.</param>
        private static void ExtractValuesFromJsonElement(JsonElement element, Dictionary<string, string?> values,
            string parentKey = "")
        {
            parentKey += ":";
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        var key = parentKey + property.Name;
                        ExtractValuesFromJsonElement(property.Value, values, key);
                    }

                    break;
                case JsonValueKind.Array:
                    var index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        var key = parentKey + index;
                        ExtractValuesFromJsonElement(item, values, key);
                        index++;
                    }

                    break;
                default:
                    values[parentKey.TrimEnd(':')] = element.ToString();
                    break;
            }
        }


        /// <summary>
        /// Converts a `Dictionary&lt;string, string?&gt;` to a JSON string representation.
        /// </summary>
        /// <param name="keyValuePairs">The input `Dictionary&lt;string, string?&gt;` to be converted.</param>
        /// <returns>A JSON string representation of the input dictionary.</returns>
        public static string ToJsonString(this Dictionary<string, string?> keyValuePairs)
        {
            var keysIndex = new Dictionary<string, int>();
            var rootNode = JsonNode.Parse("{}");
            foreach (var pair in keyValuePairs)
            {
                var keyElements = pair.Key.Split(':').Select(x =>
                {
                    object value = int.TryParse(x, out var result) ? result : x;
                    return value;
                }).ToArray();

                AddValueToJsonNode(rootNode!.Root, keyElements, pair.Value, keysIndex);
            }

            return rootNode!.ToJsonString(new JsonSerializerOptions
                { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        }

        /// <summary>
        /// Recursively adds values to a `JsonNode` object.
        /// </summary>
        /// <param name="node">The `JsonNode` object to add values to.</param>
        /// <param name="keyElements">The elements of the key that is being added to the node.</param>
        /// <param name="value">The value to be added to the node.</param>
        /// <param name="keysIndex">A `Dictionary&lt;string, int&gt;` that keeps track of the indexes of objects in arrays.</param>
        /// <param name="currentIndex">The current index of the key element being processed.</param>
        private static void AddValueToJsonNode(JsonNode node, object[] keyElements, string? value,
            IDictionary<string, int> keysIndex, int currentIndex = 0)
        {
            if (currentIndex + 1 != keyElements.Length)
            {
                if (keyElements[currentIndex + 1] is int)
                {
                    if (node[keyElements[currentIndex].ToString()!] is not JsonArray)
                        node[keyElements[currentIndex].ToString()!] = new JsonArray();
                    node = node[keyElements[currentIndex].ToString()!]!;
                }
                else
                {
                    if (node is not JsonArray array)
                    {
                        node[keyElements[currentIndex].ToString() ?? string.Empty] =
                            node[keyElements[currentIndex].ToString()!] ?? new JsonObject();
                        node = node[keyElements[currentIndex].ToString()!]!;
                    }
                    else
                    {
                        if (int.TryParse(keyElements[currentIndex].ToString(), out var arrayIndex))
                        {
                            if (arrayIndex >= array.Count)
                            {
                                arrayIndex = array.Count;
                                array.Add(new JsonObject());
                            }

                            node = node[arrayIndex]!;
                        }
                        else
                        {
                            var currentKey = string.Join("-", keyElements[..(currentIndex + 1)]);
                            if (!keysIndex.ContainsKey(currentKey))
                            {
                                keysIndex[currentKey] = array.Count;
                                array.Add(new JsonObject());
                            }

                            node = node[keysIndex[currentKey]]!;
                        }
                    }
                }

                currentIndex++;
                AddValueToJsonNode(node, keyElements, value, keysIndex, currentIndex);
                return;
            }

            if (node is JsonArray arr)
            {
                arr.Add(GetJsonValue(value));
            }
            else
            {
                node[keyElements[currentIndex].ToString() ?? string.Empty] = GetJsonValue(value);
            }
        }

        /// <summary>
        /// Gets the `JsonValue` object representation of a string.
        /// </summary>
        /// <param name="obj">The input string to be converted to a `JsonValue` object.</param>
        /// <returns>A `JsonValue` object that represents the input string.</returns>
        private static JsonValue? GetJsonValue(string? obj)
        {
            if (bool.TryParse(obj, out var boolValue))
                return JsonValue.Create(boolValue);
            if (int.TryParse(obj, out var intValue))
                return JsonValue.Create(intValue);
            if (decimal.TryParse(obj, out var decimalValue))
                return JsonValue.Create(decimalValue);
            if (DateTime.TryParse(obj, out var dateTime))
                return JsonValue.Create(dateTime);
            if (DateTimeOffset.TryParse(obj, out var dateTimeOffset))
                return JsonValue.Create(dateTimeOffset);
            return obj != null ? JsonValue.Create(obj) : null;
        }
    }
}