using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using GorselProgramlamaOdev3.Models;

namespace GorselProgramlamaOdev3;

public partial class Haberler : ContentPage, INotifyPropertyChanged
{
    private readonly HttpClient _httpClient;
    private bool _isLoading;
    private bool _isDarkTheme;
    private ObservableCollection<NewsItem> _news;
    private ObservableCollection<NewsItem> _allNews;
    private string _selectedCategory = "Manþet";
    private const string THEME_FILE = "theme_settings.json";
    private readonly string _themeFilePath;

    // Tema renkleri 
    private Color _pageBackgroundColor;
    private Color _cardBackgroundColor;
    private Color _categoryBackgroundColor;
    private Color _categoryTextColor;
    private Color _newsCardBackgroundColor;
    private Color _titleTextColor;
    private Color _descriptionTextColor;
    private Color _dateTextColor;

    private readonly Dictionary<string, string> _newsCategories = new()
    {
        { "Manþet", "https://www.trthaber.com/manset_articles.rss" },
        { "Son Dakika", "https://www.trthaber.com/sondakika_articles.rss" },
        { "Gündem", "https://www.trthaber.com/gundem_articles.rss" },
        { "Ekonomi", "https://www.trthaber.com/ekonomi_articles.rss" },
        { "Spor", "https://www.trthaber.com/spor_articles.rss" }
    };

    public Haberler()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _themeFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, THEME_FILE);

        Categories = new ObservableCollection<string> { "Manþet", "Son Dakika", "Gündem", "Ekonomi", "Spor" };
        News = new ObservableCollection<NewsItem>();
        AllNews = new ObservableCollection<NewsItem>();

        RefreshCommand = new Command(async () => await LoadNewsAsync());

        // Tema deðiþiklik mesajý
        MessagingCenter.Subscribe<Ayarlar, bool>(this, "ThemeChanged", OnThemeChanged);

        LoadThemeSettings();
        BindingContext = this;

        // Ýlk kategori olarak Manþet'i seç
        SelectedCategory = "Manþet";

        _ = LoadNewsAsync();
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

    public Color CardBackgroundColor
    {
        get => _cardBackgroundColor;
        private set
        {
            _cardBackgroundColor = value;
            OnPropertyChanged();
        }
    }

    public Color CategoryBackgroundColor
    {
        get => _categoryBackgroundColor;
        private set
        {
            _categoryBackgroundColor = value;
            OnPropertyChanged();
        }
    }

    public Color CategoryTextColor
    {
        get => _categoryTextColor;
        private set
        {
            _categoryTextColor = value;
            OnPropertyChanged();
        }
    }

    public Color NewsCardBackgroundColor
    {
        get => _newsCardBackgroundColor;
        private set
        {
            _newsCardBackgroundColor = value;
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

    public Color DescriptionTextColor
    {
        get => _descriptionTextColor;
        private set
        {
            _descriptionTextColor = value;
            OnPropertyChanged();
        }
    }

    public Color DateTextColor
    {
        get => _dateTextColor;
        private set
        {
            _dateTextColor = value;
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
            PageBackgroundColor = Color.FromArgb("#2E3440");
            CardBackgroundColor = Color.FromArgb("#3B4252");
            CategoryBackgroundColor = Color.FromArgb("#4C566A");
            CategoryTextColor = Color.FromArgb("#ECEFF4");
            NewsCardBackgroundColor = Color.FromArgb("#3B4252");
            TitleTextColor = Color.FromArgb("#ECEFF4");
            DescriptionTextColor = Color.FromArgb("#D8DEE9");
            DateTextColor = Color.FromArgb("#88C0D0");
        }
        else
        {
            // Açýk tema renkleri
            PageBackgroundColor = Color.FromArgb("#F5F5F5");
            CardBackgroundColor = Colors.White;
            CategoryBackgroundColor = Color.FromArgb("#E0E0E0");
            CategoryTextColor = Color.FromArgb("#333333");
            NewsCardBackgroundColor = Colors.White;
            TitleTextColor = Color.FromArgb("#2C3E50");
            DescriptionTextColor = Color.FromArgb("#7F8C8D");
            DateTextColor = Color.FromArgb("#95A5A6");
        }

        // Sayfa arka plan rengini güncelle
        this.BackgroundColor = PageBackgroundColor;
    }

    public ObservableCollection<string> Categories { get; }

    public ObservableCollection<NewsItem> News
    {
        get => _news;
        set
        {
            _news = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<NewsItem> AllNews
    {
        get => _allNews;
        set
        {
            _allNews = value;
            OnPropertyChanged();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            OnPropertyChanged();
        }
    }

    public string SelectedCategoryItem { get; set; }

    public Command RefreshCommand { get; }

    private async Task LoadNewsAsync()
    {
        try
        {
            IsLoading = true;

            // Mevcut haberleri temizle
            News.Clear();

            // Seçili kategoriden haberleri getir
            var categoryNews = await GetNewsByCategoryAsync(SelectedCategory);

            if (categoryNews.Count > 0)
            {
                // Tarihe göre sýrala
                var sortedNews = categoryNews.OrderByDescending(n => n.ParsedDate).ToList();

                // Tekrar haberleri News koleksiyonuna ekle
                foreach (var news in sortedNews)
                {
                    News.Add(news);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Haberler yüklenirken hata oluþtu: {ex.Message}", "Tamam");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task<List<NewsItem>> GetNewsByCategoryAsync(string category)
    {
        try
        {
            if (!_newsCategories.ContainsKey(category))
                return new List<NewsItem>();

            var rssUrl = _newsCategories[category];
            var apiUrl = $"https://api.rss2json.com/v1/api.json?rss_url={Uri.EscapeDataString(rssUrl)}";

            var response = await _httpClient.GetStringAsync(apiUrl);
            var newsResponse = JsonSerializer.Deserialize<RssResponse>(response);

            if (newsResponse?.Items != null)
            {
                foreach (var item in newsResponse.Items)
                {
                    item.Category = category;

                    if (string.IsNullOrEmpty(item.Thumbnail) && item.Enclosure?.Link != null)
                    {
                        item.Thumbnail = item.Enclosure.Link;
                    }
                }

                return newsResponse.Items;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Kategori haber getirme hatasý ({category}): {ex.Message}");
        }

        return new List<NewsItem>();
    }

    private async void OnCategorySelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is string selectedCategory)
        {
            SelectedCategory = selectedCategory;
            await LoadNewsAsync();

            // Seçimi temizle
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private async void OnNewsSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is NewsItem selectedNews)
        {
            await Shell.Current.GoToAsync("HaberDetay", new Dictionary<string, object>
            {
                ["NewsItem"] = selectedNews
            });

            // Seçimi temizle
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private async void OnNewsDetailClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is NewsItem news)
        {
            await Shell.Current.GoToAsync("HaberDetay", new Dictionary<string, object>
            {
                ["NewsItem"] = news
            });
        }
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