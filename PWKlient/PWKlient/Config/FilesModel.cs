using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace PWKlient.Config
{
    public class FilesModel
    {
        public FilesModel(ICollection<TransferFile> files)
        {
            Files = files;
        }

        [JsonProperty("pliki")]
        public ICollection<TransferFile> Files { get; set; }

        public void Serialize(string filePath)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static void Serialize(string filePath, FilesModel configModel)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, configModel);
            }
        }

        public static FilesModel Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<FilesModel>(File.ReadAllText(json));
        }
    }
}
