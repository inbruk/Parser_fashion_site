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
    class OneCommodity
    {
        static public void Process(Browser browser, String brand, Div container, CommodityList comList)
        {
            var div1 = container.Div(Find.ByClass("products-list-item__brand"));

            String str = div1.Text;
            var fullName = str;

            var divs = container.Divs.Filter(Find.ByClass("size-text"));
            foreach (var currDiv in divs)
            {
                String tmp = currDiv.Text;

                String vendorCode = fullName + "-" + tmp;
                var currCom = new Commodity(vendorCode, fullName, 0);
                comList.UpdateOrAdd(currCom);

                Console.WriteLine();
                Console.WriteLine("            Спарсили товар (с 1 размером) " + vendorCode);
            }
        }

    }
}
