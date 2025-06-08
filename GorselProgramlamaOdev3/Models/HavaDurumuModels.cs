using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GorselProgramlamaOdev3.Models
{
    public class HavaDurumuModels
    {
        public string Name { get; set; }
        public string Source => $"https://www.mgm.gov.tr/sunum/tahmin-klasik-5070.aspx?m={Name}&basla=1&bitir=5&rC=111&rZ=fff";
    }

    public static class TurkishCities
    {
        // Türkiye'nin 81 ili - alfabetik sıralı
        public static readonly List<string> Cities = new List<string>
        {
            "ADANA", "ADIYAMAN", "AFYONKARAHISAR", "AGRI", "AMASYA", "ANKARA", "ANTALYA", "ARTVIN",
            "AYDIN", "BALIKESIR", "BILECIK", "BINGOL", "BITLIS", "BOLU", "BURDUR", "BURSA",
            "CANAKKALE", "CANKIRI", "CORUM", "DENIZLI", "DIYARBAKIR", "EDIRNE", "ELAZIG", "ERZINCAN",
            "ERZURUM", "ESKISEHIR", "GAZIANTEP", "GIRESUN", "GUMUSHANE", "HAKKARI", "HATAY", "ISPARTA",
            "MERSIN", "ISTANBUL", "IZMIR", "KARS", "KASTAMONU", "KAYSERI", "KIRKLARELI", "KIRSEHIR",
            "KOCAELI", "KONYA", "KUTAHYA", "MALATYA", "MANISA", "K.MARAS", "MARDIN", "MUGLA",
            "MUS", "NEVSEHIR", "NIGDE", "ORDU", "RIZE", "SAKARYA", "SAMSUN", "SIIRT", "SINOP",
            "SIVAS", "TEKIRDAG", "TOKAT", "TRABZON", "TUNCELI", "SANLIURFA", "USAK", "VAN",
            "YOZGAT", "ZONGULDAK", "AKSARAY", "BAYBURT", "KARAMAN", "KIRIKKALE", "BATMAN", "SIRNAK",
            "BARTIN", "ARDAHAN", "IGDIR", "YALOVA", "KARABUK", "KILIS", "OSMANIYE", "DUZCE"
        };

        // Türkçe karakterli isimler
        public static readonly Dictionary<string, string> CityDisplayNames = new Dictionary<string, string>
        {
            { "ADANA", "Adana" },
            { "ADIYAMAN", "Adıyaman" },
            { "AFYONKARAHISAR", "Afyonkarahisar" },
            { "AGRI", "Ağrı" },
            { "AMASYA", "Amasya" },
            { "ANKARA", "Ankara" },
            { "ANTALYA", "Antalya" },
            { "ARTVIN", "Artvin" },
            { "AYDIN", "Aydın" },
            { "BALIKESIR", "Balıkesir" },
            { "BILECIK", "Bilecik" },
            { "BINGOL", "Bingöl" },
            { "BITLIS", "Bitlis" },
            { "BOLU", "Bolu" },
            { "BURDUR", "Burdur" },
            { "BURSA", "Bursa" },
            { "CANAKKALE", "Çanakkale" },
            { "CANKIRI", "Çankırı" },
            { "CORUM", "Çorum" },
            { "DENIZLI", "Denizli" },
            { "DIYARBAKIR", "Diyarbakır" },
            { "EDIRNE", "Edirne" },
            { "ELAZIG", "Elazığ" },
            { "ERZINCAN", "Erzincan" },
            { "ERZURUM", "Erzurum" },
            { "ESKISEHIR", "Eskişehir" },
            { "GAZIANTEP", "Gaziantep" },
            { "GIRESUN", "Giresun" },
            { "GUMUSHANE", "Gümüşhane" },
            { "HAKKARI", "Hakkâri" },
            { "HATAY", "Hatay" },
            { "ISPARTA", "Isparta" },
            { "MERSIN", "Mersin" },
            { "ISTANBUL", "İstanbul" },
            { "IZMIR", "İzmir" },
            { "KARS", "Kars" },
            { "KASTAMONU", "Kastamonu" },
            { "KAYSERI", "Kayseri" },
            { "KIRKLARELI", "Kırklareli" },
            { "KIRSEHIR", "Kırşehir" },
            { "KOCAELI", "Kocaeli" },
            { "KONYA", "Konya" },
            { "KUTAHYA", "Kütahya" },
            { "MALATYA", "Malatya" },
            { "MANISA", "Manisa" },
            { "K.MARAS", "Kahramanmaraş" },
            { "MARDIN", "Mardin" },
            { "MUGLA", "Muğla" },
            { "MUS", "Muş" },
            { "NEVSEHIR", "Nevşehir" },
            { "NIGDE", "Niğde" },
            { "ORDU", "Ordu" },
            { "RIZE", "Rize" },
            { "SAKARYA", "Sakarya" },
            { "SAMSUN", "Samsun" },
            { "SIIRT", "Siirt" },
            { "SINOP", "Sinop" },
            { "SIVAS", "Sivas" },
            { "TEKIRDAG", "Tekirdağ" },
            { "TOKAT", "Tokat" },
            { "TRABZON", "Trabzon" },
            { "TUNCELI", "Tunceli" },
            { "SANLIURFA", "Şanlıurfa" },
            { "USAK", "Uşak" },
            { "VAN", "Van" },
            { "YOZGAT", "Yozgat" },
            { "ZONGULDAK", "Zonguldak" },
            { "AKSARAY", "Aksaray" },
            { "BAYBURT", "Bayburt" },
            { "KARAMAN", "Karaman" },
            { "KIRIKKALE", "Kırıkkale" },
            { "BATMAN", "Batman" },
            { "SIRNAK", "Şırnak" },
            { "BARTIN", "Bartın" },
            { "ARDAHAN", "Ardahan" },
            { "IGDIR", "Iğdır" },
            { "YALOVA", "Yalova" },
            { "KARABUK", "Karabük" },
            { "KILIS", "Kilis" },
            { "OSMANIYE", "Osmaniye" },
            { "DUZCE", "Düzce" }
        };

        public static List<string> GetDisplayNames()
        {
            return CityDisplayNames.Values.OrderBy(x => x).ToList();
        }

        public static string GetApiName(string displayName)
        {
            var kvp = CityDisplayNames.FirstOrDefault(x => x.Value.Equals(displayName, StringComparison.OrdinalIgnoreCase));
            return kvp.Key ?? displayName.ToUpperInvariant();
        }

        public static string GetDisplayName(string apiName)
        {
            return CityDisplayNames.GetValueOrDefault(apiName, apiName);
        }
    }
}