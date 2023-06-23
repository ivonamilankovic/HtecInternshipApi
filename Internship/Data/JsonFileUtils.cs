using System.Text.Json.Serialization;
using System.Text.Json;

namespace Internship.Data
{
    public static class JsonFileUtils
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

        public static void MakeFile(object obj, string fileName)
        {
           var jsonString = JsonSerializer.Serialize(obj, _options);
           File.WriteAllText(fileName, jsonString);
        }
    }
}
