using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

using Parser.PL_SiteParser;
using Parser.BL_Commodities;
using Parser.DAL2CSVFiles;

namespace Parser.App
{
    class Program
    {      
        static List<String> GetBrandList()
        {
            var result = new List<String>();

            try
            {
                Console.WriteLine();
                Console.Write("Грузим список брендов из csv файла...");

                String brandListCSVFileName = ConfigurationSettings.AppSettings["BrandListCSVFile"];
                if (!File.Exists(brandListCSVFileName))
                {
                    Console.WriteLine("    Ошибка - файл со списком брендов не найден!");
                    Console.ReadKey();
                    Environment.Exit(-1);
                }
                var brandListFile = new InputCSVFile(brandListCSVFileName);

                var firstTime = true;
                while (true)
                {
                    CSVFileRow currInputCSVRow = brandListFile.GetNextRow();

                    // проверим на окончание входного файла, и выйдем если так
                    if (currInputCSVRow == null) break;

                    // такие строки считаем битыми
                    if (currInputCSVRow.GetItemsCount() != 1)
                    {
                        Console.WriteLine("    Предупреждение - пустая строка или строка неверного формата ! Пропускаем ее");
                        continue; // и пропускаем их
                    }

                    // пропускаем заголовок
                    if (firstTime)
                    {
                        firstTime = false;
                        continue;
                    }

                    String value = currInputCSVRow.GetItemByIndex(0);
                    result.Add(value.Trim());

                }
                brandListFile.Close();
                brandListFile = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(" - во время загрузки случилось исключение ! " + e.Source + " " + e.Message);
                Console.ReadKey();
                Environment.Exit(-2);
            }

            Console.WriteLine("Завершено");
            return result;
        }

        static CommodityList LoadPreviouslyParsedData()
        {
            CommodityList result = new CommodityList();

            try
            {
                Console.WriteLine();
                Console.Write("Грузим данные, которые спарсили раньше, из xls файла...");

                String parsedDataXLSFileName = ConfigurationSettings.AppSettings["ParsedDataXLS"];
                if (!File.Exists(parsedDataXLSFileName))
                {
                    Console.WriteLine("    Файл с ранее загруженными данными не найден. Значит ничего не грузим.");
                    return result;
                }
                result.LoadFromXSLFile(parsedDataXLSFileName);
            }
            catch (Exception e)
            {
                Console.WriteLine("    Ошибка - во время загрузки случилось исключение ! " + e.Source + " " + e.Message);
                Console.ReadKey();
                Environment.Exit(-3);
            }

            Console.WriteLine("Завершено");
            return result;
        }

        static void SaveParsedData(CommodityList comList)
        {
            try
            {
                Console.WriteLine();
                Console.Write("Сохраняем данные в xls файл...");

                String parsedDataXLSFileName = ConfigurationSettings.AppSettings["ParsedDataXLS"];
                comList.SaveToXLSFile(parsedDataXLSFileName);
            }
            catch (Exception e)
            {
                Console.WriteLine("    Ошибка - во время сохранения случилось исключение ! " + e.Source + " " + e.Message);
                Console.ReadKey();
                Environment.Exit(-4);
            }

            Console.WriteLine("Завершено");
        }

        [STAThread]
        static void Main(string[] args)
        {
            var brandList = GetBrandList();
            var comList = LoadPreviouslyParsedData();
            comList.FillAllWithZeroCounts();

            var catPars = new CatalogParser();
            catPars.Process(brandList, comList);
            catPars = null;

            SaveParsedData(comList);

            Console.WriteLine();
            Console.WriteLine();
            Console.Write("Завершено.");
            Console.ReadKey();
        }
    }
}
