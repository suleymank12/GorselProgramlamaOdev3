using System.Collections.ObjectModel;
using System.Text.Json;
using GorselProgramlamaOdev3.Models;
using System.Diagnostics;
using System.ComponentModel;

namespace GorselProgramlamaOdev3;

public partial class HavaDurumu : ContentPage, INotifyPropertyChanged
{
    string fileName = Path.Combine(FileSystem.Current.AppDataDirectory, "hava_durumu.json");
    private const string THEME_FILE = "theme_settings.json";
    private readonly string _themeFilePath;
    private bool _isDarkTheme;

    public ObservableCollection<HavaDurumuModels> Sehirler { get; set; } = new ObservableCollection<HavaDurumuModels>();

    public HavaDurumu()
    {
        InitializeComponent();
        _themeFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, THEME_FILE);

        listCity.ItemsSource = Sehirler;

        // Collection de�i�ikliklerini dinle
        Sehirler.CollectionChanged += (s, e) => UpdateEmptyState();

        // Tema de�i�iklik mesaj�n� dinle
        MessagingCenter.Subscribe<Ayarlar, bool>(this, "ThemeChanged", OnThemeChanged);

        LoadThemeSettings();
        BindingContext = this;
    }

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            _isDarkTheme = value;
            OnPropertyChanged();
            UpdateThemeColors();
        }
    }

    // Tema Renkleri
    public Color PageBackgroundColor => IsDarkTheme ? Colors.Black : Colors.White;
    public Color RefreshBackgroundColor => IsDarkTheme ? Colors.Black : Color.FromArgb("#F0F0F0");
    public Color HeaderBackgroundColor => IsDarkTheme ? Color.FromArgb("#333333") : Color.FromArgb("#2196F3");
    public Color CardBackgroundColor => IsDarkTheme ? Color.FromArgb("#222222") : Colors.White;
    public Color CardHeaderBackgroundColor => IsDarkTheme ? Color.FromArgb("#444444") : Color.FromArgb("#1976D2");
    public Color EmptyBackgroundColor => IsDarkTheme ? Color.FromArgb("#333333") : Color.FromArgb("#E0E0E0");
    public Color EmptyTextColor => IsDarkTheme ? Colors.White : Color.FromArgb("#666666");
    public Color EmptySubTextColor => IsDarkTheme ? Color.FromArgb("#CCCCCC") : Color.FromArgb("#999999");
    public Color LoadingBackgroundColor => IsDarkTheme ? Color.FromArgb("#333333") : Color.FromArgb("#F5F5F5");
    public Color LoadingTextColor => IsDarkTheme ? Colors.White : Color.FromArgb("#333333");

    private void LoadThemeSettings()
    {
        try
        {
            if (File.Exists(_themeFilePath))
            {
                string data = File.ReadAllText(_themeFilePath);
                var settings = JsonSerializer.Deserialize<ThemeSettings>(data);
                IsDarkTheme = settings?.IsDarkTheme ?? false;
            }
            else
            {
                IsDarkTheme = false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Theme y�kleme hatas�: {ex.Message}");
            IsDarkTheme = false;
        }

        UpdateThemeColors();
    }

    private void OnThemeChanged(Ayarlar sender, bool isDarkTheme)
    {
        IsDarkTheme = isDarkTheme;
    }

    private void UpdateThemeColors()
    {
        this.BackgroundColor = PageBackgroundColor;

        // T�m renk �zelliklerini g�ncelle
        OnPropertyChanged(nameof(PageBackgroundColor));
        OnPropertyChanged(nameof(RefreshBackgroundColor));
        OnPropertyChanged(nameof(HeaderBackgroundColor));
        OnPropertyChanged(nameof(CardBackgroundColor));
        OnPropertyChanged(nameof(CardHeaderBackgroundColor));
        OnPropertyChanged(nameof(EmptyBackgroundColor));
        OnPropertyChanged(nameof(EmptyTextColor));
        OnPropertyChanged(nameof(EmptySubTextColor));
        OnPropertyChanged(nameof(LoadingBackgroundColor));
        OnPropertyChanged(nameof(LoadingTextColor));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadData();
    }

    private void LoadData()
    {
        Debug.WriteLine("LoadData called");

        // Loading g�ster
        loadingFrame.IsVisible = true;

        try
        {
            if (File.Exists(fileName))
            {
                Debug.WriteLine("File exists");
                string data = File.ReadAllText(fileName);
                Debug.WriteLine($"Data read from file: {data}");

                var sehirler = JsonSerializer.Deserialize<ObservableCollection<HavaDurumuModels>>(data);
                if (sehirler != null)
                {
                    Sehirler.Clear();
                    foreach (var sehir in sehirler)
                    {
                        Debug.WriteLine($"Adding city: {sehir.Name}");
                        Sehirler.Add(sehir);
                    }
                }
            }

            UpdateEmptyState();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoadData Error: {ex.Message}");
            DisplayAlert("Hata", "Veriler y�klenirken bir hata olu�tu.", "Tamam");
        }
        finally
        {
            // Loading gizle
            loadingFrame.IsVisible = false;
        }
    }

    private async void EkleClicked(object sender, EventArgs e)
    {
        try
        {
            // Zaten eklenmi� �ehirlerin API isimlerini al
            var ekliSehirler = Sehirler.Select(s => s.Name.ToUpperInvariant()).ToHashSet();

            // Hen�z eklenmeyen �ehirleri filtrele
            var uygunSehirler = TurkishCities.GetDisplayNames()
                .Where(displayName =>
                {
                    var apiName = TurkishCities.GetApiName(displayName);
                    return !ekliSehirler.Contains(apiName.ToUpperInvariant());
                })
                .ToArray();

            if (uygunSehirler.Length == 0)
            {
                await DisplayAlert("Bilgi", "T�m �ehirler zaten eklenmi�!", "Tamam");
                return;
            }

            // �ehir se�im men�s� g�ster
            var selectedCity = await DisplayActionSheet("�ehir Se�in", "�ptal", null, uygunSehirler);

            if (string.IsNullOrEmpty(selectedCity) || selectedCity == "�ptal")
                return;

            // Se�ilen �ehirin api ad�n� al
            var apiName = TurkishCities.GetApiName(selectedCity);

            // �ehri ekle
            Sehirler.Add(new HavaDurumuModels() { Name = apiName });
            SaveData();

            await DisplayAlert("Ba�ar�l�", $"{selectedCity} �ehri eklendi!", "Tamam");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"EkleClicked Error: {ex.Message}");
            await DisplayAlert("Hata", "�ehir eklenirken bir hata olu�tu.", "Tamam");
        }
    }

    private void YukleClicked(object sender, EventArgs e)
    {
        refreshView.IsRefreshing = false;
        LoadData();
    }

    private async void SilClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            if (button?.CommandParameter is string cityApiName)
            {
                var sehir = Sehirler.FirstOrDefault(s => s.Name == cityApiName);
                if (sehir != null)
                {
                    // Kullan�c� dostu ismi al
                    var displayName = TurkishCities.GetDisplayName(cityApiName);

                    var result = await DisplayAlert("�ehir Sil",
                        $"{displayName} �ehrini silmek istedi�inizden emin misiniz?",
                        "Evet",
                        "Hay�r");

                    if (result)
                    {
                        Sehirler.Remove(sehir);
                        SaveData();
                        await DisplayAlert("Ba�ar�l�", $"{displayName} �ehri silindi!", "Tamam");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SilClicked Error: {ex.Message}");
            await DisplayAlert("Hata", "�ehir silinirken bir hata olu�tu.", "Tamam");
        }
    }

    private void SaveData()
    {
        try
        {
            string data = JsonSerializer.Serialize(Sehirler, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(fileName, data);
            Debug.WriteLine($"Data saved: {Sehirler.Count} cities");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SaveData Error: {ex.Message}");
        }
    }

    private void UpdateEmptyState()
    {
        emptyFrame.IsVisible = Sehirler.Count == 0;
        listCity.IsVisible = Sehirler.Count > 0;
    }

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
        SaveData();
        MessagingCenter.Unsubscribe<Ayarlar, bool>(this, "ThemeChanged");
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    //tema ayarlar�
    public class ThemeSettings
    {
        public bool IsDarkTheme { get; set; }
        public DateTime LastChanged { get; set; } = DateTime.Now;
    }
}