using GorselProgramlamaOdev3.Models;
using System.ComponentModel;
using System.Text.Json;

namespace GorselProgramlamaOdev3;

public partial class GorevDetay : ContentPage, INotifyPropertyChanged
{
    private Gorev mevcutGorev;
    private bool isEditMode;

    private bool _isDarkTheme;
    private const string THEME_FILE = "theme_settings.json";
    private readonly string _themeFilePath;

    // Yeni görev için constructor
    public GorevDetay()
    {
        InitializeComponent();
        _themeFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, THEME_FILE);
        isEditMode = false;
        lblBaslik.Text = "Görev Ekleme";
        LoadThemeSettings();
        BindingContext = this;

        // Tema deðiþikliði mesajýný dinle
        MessagingCenter.Subscribe<Ayarlar, bool>(this, "ThemeChanged", OnThemeChanged);
    }

    // Düzenleme için constructor
    public GorevDetay(Gorev gorev)
    {
        InitializeComponent();
        _themeFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, THEME_FILE);
        mevcutGorev = gorev;
        isEditMode = true;
        lblBaslik.Text = "Görev Düzenleme";
        LoadThemeSettings();
        BindingContext = this;
        LoadGorevData();

        // Tema deðiþikliði mesajýný dinle
        MessagingCenter.Subscribe<Ayarlar, bool>(this, "ThemeChanged", OnThemeChanged);
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
    public Color TextColor => IsDarkTheme ? Colors.White : Color.FromArgb("#333333");
    public Color PageBackgroundColor => IsDarkTheme ? Colors.Black : Color.FromArgb("#F5F5F5");
    public Color CardBackgroundColor => IsDarkTheme ? Color.FromArgb("#222222") : Colors.White;
    public Color CardBorderColor => IsDarkTheme ? Color.FromArgb("#444444") : Colors.LightGray;
    public Color EntryBackgroundColor => IsDarkTheme ? Color.FromArgb("#2C2C2C") : Color.FromArgb("#F8F8F8");

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
        catch
        {
            IsDarkTheme = false;
        }
    }

    private void OnThemeChanged(Ayarlar sender, bool isDarkTheme)
    {
        IsDarkTheme = isDarkTheme;
    }

    private void UpdateThemeColors()
    {
        OnPropertyChanged(nameof(TextColor));
        OnPropertyChanged(nameof(PageBackgroundColor));
        OnPropertyChanged(nameof(CardBackgroundColor));
        OnPropertyChanged(nameof(CardBorderColor));
        OnPropertyChanged(nameof(EntryBackgroundColor));
    }

    private void LoadGorevData()
    {
        if (mevcutGorev != null)
        {
            entryBaslik.Text = mevcutGorev.Baslik;
            editorDetay.Text = mevcutGorev.Detay;
            entryTarih.Text = mevcutGorev.Tarih;
            entrySaat.Text = mevcutGorev.Saat;
        }
    }

    private async void OnTamamClicked(object sender, EventArgs e)
    {
        // doðrulama
        if (string.IsNullOrWhiteSpace(entryBaslik.Text))
        {
            await DisplayAlert("Hata", "Baþlýk alaný boþ olamaz!", "Tamam");
            return;
        }

        bool success = false;

        if (isEditMode && mevcutGorev != null)
        {
            // Güncelleme
            mevcutGorev.Baslik = entryBaslik.Text;
            mevcutGorev.Detay = editorDetay.Text ?? "";
            mevcutGorev.Tarih = entryTarih.Text ?? "";
            mevcutGorev.Saat = entrySaat.Text ?? "";

            success = await FireBaseServices.UpdateGorevAsync(mevcutGorev.Id, mevcutGorev);
        }
        else
        {
            // Yeni görev ekleme
            var yeniGorev = new Gorev(
                entryBaslik.Text,
                editorDetay.Text ?? "",
                entryTarih.Text ?? "",
                entrySaat.Text ?? ""
            );

            success = await FireBaseServices.AddGorevAsync(yeniGorev);
        }

        if (success)
        {
            await DisplayAlert("Baþarýlý",
                isEditMode ? "Görev güncellendi" : "Görev eklendi",
                "Tamam");
            await Navigation.PopAsync();
        }
    }

    private async void OnIptalClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadThemeSettings(); // Tema ayarlarýný yeniden yükle
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Mesaj dinlemeyi durdur
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