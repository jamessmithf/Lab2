using WorkingWithXMLApplication.ParsingStrategy;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;

namespace WorkingWithXMLApplication
{
    public partial class MainPage : ContentPage
    {
        private string? _selectedFilePath;
        private string _selectedParsingMethod = "LINQ";

        public MainPage()
        {
            InitializeComponent();
        }

        public string SelectedParsingMethod => _selectedParsingMethod;
        public string? SelectedFilePath => _selectedFilePath;

        public static List<string?> GetUniqueValues(string filePath, string node, string attribute)
        {
            var xmlDocument = XDocument.Load(filePath);
            return xmlDocument.Descendants(node)
                              .Select(element => (string?)element.Attribute(attribute))
                              .Where(value => !string.IsNullOrEmpty(value))
                              .Distinct()
                              .ToList();
        }

        public async void OnInfoButtonClicked(object sender, EventArgs e)
        {
            string studentInfo = "Роботу виконав Сікора Віктор, студент групи К - 26\n" +
                                 "Програма забезпечує обробку XML-файлів (аналіз та трансформація) " +
                                 "з використанням технологій LINQ, SAX та DOM.\nОбраний варіант - \"Розклад\"";

            string gptInfo = "Роботу виконав GPT-4o, студент групи К - 26\n" +
                             "Програма забезпечує обробку XML-файлів (аналіз та трансформація) " +
                             "з використанням технології GPT-4o.\nОбраний варіант - \"Розклад\"\n\n" +
                             "P.S. Навіщо нам робити кнопку \"Загальна інформація\", " +
                             "якщо це надто проста задача для GPT-4o )))";

            bool result = await DisplayAlert("Інформація про програму", studentInfo, "Зрозуміло", "Давай чесно!");

            if (!result)
            {
                await DisplayAlert("Інформація про програму", gptInfo, "Погодитись оцінити на 10 балів");
            }
        }

        private async Task HandleFileSelectionAsync(string fileType, string pickerTitle, Action<string> onSuccess)
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { fileType } }
            });

            var result = await FilePicker.PickAsync(new PickOptions { PickerTitle = pickerTitle, FileTypes = customFileType });

            if (result?.FullPath is string filePath)
                onSuccess(filePath);
        }

        public async void OnOpenFileButtonClicked(object sender, EventArgs e)
        {
            await HandleFileSelectionAsync(".xml", "Оберіть XML файл", filePath =>
            {
                _selectedFilePath = filePath;
                PathToFile.Text = $"Відкрито {filePath}";
                ToggleUIElementsVisibility(true);

                PopulatePicker(DayPicker, GetUniqueValues(filePath, "Course", "Day"));
                PopulatePicker(RoomPicker, GetUniqueValues(filePath, "Course", "Room"));

#if WINDOWS
                Application.Current.Windows[0].Width = 750;
                Application.Current.Windows[0].Height = 770;
#endif
            });
        }

        private void ToggleUIElementsVisibility(bool isVisible)
        {
            FiltersMenu.IsVisible = isVisible;
            ParsingTecnology.IsVisible = isVisible;
            ParsingOptions.IsVisible = isVisible;
            OpenScheduleButton.IsVisible = isVisible;
            HTMLTransorm.IsVisible = isVisible;
        }

        private static void PopulatePicker(Picker picker, IEnumerable<string?> items)
        {
            picker.Items.Clear();
            picker.Items.Add(" ");
            foreach (var item in items.Where(i => !string.IsNullOrWhiteSpace(i)))
                picker.Items.Add(item);
        }

        public void OnClearFiltersButtonClicked(object sender, EventArgs e)
        {
            CourseNameFilter.Text = string.Empty;
            InstructorNameFilter.Text = string.Empty;
            TimeFilter.Time = TimeSpan.Zero;
            DayPicker.SelectedIndex = -1;
            RoomPicker.SelectedIndex = -1;
        }

        private void OnParsingMethodChanged(object sender, CheckedChangedEventArgs e)
        {
            _selectedParsingMethod = LINQRadioButton.IsChecked ? "LINQ" :
                                      SAXRadioButton.IsChecked ? "SAX" :
                                      DOMRadioButton.IsChecked ? "DOM" : _selectedParsingMethod;
        }

        private async void OnOpenScheduleButtonClicked(object sender, EventArgs e)
        {
            if (_selectedFilePath == null)
            {
                await DisplayAlert("Помилка", "Файл не вибрано", "ОК");
                return;
            }

            IParsingStrategy selectedParsingStrategy = _selectedParsingMethod switch
            {
                "LINQ" => new LINQParsingStrategy(),
                "SAX" => new SAXParsingStrategy(),
                "DOM" => new DOMParsingStrategy(),
                _ => throw new InvalidOperationException("Невідома стратегія парсингу")
            };

            var newResult = new InfoSheet(
                _selectedFilePath,
                selectedParsingStrategy,
                InstructorNameFilter.Text,
                TimeFilter.Time == TimeSpan.Zero ? null : TimeFilter.Time.ToString(),
                CourseNameFilter.Text,
                RoomPicker.SelectedItem as string,
                DayPicker.SelectedItem as string);

            await Navigation.PushAsync(newResult);
        }

        private async void OnHTMLTransformButtonClicked(object sender, EventArgs e)
        {
            await HandleFileSelectionAsync(".xsl", "Оберіть XSL файл", async xslFilePath =>
            {
                try
                {
                    if (_selectedFilePath == null)
                        throw new InvalidOperationException("Файл не вибрано");

                    using var stream = new MemoryStream();
                    var xsl = new XslCompiledTransform();
                    xsl.Load(xslFilePath);

                    using (var reader = XmlReader.Create(_selectedFilePath))
                    using (var writer = XmlWriter.Create(stream, xsl.OutputSettings))
                    {
                        xsl.Transform(reader, writer);
                    }

                    stream.Position = 0;
                    var saveResult = await FileSaver.Default.SaveAsync("TransformedResult.html", stream);
                    saveResult.EnsureSuccess();

                    await Toast.Make("HTML файл збережено!").Show();
                }
                catch (Exception ex)
                {
                    await Toast.Make($"Файл не збережено!\n{ex.Message}").Show();
                }
            });
        }
    }
}
