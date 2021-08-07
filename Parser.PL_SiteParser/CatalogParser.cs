using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;

using WatiN.Core;
using WatiN.Core.Native.InternetExplorer;

using Parser.BL_Commodities;

namespace Parser.PL_SiteParser
{
    public class CatalogParser : IDisposable
    {
        private String _brandCatalogURL;
        private Int32 _pageTimeout;
        private Browser _currBrowser;

        public CatalogParser()
        {
            _brandCatalogURL = ConfigurationSettings.AppSettings["BrandCatalogURL"];
            String pageTimeoutStr = ConfigurationSettings.AppSettings["PageTimeout"];
            _pageTimeout = Int32.Parse(pageTimeoutStr);

            // запустим браузер через библиотеку WatIn
            _currBrowser = new IE();
            _currBrowser.BringToFront();
        }

        void CloseBrowser()
        {
            _currBrowser.Close();
            _currBrowser = null;
        }

        public void Process(List<String> brandList, CommodityList comList)
        {
            Console.WriteLine();
            Console.Write("Начинаем парсить сайт (совмещая данные)...");

            try
            {
                foreach (var currBrand in brandList)
                    OneBrand.Process(_currBrowser, currBrand, comList);
            }
            catch (Exception e)
            {
                Console.WriteLine("    Ошибка - во время спарсивания и совмещения данных произошло исключение ! " + e.Source + " " + e.Message);
                CloseBrowser();
                Console.ReadKey();
                Environment.Exit(-4);
            }

            CloseBrowser();
            Console.WriteLine();
            Console.WriteLine("Парсинг завершен");
        }

        // ----------------------------------------------------------------- все остальное, только для правильной деструкции ---------------------------------------------------------------

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed and unmanaged resources.
                if (disposing)
                {
                    if (_currBrowser != null)
                    {
                        CloseBrowser();
                    }
                }
                // Note disposing has been done.
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        ~CatalogParser()
        {
            Dispose(false);
        }
    }
}
