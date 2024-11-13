namespace WorkingWithXMLApplication.ParsingStrategy
{
    public class ScheduleParser
    {
        private IParsingStrategy _parsingStrategy;

        public void SetParsingStrategy(IParsingStrategy parsingStrategy)
        {
            _parsingStrategy = parsingStrategy;
        }

        public Schedule Parse(string SelectedFilePath)
        {
            return _parsingStrategy.Parse(SelectedFilePath);
        }
    }
}
