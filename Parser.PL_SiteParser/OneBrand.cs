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
    class OneBrand
    {
        protected static String _brandPageStartPartOfURL;
        protected static Int32 _pageTimeout;
        static OneBrand()
        {
            _brandPageStartPartOfURL = ConfigurationSettings.AppSettings["BrandPageStartPartOfURL"];
            String pageTimeoutStr = ConfigurationSettings.AppSettings["PageTimeout"];
            _pageTimeout = Int32.Parse(pageTimeoutStr);
        }

        static String PrepareBrandPartOfURL(String inStr)
        {
            String res = inStr.Trim().ToLower().Replace(' ', '-');
            return res;
        }

        static void MoveToBrandPage(Browser browser, String brand)
        {
            String brandPartOfURL = PrepareBrandPartOfURL(brand);

            browser.GoTo(_brandPageStartPartOfURL + brandPartOfURL);
            browser.WaitForComplete();

            Thread.Sleep(_pageTimeout);
        }

        protected static void ChangePerPageCountToMaximum(Browser browser)
        {
            var span1 = browser.Span(Find.ByClass("products-catalog__sort catalog-onpage button-dropdown"));
            var span2 = span1.Span(Find.ByClass("select"));
            var list = span2.List(Find.ByClass("dropdown"));
            list.Click();
            browser.WaitForComplete();

            var listItem = list.OwnListItem(Find.By("data-sort", "108"));
            listItem.Click();
            browser.WaitForComplete();

            var div = browser.Div(Find.ByClass("title total-text"));
            div.Click();
            browser.WaitForComplete();
        }

        protected static Span GetNextButton(Browser browser)
        {
            var span = browser.Span(Find.ByClass("button button_s button_outline paginator__next"));
            return span;
        }

        protected static Boolean IsNextButtonExists(Span span)
        {
            return span.Exists;
        }
        static Boolean BrandPageWasLoaded(Browser browser)
        {
            var span1 = browser.Span(Find.ByClass("products-catalog__sort catalog-onpage button-dropdown"));
            if (span1 != null && span1.Exists == true)
                return true;
            else
                return false;
        }
        static public void Process(Browser browser, String brand, CommodityList comList)
        {
            Console.WriteLine();
            Console.Write("    Парсим бренд - " + brand + " ...");

            MoveToBrandPage(browser, brand);
            if (BrandPageWasLoaded(browser))
            {
                ChangePerPageCountToMaximum(browser);

                Int32 pageNumber = 1;
                Boolean isNextButtonExists = false;
                do
                {
                    OnePage.Process(browser, brand, pageNumber, comList);

                    Span span = GetNextButton(browser);
                    isNextButtonExists = IsNextButtonExists(span);
                    if (isNextButtonExists)
                    {
                        span.Click();
                        browser.WaitForComplete();
                        Thread.Sleep(_pageTimeout);
                    }
                    pageNumber++;
                }
                while (isNextButtonExists);

                Console.WriteLine("    Обработка бренда завершена");
            }
            else
                Console.WriteLine("    Такой бренд на сайте отсутствует");

        }
    }
}
