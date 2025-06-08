using System.ComponentModel;
using System.Text.Json;

namespace GorselProgramlamaOdev3;

public partial class Ayarlar : ContentPage, INotifyPropertyChanged
{
    private bool _isDarkTheme;
    private const string THEME_FILE = "theme_settings.json";
    private readonly string _themeFilePath;

    public Ayarlar()
    {
        InitializeComponent();
        _themeFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, THEME_FILE);
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

    // Tema Renkleri - siyah ve beyaz
    public Color TextColor => IsDarkTheme ? Colors.White : Color.FromArgb("#333333");
    public Color CardBackgroundColor => IsDarkTheme ? Color.FromArgb("#222222") : Color.FromArgb("#F5F5F5");
    public Color CardBorderColor => IsDarkTheme ? Color.FromArgb("#444444") : Color.FromArgb("#E0E0E0");

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
                IsDarkTheme = false; // Varsayılan açık tema
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Theme yükleme hatası: {ex.Message}");
            IsDarkTheme = false;
        }

        this.BackgroundColor = IsDarkTheme ? Colors.Black : Colors.White;
    }

    private void SaveThemeSettings()
    {
        try
        {
            var settings = new ThemeSettings { IsDarkTheme = IsDarkTheme };
            string data = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_themeFilePath, data);

            System.Diagnostics.Debug.WriteLine($"Theme kaydedildi: {(IsDarkTheme ? "Koyu" : "Açık")}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Theme kaydetme hatası: {ex.Message}");
        }
    }

    // switch kontrolüne tıklanınca çalışıyor
    private void OnThemeToggled(object sender, ToggledEventArgs e)
    {
        IsDarkTheme = e.Value;
        SaveThemeSettings();

        // Ana uygulamaya tema değişikliğini bildir
        MessagingCenter.Send(this, "ThemeChanged", IsDarkTheme);

        DisplayAlert("Tema Değişti",
            IsDarkTheme ? "Koyu tema aktif edildi" : "Açık tema aktif edildi",
            "Tamam");
    }

    private void UpdateThemeColors()
    {
        this.BackgroundColor = IsDarkTheme ? Colors.Black : Colors.White;

        // Renk özelliklerini güncelle
        OnPropertyChanged(nameof(TextColor));
        OnPropertyChanged(nameof(CardBackgroundColor));
        OnPropertyChanged(nameof(CardBorderColor));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // tema ayarları
    public class ThemeSettings
    {
        public bool IsDarkTheme { get; set; }
        public DateTime LastChanged { get; set; } = DateTime.Now;
    }
}