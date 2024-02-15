using Newtonsoft.Json;
using System.IO;

namespace DayBook
{
    public class DeSerializer
    {
        public void Serialize<T>(T obj, string filePath)
        {
            string json = JsonConvert.SerializeObject(obj);
            File.WriteAllText(filePath, json);
        }

        public T Deserialize<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(json);
            }
            else return default;
        }
    }
}
