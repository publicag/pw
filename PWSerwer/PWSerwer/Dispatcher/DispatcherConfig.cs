using Newtonsoft.Json;
using System.IO;

namespace PWSerwer.Dispatcher
{
    [JsonObject("konfiguracja")]
    public class DispatcherConfig
    {
        public DispatcherConfig(double mediumFileSize = 50, double smallFileSize = 10, double processingSpeed = 2)
        {
            SmallFileSize = smallFileSize;
            MediumFileSize = mediumFileSize;
            ProcessingSpeed = processingSpeed;
        }

        [JsonProperty("rozmiarMalegoPliku")]
        public double SmallFileSize { get; set; }
        [JsonProperty("rozmiarSredniegoPliku")]
        public double MediumFileSize { get; set; }
        [JsonProperty("szybkoscProcesowania")]
        public double ProcessingSpeed { get; set; }
        public void Serialize(string filePath)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, this);
            }
        }
        public static void Serialize(string filePath, DispatcherConfig configModel)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, configModel);
            }
        }

        public static DispatcherConfig Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<DispatcherConfig>(File.ReadAllText(json));
        }
    }
}
