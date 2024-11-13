using WorkingWithXMLApplication.ParsingStrategy;
namespace WorkingWithXMLApplication;

public partial class InfoSheet : ContentPage
{
    public Schedule ScheduleData { get; set; }
    public InfoSheet(string? SelectedFilePath, IParsingStrategy parsingStrategy)
    {
        InitializeComponent();

        // Use the selected parsing method
        ScheduleData = parsingStrategy.Parse(SelectedFilePath);

        BindingContext = ScheduleData;
    }
}

