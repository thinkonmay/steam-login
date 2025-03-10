using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

class LocalizationHelper
{
    public static Dictionary<string, string> GetLocalizedValues(string key)
    {
        string localizationFolder = Path.Combine(Directory.GetCurrentDirectory(), "localization");
        Dictionary<string, string> values = new Dictionary<string, string>();

        if (!Directory.Exists(localizationFolder))
        {
            Console.WriteLine("Localization folder not found.");
            return values;
        }

        foreach (var file in Directory.GetFiles(localizationFolder, "*.json"))
        {
            Console.WriteLine(file);
            string language = Path.GetFileNameWithoutExtension(file);


            try
            {
                var jsonData = JObject.Parse(File.ReadAllText(file));
                if (jsonData.TryGetValue(key, out JToken value))
                {
                    values[language] = value.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading {file}: {ex.Message}");
            }
        }

        return values;
    }
  
}
