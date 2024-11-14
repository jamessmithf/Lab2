using System.IO;
using static System.Net.Mime.MediaTypeNames;
using WorkingWithXMLApplication.ParsingStrategy;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;

namespace WorkingWithXMLApplication
{
    public partial class MainPage : ContentPage
    {
        private string? _selectedFilePath;

        private string _selectedParsingMethod = "LINQ";
        private string SelectedParsingMethod { get { return _selectedParsingMethod; } }
        public string? SelectedFilePath { get { return _selectedFilePath; } }
        public MainPage()
        {
            InitializeComponent();
        }

        public async void OnOpenFileButtonClicked(object sender, EventArgs e)
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[]{ ".xml" } }
            });
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Оберіть необхідний .xml файл",
                FileTypes = customFileType
            });

            if (result != null)
            {
                _selectedFilePath = result.FullPath;
                PathToFile.Text = $"Відкрито {SelectedFilePath}";

                FiltersMenu.IsVisible = true;
                ParsingTecnology.IsVisible = true;
                ParsingOptions.IsVisible = true;
                OpenScheduleButton.IsVisible = true;                
            }
        }

        private void OnParsingMethodChanged(object sender, CheckedChangedEventArgs e)
        {
            // Check if RadioButton's CheckedChanged event is called
            Console.WriteLine("CheckedChanged triggered");

            if (LINQRadioButton.IsChecked)
                _selectedParsingMethod = "LINQ";
            else if (SAXRadioButton.IsChecked)
                _selectedParsingMethod = "SAX";
            else if (DOMRadioButton.IsChecked)
                _selectedParsingMethod = "DOM";

            Console.WriteLine($"Selected parsing method: {_selectedParsingMethod}");
        }


        private async void OnOpenScheduleButtonClicked(object sender, EventArgs e)
        {           
            IParsingStrategy selectedParsingStrategy = SelectedParsingMethod switch
            {
                "LINQ" => new LINQParsingStrategy(),
                "SAX" => new SAXParsingStrategy(),
                "DOM" => new DOMParsingStrategy(),
                _ => throw new InvalidOperationException("Стратегія не вибрана")
            };

            await Navigation.PushAsync(new InfoSheet(SelectedFilePath, selectedParsingStrategy));
        }
    }
}
