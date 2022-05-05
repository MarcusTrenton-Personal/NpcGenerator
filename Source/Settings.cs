using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace NpcGenerator
{
    public class Settings
    {
        public string ConfigurationPath { get; set; } = "...";

        public int NpcQuantity { get; set; } = 1;

        public void Save(string path)
        {
            string directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);

            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (FileStream fs = File.Create(path))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(json);
                fs.Write(info, 0, info.Length);
            }
        }

        public static Settings Load(string path)
        {
            bool fileExists = File.Exists(path);
            if(fileExists)
            {
                string text = File.ReadAllText(path);
                Settings settings = JsonConvert.DeserializeObject<Settings>(text);
                return settings;
            }
            return null;
        }
    }
}
