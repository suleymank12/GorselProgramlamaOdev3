using System.ComponentModel;
using System.Text.Json;

namespace GorselProgramlamaOdev3;

public partial class AnaSayfa : ContentPage, INotifyPropertyChanged
{
    private bool _isDarkTheme;
    private const string THEME_FILE = "theme_settings.json";
    private readonly string _themeFilePath;

    // Tema renkleri i�in private alanlar
    private Color _pageBackgroundColor;
    private Color _cardBackgroundColor;
    private Color _photoBackgroundColor;
    private Color _primaryTextColor;
    private Color _secondaryTextColor;
    private Color _accentTextColor;
    private Color _borderColor;

    public AnaSayfa()
    {
        InitializeComponent();
        _themeFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, THEME_FILE);

        // Tema de�i�iklik mesaj� gelirse ona g�re rengi de�i�tir
        MessagingCenter.Subscribe<Ayarlar, bool>(this, "ThemeChanged", OnThemeChanged);

        LoadThemeSettings();
        BindingContext = this;
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
                SaveThemeSettings(); // Tema ayar�n� kaydet
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

    public Color CardBackgroundColor
    {
        get => _cardBackgroundColor;
        private set
        {
            _cardBackgroundColor = value;
            OnPropertyChanged();
        }
    }

    public Color PhotoBackgroundColor
    {
        get => _photoBackgroundColor;
        private set
        {
            _photoBackgroundColor = value;
            OnPropertyChanged();
        }
    }

    public Color PrimaryTextColor
    {
        get => _primaryTextColor;
        private set
        {
            _primaryTextColor = value;
            OnPropertyChanged();
        }
    }

    public Color SecondaryTextColor
    {
        get => _secondaryTextColor;
        private set
        {
            _secondaryTextColor = value;
            OnPropertyChanged();
        }
    }

    public Color AccentTextColor
    {
        get => _accentTextColor;
        private set
        {
            _accentTextColor = value;
            OnPropertyChanged();
        }
    }

    public Color BorderColor
    {
        get => _borderColor;
        private set
        {
            _borderColor = value;
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
            System.Diagnostics.Debug.WriteLine($"Theme y�kleme hatas�: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"Theme kaydetme hatas�: {ex.Message}");
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
            PageBackgroundColor = Colors.Black;
            CardBackgroundColor = Color.FromArgb("#222222");
            PhotoBackgroundColor = Color.FromArgb("#333333");
            PrimaryTextColor = Colors.White;
            SecondaryTextColor = Color.FromArgb("#CCCCCC");
            AccentTextColor = Color.FromArgb("#AAAAAA");
            BorderColor = Color.FromArgb("#555555");
        }
        else
        {
            // A��k tema renkleri
            PageBackgroundColor = Color.FromArgb("#F5F5F5");
            CardBackgroundColor = Colors.White;
            PhotoBackgroundColor = Colors.White;
            PrimaryTextColor = Color.FromArgb("#2C3E50");
            SecondaryTextColor = Color.FromArgb("#7F8C8D");
            AccentTextColor = Color.FromArgb("#95A5A6");
            BorderColor = Color.FromArgb("#667eea");
        }

        // Sayfa arka plan rengini g�ncelle
        this.BackgroundColor = PageBackgroundColor;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // giri� yapt�ktan sonra pencereyi etkinle�tir
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;

        // Sayfa g�r�n�r oldu�unda tema ayarlar�n� yeniden y�kle
        LoadThemeSettings();

        // kullan�c� giri�i kontrol et
        bool isLoggedIn = Preferences.Get("is_logged_in", false);
        if (!isLoggedIn)
        {
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        // kullan�c� bilgileri y�kle
        string userName = Preferences.Get("user_name", "Kullan�c�");
        lblUserName.Text = userName;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<Ayarlar, bool>(this, "ThemeChanged");
    }

    private async void NavigateToDovizKurlari(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//DovizKurlari");
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // tema ayarlar�
    public class ThemeSettings
    {
        public bool IsDarkTheme { get; set; }
        public DateTime LastChanged { get; set; } = DateTime.Now;
    }
}