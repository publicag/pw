using Newtonsoft.Json;

namespace PWKlient.Config
{
    public class TransferFile
    {
        public TransferFile()
        {
        }

        public TransferFile(string name, double size)
        {
            Name = name;
            Size = size;
        }

        [JsonProperty("nazwa")]
        public string Name { get; set; }
        [JsonProperty("rozmiar")]
        public double Size { get; set; }
    }

}
