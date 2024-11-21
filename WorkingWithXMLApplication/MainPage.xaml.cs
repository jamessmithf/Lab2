using WorkingWithXMLApplication.ParsingStrategy;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Windows;
using System.Threading.Tasks;

namespace WorkingWithXMLApplication
{
    public partial class MainPage : ContentPage
    {
        private string? _selectedFilePath;
        private string? _filteredXmlFilePath;

        private string _selectedParsingMethod = "LINQ";
        private string SelectedParsingMethod { get { return _selectedParsingMethod; } }
        public string? SelectedFilePath { get { return _selectedFilePath; } }
        public MainPage()
        {
            InitializeComponent();
        }

        public static List<string?> GetUniqueValue(string filePath, string node, string atribute)
        {
            
            var xmlDocument = XDocument.Load(filePath);

            // Отримати унікальні значення атрибуту "atribute" з елементів "node"
            var uniqueValues = xmlDocument.Descendants(node)
                                .Select(course => (string)course.Attribute(atribute))
                                .Where(day => day != null) // Перевірка на null
                                .Distinct()
                                .ToList();

            return uniqueValues;
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
                HTMLTransorm.IsVisible = true;

                List<string?> uniqueDays = GetUniqueValue(SelectedFilePath, "Course", "Day");
                List<string?> uniqueRooms = GetUniqueValue(SelectedFilePath, "Course", "Room");

                DayPicker.Items.Add(" ");
                foreach (string day in uniqueDays)
                {
                    DayPicker.Items.Add(day);
                }

                RoomPicker.Items.Add(" ");
                foreach (string room in uniqueRooms)
                {
                    RoomPicker.Items.Add(room);
                }
#if WINDOWS
                Application.Current.Windows[0].Width = 750;
                Application.Current.Windows[0].Height = 770;
#endif
            }
        }

        public async void OnInfoButtonClicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Інформація про програму",
                $"Роботу виконав Сікора Віктор, студент групи К - 26" +
                $"\n\nПрограма забезпечує обробку XML-файлів (аналіз та трансформація) з використанням технологій LINQ, SAX та DOM." +
                $"\nОбраний варіант - \"Розклад\"", 
                "Зрозуміло", 
                "Давай чесно!");

            if (result == false)
            {
                await DisplayAlert("Інформація про програму",
                $"Роботу виконав GPT-4o, студент групи К - 26" +
                $"\n\nПрограма забезпечує обробку XML-файлів (аналіз та трансформація) з використанням технології GPT-4o." +
                $"\nОбраний варіант - \"Розклад\"" +
                $"\n\nP.S. Навіщо нам робити кнопку \"Загальна інформація\", якщо це надто проста задача для GPT-4o )))",
                "Погодитись оцінити на 10 балів");
            }
        }

        public void OnClearFiltersButtonClicked(object sender, EventArgs e)
        {
            CourseNameFilter.Text = string.Empty;
            InstructorNameFilter.Text = string.Empty;
            TimeFilter.Time = TimeSpan.Zero;
            RoomPicker.SelectedItem = " ";
            DayPicker.SelectedItem = " ";
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

            string? givenInstructorName = InstructorNameFilter.Text;
            string? givenTime = TimeFilter.Time == TimeSpan.Zero ? null : TimeFilter.Time.ToString();                        
            string? givenCourseTitle = CourseNameFilter.Text;
            string? givenRoom = RoomPicker.SelectedItem as string;
            string? givenDay = DayPicker.SelectedItem as string;


            var newResult = new InfoSheet(SelectedFilePath, selectedParsingStrategy, 
                instructorName: givenInstructorName, time: givenTime, courseTitle: givenCourseTitle,
                room: givenRoom, day: givenDay);

            await Navigation.PushAsync(newResult);
        }
        private async void OnHTMLTransormButtonClicked(object sender, EventArgs e)
        {
            // Отримуємо шлях до .xsl файлу
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".xsl" } }
            });

            var xslResult = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Оберіть необхідний .xsl файл",
                FileTypes = customFileType
            });

            if (xslResult != null)
            {
                string xslFilePath = xslResult.FullPath; // Посилання 

                // Завантаження XSL та створення трансформатора
                XslCompiledTransform xsl = new XslCompiledTransform();
                xsl.Load(xslFilePath);

                CancellationTokenSource TokenSource = new CancellationTokenSource();
                using var stream = new MemoryStream();
                try
                {
                    // Виконання трансформації та запис у MemoryStream
                    using (XmlReader reader = XmlReader.Create(SelectedFilePath))
                    using (XmlWriter writer = XmlWriter.Create(stream, xsl.OutputSettings))
                    {
                        xsl.Transform(reader, writer);
                    }

                    // Скидання позиції потоку до початку для подальшого читання
                    stream.Position = 0;

                    // Збереження результату у файл за допомогою FileSaver
                    var fileSaverResult = await FileSaver.Default.SaveAsync("TransformedReult.html", stream, TokenSource.Token);
                    fileSaverResult.EnsureSuccess();

                    await Toast.Make($"HTML файл збережено!").Show();
                }
                catch (Exception ex)
                {
                    await Toast.Make($"HTML файл не був збережений!").Show();
                }
            }
        }
    }
}
