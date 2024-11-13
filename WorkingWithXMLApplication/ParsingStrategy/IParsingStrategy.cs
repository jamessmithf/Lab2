namespace WorkingWithXMLApplication.ParsingStrategy
{
    public interface IParsingStrategy
    {
        Schedule Parse(string SelectedFilePath);
    }
}
