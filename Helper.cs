using Newtonsoft.Json.Linq;

class LocalizationHelper
{
    private List<JObject> objects = new List<JObject>();

    public LocalizationHelper(string subfolder) {

        string localizationFolder = Path.Combine(Directory.GetCurrentDirectory(), subfolder);

        if (!Directory.Exists(localizationFolder))
            throw new Exception("localication folder not found");

        
        foreach (var file in Directory.GetFiles(localizationFolder, "*.json"))
        {
            try
            {
                var txt = File.ReadAllText(file);
                var obj = JObject.Parse(txt);
                objects.Add(obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading {file}: {ex.Message}");
            }
        }
    }

    public string[] GetLocalizedValues(string key)
    {
        List<string> values = new List<string>();
        foreach (var jsonData in objects) {
            if (jsonData.TryGetValue(key, out JToken value))
            {
                values.Add(value.ToString());
            }
        }


        return values.ToArray();
    }
  
}
