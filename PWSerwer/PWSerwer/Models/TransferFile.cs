using Newtonsoft.Json;

namespace PWSerwer.Models
{
    public class TransferFile
    {
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
