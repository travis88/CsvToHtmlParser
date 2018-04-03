using TinyCsvParser.Mapping;

namespace CsvToHtmlParser
{
    /// <summary>
    /// Маппинг компании и парсера
    /// </summary>
    public class CsvCompanyMapping : CsvMapping<Company>
    {
        public CsvCompanyMapping() : base()
        {
            MapProperty(0, x => x.Name);
            MapProperty(1, x => x.Link);
            MapProperty(2, x => x.Image);
        }
    }
}
