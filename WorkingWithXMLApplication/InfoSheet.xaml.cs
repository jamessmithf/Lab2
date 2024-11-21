
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using WorkingWithXMLApplication.ParsingStrategy;
namespace WorkingWithXMLApplication;

public partial class InfoSheet : ContentPage
{
    public Schedule ScheduleData { get; set; }
    public InfoSheet(string? SelectedFilePath, IParsingStrategy parsingStrategy, 
        string? instructorName = null, 
        string? time = null, 
        string? courseTitle = null, 
        string? room = null,
        string? day = null)
    {

        InitializeComponent();

        var result = ScheduleFilter.FilterSchedule(parsingStrategy.Parse(SelectedFilePath),
            instructorName: instructorName, time: time, courseTitle: courseTitle,
            room: room, day: day);

        // Use the selected parsing method
        ScheduleData = result;
        BindingContext = ScheduleData;
    }
}

