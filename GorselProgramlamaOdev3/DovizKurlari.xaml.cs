using System.Collections.ObjectModel;
using System.Text.Json;
using System.ComponentModel;

namespace GorselProgramlamaOdev3;

public partial class DovizKurlari : ContentPage, INotifyPropertyChanged
{
    private ObservableCollection<CurrencyModel> currencies; // döviz kurlarını tutar
    private readonly HttpClient httpClient; // api'dan veri çekmek için
    private bool _isDarkTheme;
    private const string THEME_FILE = "theme_settings.json";
    private readonly string _themeFilePath;

    // Tema renkleri için private field'lar
    private Color _pageBackgroundColor;
    private Color _headerBackgroundColor;
    private Color _headerTextColor;
    private Color _subTextColor;
    private Color _cardBackgroundColor;
    private Color _primaryTextColor;
    private Color _secondaryTextColor;
    private Color _overlayBackgroundColor;
    private Color _headerBorderColor;
    private Color _cardBorderColor;
    private Color _loadingIndicatorColor;
    private Color _loadingTextColor;
    private Color _errorTextColor;
    private Color _buttonBackgroundColor;
    private Color _buttonTextColor;

    public DovizKurlari()
    {
        InitializeComponent();
        currencies = new ObservableCollection<CurrencyModel>();
        currencyCollectionView.ItemsSource = currencies;
        httpClient = new HttpClient();
        _themeFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, THEME_FILE);

        // Tema değişiklik mesajı
        MessagingCenter.Subscribe<Ayarlar, bool>(this, "ThemeChanged", OnThemeChanged);

        LoadThemeSettings();
        BindingContext = this;

        LoadCurrencyData();
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
                SaveThemeSettings(); // Tema ayarını kaydet
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

    public Color HeaderBackgroundColor
    {
        get => _headerBackgroundColor;
        private set
        {
            _headerBackgroundColor = value;
            OnPropertyChanged();
        }
    }

    public Color HeaderTextColor
    {
        get => _headerTextColor;
        private set
        {
            _headerTextColor = value;
            OnPropertyChanged();
        }
    }

