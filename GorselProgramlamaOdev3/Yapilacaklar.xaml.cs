using GorselProgramlamaOdev3.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;

namespace GorselProgramlamaOdev3;

public partial class Yapilacaklar : ContentPage, INotifyPropertyChanged
{
    public ObservableCollection<Gorev> Gorevler { get; set; }

    private bool _isDarkTheme;
    private const string THEME_FILE = "theme_settings.json";
    private readonly string _themeFilePath;

    public Yapilacaklar()
    {
        InitializeComponent();
        _themeFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, THEME_FILE);
        Gorevler = new ObservableCollection<Gorev>();
        LoadThemeSettings();
        BindingContext = this;
        LoadGorevler();

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
    public Color SecondaryTextColor => IsDarkTheme ? Color.FromArgb("#BBBBBB") : Colors.Gray;
    public Color PageBackgroundColor => IsDarkTheme ? Colors.Black : Color.FromArgb("#F5F5F5");
    public Color CardBackgroundColor => IsDarkTheme ? Color.FromArgb("#222222") : Colors.White;
    public Color CardBorderColor => IsDarkTheme ? Color.FromArgb("#444444") : Colors.LightGray;

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
        OnPropertyChanged(nameof(SecondaryTextColor));
        OnPropertyChanged(nameof(PageBackgroundColor));
        OnPropertyChanged(nameof(CardBackgroundColor));
        OnPropertyChanged(nameof(CardBorderColor));
    }

    private async void LoadGorevler()
    {
        try
        {
            var gorevler = await FireBaseServices.GetGorevlerAsync();
            Gorevler.Clear();
            foreach (var gorev in gorevler)
            {
                Gorevler.Add(gorev);
            }

            emptyStateLayout.IsVisible = Gorevler.Count == 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Görevler yüklenirken hata: {ex.Message}", "Tamam");
        }
    }

    private async void OnGorevEkleClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GorevDetay());
    }

    private async void OnGorevDuzenleClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Gorev gorev)
        {
            await Navigation.PushAsync(new GorevDetay(gorev));
        }
    }

    private async void OnGorevSilClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Gorev gorev)
        {
            bool result = await DisplayAlert("Silinsin mi?",
                $"'{gorev.Baslik}' görevini silmek istediðinizden emin misiniz?",
                "Evet", "Hayýr");

            if (result)
            {
                bool success = await FireBaseServices.DeleteGorevAsync(gorev.Id);
                if (success)
                {
                    LoadGorevler();
                    await DisplayAlert("Baþarýlý", "Görev silindi", "Tamam");
                }
            }
        }
    }

    private async void OnYapildiChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.BindingContext is Gorev gorev)
        {
            gorev.Yapildi = e.Value;
            await FireBaseServices.UpdateGorevAsync(gorev.Id, gorev);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadGorevler();
        LoadThemeSettings(); // Tema ayarlarýný yeniden yükle
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