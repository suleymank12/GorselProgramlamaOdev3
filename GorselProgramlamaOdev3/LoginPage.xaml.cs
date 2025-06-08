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

        // giri� yapan var m� diye kontrol ediyor
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
            await DisplayAlert("Hata", "L�tfen t�m alanlar� doldurun", "Tamam");
            return;
        }

        ShowLoading(true);

        try
        {
            var user = await FireBaseServices.Login(txtEmail.Text.Trim(), txtPassword.Text);

            if (user != null)
            {
                Preferences.Set("user_name", user.Info.DisplayName ?? "Kullan�c�");
                Preferences.Set("user_email", user.Info.Email);
                Preferences.Set("is_logged_in", true);

                await DisplayAlert("Ba�ar�l�!", $"Ho�geldin {user.Info.DisplayName ?? "Kullan�c�"}!", "Tamam");

                // ana sayfaya at�yor
                await Shell.Current.GoToAsync("//AnaSayfa");
            }
            else
            {
                await DisplayAlert("Hata", "E-posta veya �ifre hatal�", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", "Bir hata olu�tu. L�tfen tekrar deneyin.", "Tamam");
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
            await DisplayAlert("Hata", "L�tfen t�m alanlar� doldurun", "Tamam");
            return;
        }

        if (txtRegisterPassword.Text.Length < 6)
        {
            await DisplayAlert("Hata", "�ifre en az 6 karakter olmal�d�r", "Tamam");
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
                // Kay�t ba�ar�l� mesaj� g�ster
                await DisplayAlert("Ba�ar�l�!", "Kay�t ba�ar�yla tamamland�! �imdi giri� yapabilirsiniz.", "Tamam");

                // Kay�t formundaki alanlar� temizle
                txtName.Text = string.Empty;
                txtRegisterEmail.Text = string.Empty;
                txtRegisterPassword.Text = string.Empty;

                // Giri� formuna ge�
                ShowLoginForm(null, null);
            }
            else
            {
                await DisplayAlert("Hata", "Kay�t s�ras�nda bir hata olu�tu. Bu e-posta zaten kullan�mda olabilir.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", "Bir hata olu�tu. L�tfen tekrar deneyin.", "Tamam");
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