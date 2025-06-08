using System.ComponentModel;
using System.Text.Json;
using GorselProgramlamaOdev3.Models;

namespace GorselProgramlamaOdev3;

[QueryProperty(nameof(NewsItem), "NewsItem")]
public partial class HaberDetay : ContentPage, INotifyPropertyChanged
{
    private NewsItem _newsItem;
    private bool _isDarkTheme;
    private const string THEME_FILE = "theme_settings.json";
    private readonly string _themeFilePath;

    // Tema renkleri 
    private Color _pageBackgroundColor;
    private Color _contentBackgroundColor;
    private Color _titleTextColor;
    private Color _bodyTextColor;
    private Color _metaTextColor;
    private Color _metaBackgroundColor;
    private Color _overlayColor;

    public HaberDetay()
    {
        InitializeComponent();
        _themeFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, THEME_FILE);

        // Tema deðiþiklik mesajý
        MessagingCenter.Subscribe<Ayarlar, bool>(this, "ThemeChanged", OnThemeChanged);

        LoadThemeSettings();
        BindingContext = this;
    }

    public NewsItem NewsItem
    {
        get => _newsItem;
        set
        {
            _newsItem = value;
            OnPropertyChanged();
        }
    }

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (_isDarkTheme != value)
            {
                _isDarkTheme = value;
                OnPropertyChanged();
                UpdateThemeColors();
                SaveThemeSettings();
            }
        }
    }

    // Tema Renkleri 
    public Color PageBackgroundColor
    {
        get => _pageBackgroundColor;
        private set
        {
            _pageBackgroundColor = value;
            OnPropertyChanged();
        }
    }

    public Color ContentBackgroundColor
    {
        get => _contentBackgroundColor;
        private set
        {
            _contentBackgroundColor = value;
            OnPropertyChanged();
        }
    }

    public Color TitleTextColor
    {
        get => _titleTextColor;
        private set
        {
            _titleTextColor = value;
            OnPropertyChanged();
        }
    }

    public Color BodyTextColor
    {
        get => _bodyTextColor;
        private set
        {
            _bodyTextColor = value;
            OnPropertyChanged();
        }
    }

    public Color MetaTextColor
    {
        get => _metaTextColor;
        private set
        {
            _metaTextColor = value;
            OnPropertyChanged();
        }
    }

    public Color MetaBackgroundColor
    {
        get => _metaBackgroundColor;
        private set
        {
            _metaBackgroundColor = value;
            OnPropertyChanged();
        }
    }

    public Color OverlayColor
    {
        get => _overlayColor;
        private set
        {
            _overlayColor = value;
            OnPropertyChanged();
        }
    }

    private void LoadThemeSettings()
    {
        try
        {
            if (File.Exists(_themeFilePath))
            {
                string data = File.ReadAllText(_themeFilePath);
                var settings = JsonSerializer.Deserialize<ThemeSettings>(data);
                _isDarkTheme = settings?.IsDarkTheme ?? false;
            }
            else
            {
                _isDarkTheme = false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Theme yükleme hatasý: {ex.Message}");
            _isDarkTheme = false;
        }

        UpdateThemeColors();
    }

    private void SaveThemeSettings()
    {
        try
        {
            var settings = new ThemeSettings { IsDarkTheme = _isDarkTheme };
            string data = JsonSerializer.Serialize(settings);
            File.WriteAllText(_themeFilePath, data);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Theme kaydetme hatasý: {ex.Message}");
        }
    }

    private void OnThemeChanged(Ayarlar sender, bool isDarkTheme)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsDarkTheme = isDarkTheme;
        });
    }

    private void UpdateThemeColors()
    {
        if (IsDarkTheme)
        {
            // Koyu tema renkleri
            PageBackgroundColor = Color.FromArgb("#1E1E1E");
            ContentBackgroundColor = Color.FromArgb("#2D2D2D");
            TitleTextColor = Colors.White;
            BodyTextColor = Color.FromArgb("#E0E0E0");
            MetaTextColor = Color.FromArgb("#B0B0B0");
            MetaBackgroundColor = Color.FromArgb("#404040");
            OverlayColor = Color.FromArgb("#80000000");
        }
        else
        {
            // Açýk tema renkleri
            PageBackgroundColor = Color.FromArgb("#F8F9FA");
            ContentBackgroundColor = Colors.White;
            TitleTextColor = Color.FromArgb("#2C3E50");
            BodyTextColor = Color.FromArgb("#34495E");
            MetaTextColor = Color.FromArgb("#7F8C8D");
            MetaBackgroundColor = Color.FromArgb("#ECF0F1");
            OverlayColor = Color.FromArgb("#80000000");
        }

        // Sayfa arka plan rengini güncelle
        this.BackgroundColor = PageBackgroundColor;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Sayfa görünür olduðunda tema ayarlarýný yeniden yükle
        LoadThemeSettings();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<Ayarlar, bool>(this, "ThemeChanged");
    }

    private async void OnShareClicked(object sender, EventArgs e)
    {
        await ShareNews();
    }

    private async Task ShareNews()
    {
        try
        {
            if (NewsItem == null) return;

            await Share.RequestAsync(new ShareTextRequest
            {
                Text = $"{NewsItem.Title}\n\n{NewsItem.CleanDescription}\n\nKaynak: {NewsItem.Link}",
                Title = "Haberi Paylaþ"
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Paylaþým sýrasýnda hata oluþtu: {ex.Message}", "Tamam");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // tema ayarlarý
    public class ThemeSettings
    {
        public bool IsDarkTheme { get; set; }
        public DateTime LastChanged { get; set; } = DateTime.Now;
    }
}