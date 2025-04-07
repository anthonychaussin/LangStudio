using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace LangStudio.Helpers
{
    public static class JsonHelper
    {
        public static Dictionary<string, string> FlattenJson(JObject jObject, string prefix = "")
        {
            var result = new ConcurrentDictionary<string, string>();

            jObject.Properties().ToList().ForEach(property =>
            {
                var key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";

                if (property.Value.Type == JTokenType.Object)
                {
                    FlattenJson((JObject)property.Value, key).ToList().ForEach(kvp =>
                    {
                        result[kvp.Key] = kvp.Value;
                    });
                }
                else
                {
                    result[key] = property.Value.ToString();
                }
            });

            return result.OrderBy(r => r.Key).ToDictionary();
        }

        public static void SetJsonValue(JObject root, string[] path, string value)
        {
            JToken current = root;

            for (int i = 0; i < path.Length; i++)
            {
                var key = path[i];

                if (i == path.Length - 1)
                {
                    current![key] = value;
                }
                else
                {
                    if (current![key] == null || current[key]!.Type != JTokenType.Object)
                    {
                        current[key] = new JObject();
                    }

                    current = current[key]!;
                }
            }
        }
    }
}