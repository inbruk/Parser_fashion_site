using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using WatiN.Core;
using WatiN.Core.Native.InternetExplorer;

using Parser.BL_Commodities;

namespace Parser.PL_SiteParser
{
    class OnePage
    {
        static public void Process(Browser browser, String brand, Int32 pageNumber, CommodityList comList)
        {
            Console.WriteLine();
            Console.Write("        Парсим страницу " + pageNumber + "...");

            var divs = browser.Divs.Filter(Find.ByClass("products-list-item"));
            foreach (var div in divs)
                OneCommodity.Process(browser, brand, div, comList);

            Console.WriteLine("        Обработка страницы завершена");
        }
    }
}
