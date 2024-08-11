using Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Services
{
    public class WriteToJson
    {
        public static void WriteJson(object Object, string path)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string json = JsonSerializer.Serialize(Object,typeof(Object) ,options);
            if(!File.Exists(path))
            {
                File.Create(path).Close();
            }
            File.WriteAllText(path, json);
        }

        public static FilterData ReadJson(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<FilterData>(json);
        }
    }
}
