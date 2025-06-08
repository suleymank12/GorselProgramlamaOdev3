using Newtonsoft.Json;

namespace GorselProgramlamaOdev3.Models
{
    public class Gorev
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("baslik")]
        public string Baslik { get; set; }

        [JsonProperty("detay")]
        public string Detay { get; set; }

        [JsonProperty("tarih")]
        public string Tarih { get; set; }

        [JsonProperty("saat")]
        public string Saat { get; set; }

        [JsonProperty("yapildi")]
        public bool Yapildi { get; set; }

        public Gorev()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Gorev(string baslik, string detay, string tarih, string saat, bool yapildi = false)
        {
            Id = Guid.NewGuid().ToString();
            Baslik = baslik;
            Detay = detay;
            Tarih = tarih;
            Saat = saat;
            Yapildi = yapildi;
        }
    }
}