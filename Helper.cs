using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

class LocalizationHelper
{
    public static string[] GetLocalizedValues(string key)
    {
        string localizationFolder = Path.Combine(Directory.GetCurrentDirectory(), "localization");
        List<string> values = new List<string>();

        if (!Directory.Exists(localizationFolder))
        {
            Console.WriteLine("Localization folder not found.");
            return values.ToArray();
        }

        foreach (var file in Directory.GetFiles(localizationFolder, "*.json"))
        {
            // string language = Path.GetFileNameWithoutExtension(file);
            try
            {
                var jsonData = JObject.Parse(File.ReadAllText(file));
                if (jsonData.TryGetValue(key, out JToken value))
                {
                    values.Add(value.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading {file}: {ex.Message}");
            }
        }


        return values.ToArray();
    }
  
}
