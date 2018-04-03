using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyCsvParser;

namespace CsvToHtmlParser
{
    /// <summary>
    /// Парсер из csv в html
    /// </summary>
    class Program
    {
        /// <summary>
        /// Максимальный размер буфера
        /// </summary>
        const int MAX_BUFFER_SIZE = 2048;

        /// <summary>
        /// Кодировка
        /// </summary>
        static Encoding enc8 = Encoding.UTF8;

        static void Main(string[] args)
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';');
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvCompanyMapping csvMapper = new CsvCompanyMapping();
            CsvParser<Company> csvParser = new CsvParser<Company>(csvParserOptions, csvMapper);

            // директория с исходником
            string pathFrom = System.Configuration.ConfigurationManager.AppSettings["From"];
            // директория для сохранения
            string pathTo = System.Configuration.ConfigurationManager.AppSettings["To"];

            string fileName = $"{pathFrom}list1.csv";

            FileStream fStream = new FileStream(fileName, FileMode.Open);
            string contents = null;

            if (fStream.Length <= MAX_BUFFER_SIZE)
            {
                Byte[] bytes = new Byte[fStream.Length];
                fStream.Read(bytes, 0, bytes.Length);
                contents = enc8.GetString(bytes);
            }
            else
            {
                contents = ReadFromBuffer(fStream);
            }
            fStream.Close();

            var list = csvParser.ReadFromString(csvReaderOptions, contents).ToList();
            string appendText = GetResultString(list);
            File.WriteAllText($"{pathTo}list1.html", appendText);
        }

        /// <summary>
        /// Считывает файл
        /// </summary>
        /// <param name="fStream"></param>
        /// <returns></returns>
        private static string ReadFromBuffer(FileStream fStream)
        {
            Byte[] bytes = new Byte[MAX_BUFFER_SIZE];
            string output = String.Empty;
            Decoder decoder8 = enc8.GetDecoder();

            while (fStream.Position < fStream.Length)
            {
                int nBytes = fStream.Read(bytes, 0, bytes.Length);
                int nChars = decoder8.GetCharCount(bytes, 0, nBytes);
                char[] chars = new char[nChars];
                nChars = decoder8.GetChars(bytes, 0, nBytes, chars, 0);
                output += new String(chars, 0, nChars);
            }
            return output;
        }

        /// <summary>
        /// Возвращает результирующую строку
        /// </summary>
        /// <param name="companies"></param>
        /// <returns></returns>
        private static string GetResultString(IEnumerable<TinyCsvParser.Mapping.CsvMappingResult<Company>> companies)
        {
            string res = "<div class=\"row\">\n";
            foreach (var company in companies)
            {
                res += "<div class=\"col-xs-12 col-sm-6 col-md-4 col-lg-3 yk-list-item\">\n";
                if (!String.IsNullOrWhiteSpace(company.Result.Link))
                    res += $"<a title=\"\" href=\"{company.Result.Link}\" target=\"_blank\">\n";
                res += $"<img title=\"\" src=\"{company.Result.Image.Replace("шаблон", "/usercontent/insoc/content/yk/logo_33.png")}\" alt=\"\" width=\"155\" height=\"120\" />\n";
                if (!String.IsNullOrWhiteSpace(company.Result.Link))
                    res += "</a>\n";
                res += $"<div>{company.Result.Name}</div>\n";
                res += "</div>\n";
            }
            res += "</div>\n";

            return res;
        }
    }
}
