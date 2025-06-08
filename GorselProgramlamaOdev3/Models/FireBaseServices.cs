using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Firebase.Database.Query;
using GorselProgramlamaOdev3.Models;
using System.Collections.ObjectModel;

namespace GorselProgramlamaOdev3.Models
{
    public class FireBaseServices
    {
        static string project_id = "gorselodev-cc38a";
        static string ApiKey = "AIzaSyAZrK5m8GK9kkk0JxIGbAo5dPztG5ckuqI";
        static string AuthDomain = $"{project_id}.firebaseapp.com";
        static string DatabaseURL = "https://gorselodev-cc38a-default-rtdb.firebaseio.com/";

        static FirebaseAuthConfig config = new FirebaseAuthConfig()
        {
            ApiKey = ApiKey,
            AuthDomain = AuthDomain,
            Providers = new[] { new EmailProvider() }
        };

        // Veri bağlantısı kurulum
        private static FirebaseClient firebaseClient = new FirebaseClient(DatabaseURL);

        // Kimlik doğrulama
        public static async Task<User?> Login(string email, string password)
        {
            try
            {
                var client = new FirebaseAuthClient(config);
                var res = await client.SignInWithEmailAndPasswordAsync(email, password);
                return res.User;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static async Task<User?> Register(string dname, string email, string password)
        {
            try
            {
                var client = new FirebaseAuthClient(config);
                var res = await client.CreateUserWithEmailAndPasswordAsync(email, password, dname);
                return res.User;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // Giriş yapan kullanıcı var mı kontrol et
        public static async Task<bool> IsUserLoggedIn()
        {
            try
            {
                var client = new FirebaseAuthClient(config);
                return client.User != null;
            }
            catch
            {
                return false;
            }
        }

        public static async Task SignOut()
        {
            try
            {
                var client = new FirebaseAuthClient(config);
                client.SignOut();
            }
            catch
            {         
            }
        }

        // Yapilacaklar listesi için realtime database
        public static async Task<ObservableCollection<Gorev>> GetGorevlerAsync()
        {
            try
            {
                var gorevler = await firebaseClient
                    .Child("gorevler")
                    .OnceAsync<Gorev>();

                var gorevListesi = new ObservableCollection<Gorev>();

                foreach (var item in gorevler)
                {
                    var gorev = item.Object;
                    gorev.Id = item.Key;
                    gorevListesi.Add(gorev);
                }

                return gorevListesi;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata",
                    $"Görevler yüklenirken hata oluştu: {ex.Message}", "Tamam");
                return new ObservableCollection<Gorev>();
            }
        }

        public static async Task<bool> AddGorevAsync(Gorev gorev)
        {
            try
            {
                await firebaseClient
                    .Child("gorevler")
                    .PostAsync(gorev);
                return true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata",
                    $"Görev eklenirken hata oluştu: {ex.Message}", "Tamam");
                return false;
            }
        }

        public static async Task<bool> UpdateGorevAsync(string id, Gorev gorev)
        {
            try
            {
                await firebaseClient
                    .Child("gorevler")
                    .Child(id)
                    .PutAsync(gorev);
                return true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata",
                    $"Görev güncellenirken hata oluştu: {ex.Message}", "Tamam");
                return false;
            }
        }

        public static async Task<bool> DeleteGorevAsync(string id)
        {
            try
            {
                await firebaseClient
                    .Child("gorevler")
                    .Child(id)
                    .DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata",
                    $"Görev silinirken hata oluştu: {ex.Message}", "Tamam");
                return false;
            }
        }
    }
}