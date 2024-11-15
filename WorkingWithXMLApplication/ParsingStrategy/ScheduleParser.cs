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
            Schedule result = _parsingStrategy.Parse(SelectedFilePath);
            return result;
        }
    }
}
