namespace GorselProgramlamaOdev3
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
           
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("AnaSayfa", typeof(AnaSayfa));
            Routing.RegisterRoute("HaberDetay", typeof(HaberDetay));
            Routing.RegisterRoute("Haberler", typeof(Haberler));
            Routing.RegisterRoute("HavaDurumu", typeof(HavaDurumu));
            Routing.RegisterRoute("DovizKurlari", typeof(DovizKurlari));
            Routing.RegisterRoute("Yapilacaklar", typeof(Yapilacaklar));
            Routing.RegisterRoute("GorevDetay", typeof(GorevDetay));
            Routing.RegisterRoute("Ayarlar", typeof(Ayarlar)); 
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // direkt login page sayfasına atıyor
            await Current.GoToAsync("//LoginPage");
        }
    }
}

