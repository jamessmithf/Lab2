using WorkingWithXMLApplication.ParsingStrategy;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

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
    }
}
