
namespace WorkingWithXMLApplication
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);
            if (window != null)
            {
                window.Title = "Робота з .xml файлами";
                window.Width = 750;
                window.Height = 750;
            }

#if WINDOWS
        window.Created += (s, e) =>
        {
            var handle = WinRT.Interop.WindowNative.GetWindowHandle(window.Handler.PlatformView);
            var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);

            //and here it is
            appWindow.Closing += async (s, e) =>
            {
                e.Cancel = true;
                bool result = await App.Current.MainPage.DisplayAlert(
                    "Підтвердженння",
                    "Ви впевнені, що хочете завершити?",
                    "Так",
                    "Ні");

                if (result)
                {
                    App.Current.Quit();
                }
            };
        };
#endif
            return window;
        }

    }

}

