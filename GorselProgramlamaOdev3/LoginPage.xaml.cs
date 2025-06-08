using GorselProgramlamaOdev3.Models;

namespace GorselProgramlamaOdev3;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // giriþ yapan var mý diye kontrol ediyor
        if (await FireBaseServices.IsUserLoggedIn())
        {
            await Shell.Current.GoToAsync("//AnaSayfa");
        }
    }

    private void ShowRegisterForm(object sender, EventArgs e)
    {
        loginStack.IsVisible = false;
        registerStack.IsVisible = true;
    }

    private void ShowLoginForm(object sender, EventArgs e)
    {
        registerStack.IsVisible = false;
        loginStack.IsVisible = true;
    }

    private async void LoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtPassword.Text))
        {
            await DisplayAlert("Hata", "Lütfen tüm alanlarý doldurun", "Tamam");
            return;
        }

        ShowLoading(true);

        try
        {
            var user = await FireBaseServices.Login(txtEmail.Text.Trim(), txtPassword.Text);

            if (user != null)
            {
                Preferences.Set("user_name", user.Info.DisplayName ?? "Kullanýcý");
                Preferences.Set("user_email", user.Info.Email);
                Preferences.Set("is_logged_in", true);

                await DisplayAlert("Baþarýlý!", $"Hoþgeldin {user.Info.DisplayName ?? "Kullanýcý"}!", "Tamam");

                // ana sayfaya atýyor
                await Shell.Current.GoToAsync("//AnaSayfa");
            }
            else
            {
                await DisplayAlert("Hata", "E-posta veya þifre hatalý", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", "Bir hata oluþtu. Lütfen tekrar deneyin.", "Tamam");
        }
        finally
        {
            ShowLoading(false);
        }
    }

    private async void RegisterClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtName.Text) ||
            string.IsNullOrEmpty(txtRegisterEmail.Text) ||
            string.IsNullOrEmpty(txtRegisterPassword.Text))
        {
            await DisplayAlert("Hata", "Lütfen tüm alanlarý doldurun", "Tamam");
            return;
        }

        if (txtRegisterPassword.Text.Length < 6)
        {
            await DisplayAlert("Hata", "Þifre en az 6 karakter olmalýdýr", "Tamam");
            return;
        }

        ShowLoading(true);

        try
        {
            var user = await FireBaseServices.Register(
                txtName.Text.Trim(),
                txtRegisterEmail.Text.Trim(),
                txtRegisterPassword.Text);

            if (user != null)
            {
                // Kayýt baþarýlý mesajý göster
                await DisplayAlert("Baþarýlý!", "Kayýt baþarýyla tamamlandý! Þimdi giriþ yapabilirsiniz.", "Tamam");

                // Kayýt formundaki alanlarý temizle
                txtName.Text = string.Empty;
                txtRegisterEmail.Text = string.Empty;
                txtRegisterPassword.Text = string.Empty;

                // Giriþ formuna geç
                ShowLoginForm(null, null);
            }
            else
            {
                await DisplayAlert("Hata", "Kayýt sýrasýnda bir hata oluþtu. Bu e-posta zaten kullanýmda olabilir.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", "Bir hata oluþtu. Lütfen tekrar deneyin.", "Tamam");
        }
        finally
        {
            ShowLoading(false);
        }
    }

    private void ShowLoading(bool show)
    {
        loadingOverlay.IsVisible = show;
        btnLogin.IsEnabled = !show;
        btnRegister.IsEnabled = !show;
    }
}