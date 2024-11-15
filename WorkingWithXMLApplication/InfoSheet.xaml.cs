
using System.Xml;
using System.Xml.Xsl;
using WorkingWithXMLApplication.ParsingStrategy;
namespace WorkingWithXMLApplication;

public partial class InfoSheet : ContentPage
{
    public static string xmlFilePath;
    public Schedule ScheduleData { get; set; }
    public InfoSheet(string? SelectedFilePath, IParsingStrategy parsingStrategy, 
        string? instructorName = null, 
        string? time = null, 
        string? courseTitle = null, 
        string? room = null,
        string? day = null)
    {
        xmlFilePath = SelectedFilePath;

        InitializeComponent();

        var result = ScheduleFilter.FilterSchedule(parsingStrategy.Parse(xmlFilePath),
            instructorName: instructorName, time: time, courseTitle: courseTitle,
            room: room, day: day);
        // Use the selected parsing method
        ScheduleData = result;
        BindingContext = ScheduleData;
    }

    private void OnHTMLTransormButtonClicked(object sender, EventArgs e)
    {
        string xmlFilePath = InfoSheet.xmlFilePath; // Ваш XML-файл
        string xsltFilePath = "C:\\Users\\vikto\\source\\repos\\Lab2\\schedule_transform.xsl"; // Ваш XSLT-файл
        string outputHtmlPath = "C:\\Users\\vikto\\source\\repos\\Lab2\\WorkingWithXMLApplication\\Temp\\output.html"; // Результуючий HTML-файл

        // Завантаження XSLT та створення трансформатора
        XslCompiledTransform xslt = new XslCompiledTransform();
        xslt.Load(xsltFilePath);

        // Виконання трансформації
        using (XmlReader reader = XmlReader.Create(xmlFilePath))
        using (XmlWriter writer = XmlWriter.Create(outputHtmlPath, xslt.OutputSettings))
        {
            xslt.Transform(reader, writer);
        }

        DisplayAlert("Успіх!", $"HTML файл був успішно створений за посиланням:\n\"{outputHtmlPath}\"", "Зрозуміло");
    }
}