    public Color SubTextColor
    {
        get => _subTextColor;
        private set
        {
            _subTextColor = value;
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

    public Color OverlayBackgroundColor
    {
        get => _overlayBackgroundColor;
        private set
        {
            _overlayBackgroundColor = value;
            OnPropertyChanged();
        }
    }

    public Color HeaderBorderColor
    {
        get => _headerBorderColor;
        private set
        {
            _headerBorderColor = value;
            OnPropertyChanged();
        }
    }

    public Color CardBorderColor
    {
        get => _cardBorderColor;
        private set
        {
            _cardBorderColor = value;
            OnPropertyChanged();
        }
    }

    public Color LoadingIndicatorColor
    {
        get => _loadingIndicatorColor;
        private set
        {
            _loadingIndicatorColor = value;
            OnPropertyChanged();
        }
    }

    public Color LoadingTextColor
    {
        get => _loadingTextColor;
        private set
        {
            _loadingTextColor = value;
            OnPropertyChanged();
        }
    }

    public Color ErrorTextColor
    {
        get => _errorTextColor;
        private set
        {
            _errorTextColor = value;
            OnPropertyChanged();
        }
    }

    public Color ButtonBackgroundColor
    {
        get => _buttonBackgroundColor;
        private set
        {
            _buttonBackgroundColor = value;
            OnPropertyChanged();
        }
    }

    public Color ButtonTextColor
    {
        get => _buttonTextColor;
        private set
        {
            _buttonTextColor = value;
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
            System.Diagnostics.Debug.WriteLine($"Theme yükleme hatası: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"Theme kaydetme hatası: {ex.Message}");
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
            HeaderBackgroundColor = Color.FromArgb("#222222");
            HeaderTextColor = Colors.White;
            SubTextColor = Color.FromArgb("#CCCCCC");
            CardBackgroundColor = Color.FromArgb("#222222");
            PrimaryTextColor = Colors.White;
            SecondaryTextColor = Color.FromArgb("#CCCCCC");
            OverlayBackgroundColor = Color.FromArgb("#80000000");
            HeaderBorderColor = Color.FromArgb("#555555");
            CardBorderColor = Color.FromArgb("#444444");
            LoadingIndicatorColor = Colors.White;
            LoadingTextColor = Colors.White;
            ErrorTextColor = Color.FromArgb("#E74C3C");
            ButtonBackgroundColor = Color.FromArgb("#3498DB");
            ButtonTextColor = Colors.White;
        }
        else
        {
            // Açık tema renkleri
            PageBackgroundColor = Color.FromArgb("#F5F5F5");
            HeaderBackgroundColor = Colors.White;
            HeaderTextColor = Color.FromArgb("#2C3E50");
            SubTextColor = Color.FromArgb("#95A5A6");
            CardBackgroundColor = Colors.White;
            PrimaryTextColor = Color.FromArgb("#2C3E50");
            SecondaryTextColor = Color.FromArgb("#7F8C8D");
            OverlayBackgroundColor = Color.FromArgb("#80000000");
            HeaderBorderColor = Color.FromArgb("#667eea");
            CardBorderColor = Color.FromArgb("#E0E0E0");
            LoadingIndicatorColor = Color.FromArgb("#3498DB");
            LoadingTextColor = Colors.White;
            ErrorTextColor = Color.FromArgb("#E74C3C");
            ButtonBackgroundColor = Color.FromArgb("#3498DB");
            ButtonTextColor = Colors.White;
        }

        // Sayfa arka plan rengini güncelle
        this.BackgroundColor = PageBackgroundColor;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        // Sayfa görünür olduğunda tema ayarlarını yeniden yükle
        LoadThemeSettings();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<Ayarlar, bool>(this, "ThemeChanged");
    }

    private async void RefreshClicked(object sender, EventArgs e)
    {
        await LoadCurrencyData();
    }

    private async Task LoadCurrencyData() // api'dan veri çekme işlemine başlar
    {
        try
        {
            ShowLoading(true);
            HideError();

            var currencyData = await FetchCurrencyData();

            if (currencyData != null && currencyData.Count > 0)
            {
                currencies.Clear();
                foreach (var currency in currencyData)
                {
                    currencies.Add(currency);
                }

                lblLastUpdate.Text = $"Son Güncelleme: {DateTime.Now:dd.MM.yyyy HH:mm}";
            }
            else
            {
                ShowError("Veri alınamadı. İnternet bağlantınızı kontrol edin.");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Hata: {ex.Message}");
        }
        finally
        {
            ShowLoading(false);
        }
    }

    private async Task<List<CurrencyModel>> FetchCurrencyData()
    {
        try
        {
            var finansUrl = "https://finans.truncgil.com/today.json";
            var response = await httpClient.GetStringAsync(finansUrl);

            if (!string.IsNullOrEmpty(response))
            {
                return ParseFinansData(response);
            }
        }
        catch (Exception ex)
        {
            ShowError("Veri çekilirken hata oluştu: " + ex.Message);
        }

        return new List<CurrencyModel>(); // boş liste döner
    }

    // json metnini döviz nesnelerine çeviren fonksiyon
    private List<CurrencyModel> ParseFinansData(string jsonData)
    {
        var currencies = new List<CurrencyModel>();

        try
        {
            using (JsonDocument document = JsonDocument.Parse(jsonData))
            {
                var root = document.RootElement;

                if (root.TryGetProperty("USD", out var usd))
                {
                    currencies.Add(new CurrencyModel
                    {
                        Name = "Amerikan Doları",
                        Code = "USD",
                        Symbol = "$",
                        Color = "#2ECC71",
                        BuyRate = usd.TryGetProperty("Alış", out var usdBuy) ? ParseDouble(usdBuy.GetString()) : 0,
                        SellRate = usd.TryGetProperty("Satış", out var usdSell) ? ParseDouble(usdSell.GetString()) : 0,
                        ChangePercent = usd.TryGetProperty("Değişim", out var usdChange) ? ParseDouble(usdChange.GetString()?.Replace("%", "")) : 0
                    });
                }

                if (root.TryGetProperty("EUR", out var eur))
                {
                    currencies.Add(new CurrencyModel
                    {
                        Name = "Euro",
                        Code = "EUR",
                        Symbol = "€",
                        Color = "#3498DB",
                        BuyRate = eur.TryGetProperty("Alış", out var eurBuy) ? ParseDouble(eurBuy.GetString()) : 0,
                        SellRate = eur.TryGetProperty("Satış", out var eurSell) ? ParseDouble(eurSell.GetString()) : 0,
                        ChangePercent = eur.TryGetProperty("Değişim", out var eurChange) ? ParseDouble(eurChange.GetString()?.Replace("%", "")) : 0
                    });
                }

                if (root.TryGetProperty("GBP", out var gbp))
                {
                    currencies.Add(new CurrencyModel
                    {
                        Name = "İngiliz Sterlini",
                        Code = "GBP",
                        Symbol = "£",
                        Color = "#9B59B6",
                        BuyRate = gbp.TryGetProperty("Alış", out var gbpBuy) ? ParseDouble(gbpBuy.GetString()) : 0,
                        SellRate = gbp.TryGetProperty("Satış", out var gbpSell) ? ParseDouble(gbpSell.GetString()) : 0,
                        ChangePercent = gbp.TryGetProperty("Değişim", out var gbpChange) ? ParseDouble(gbpChange.GetString()?.Replace("%", "")) : 0
                    });
                }

                if (root.TryGetProperty("gram-altin", out var gramAltin))
                {
                    currencies.Add(new CurrencyModel
                    {
                        Name = "Gram Altın",
                        Code = "GOLD",
                        Symbol = "Au",
                        Color = "#F39C12",
                        BuyRate = gramAltin.TryGetProperty("Alış", out var goldBuy) ? ParseDouble(goldBuy.GetString()) : 0,
                        SellRate = gramAltin.TryGetProperty("Satış", out var goldSell) ? ParseDouble(goldSell.GetString()) : 0,
                        ChangePercent = gramAltin.TryGetProperty("Değişim", out var goldChange) ? ParseDouble(goldChange.GetString()?.Replace("%", "")) : 0
                    });
                }

                if (root.TryGetProperty("ceyrek-altin", out var ceyrekAltin))
                {
                    currencies.Add(new CurrencyModel
                    {
                        Name = "Çeyrek Altın",
                        Code = "QUARTER",
                        Symbol = "¼",
                        Color = "#E67E22",
                        BuyRate = ceyrekAltin.TryGetProperty("Alış", out var quarterBuy) ? ParseDouble(quarterBuy.GetString()) : 0,
                        SellRate = ceyrekAltin.TryGetProperty("Satış", out var quarterSell) ? ParseDouble(quarterSell.GetString()) : 0,
                        ChangePercent = ceyrekAltin.TryGetProperty("Değişim", out var quarterChange) ? ParseDouble(quarterChange.GetString()?.Replace("%", "")) : 0
                    });
                }

                if (root.TryGetProperty("gumus", out var gumus))
                {
                    currencies.Add(new CurrencyModel
                    {
                        Name = "Gümüş",
                        Code = "SILVER",
                        Symbol = "Ag",
                        Color = "#95A5A6",
                        BuyRate = gumus.TryGetProperty("Alış", out var silverBuy) ? ParseDouble(silverBuy.GetString()) : 0,
                        SellRate = gumus.TryGetProperty("Satış", out var silverSell) ? ParseDouble(silverSell.GetString()) : 0,
                        ChangePercent = gumus.TryGetProperty("Değişim", out var silverChange) ? ParseDouble(silverChange.GetString()?.Replace("%", "")) : 0
                    });
                }
            }
        }
        catch (Exception ex)
        {
            ShowError("Veri işlenirken hata oluştu: " + ex.Message);
        }

        return currencies;
    }

    private double ParseDouble(string value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;

        value = value.Replace(".", "").Replace(",", ".").Replace("$", "").Trim();

        if (double.TryParse(value, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out double result))
        {
            return result;
        }

        return 0;
    }

    private void ShowLoading(bool show)
    {
        loadingOverlay.IsVisible = show;
    }

    private void ShowError(string message)
    {
        lblErrorMessage.Text = message;
        errorOverlay.IsVisible = true;
    }

    private void HideError()
    {
        errorOverlay.IsVisible = false;
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

public class CurrencyModel
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Symbol { get; set; }
    public string Color { get; set; }
    public double BuyRate { get; set; }
    public double SellRate { get; set; }
    public double ChangePercent { get; set; }

    public string ChangeColor => ChangePercent >= 0 ? "#27AE60" : "#E74C3C";
    public string TrendArrow => ChangePercent >= 0 ? "↗" : "↘";
}