using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Configuration;

using Parser.DAL_XLSFiles;


namespace Parser.BL_Commodities
{
    public class Commodity
    {

        public String Warehouse { set; get; }
        public String VendorCode { set; get; }
        public String Name { set; get; }
        public String Count { set; get; }
        public String RequiredCellsAreFilled { set; get; }
        public String GenerateLastCellFormula(int rowNumber)
        {
            var rnStr = rowNumber.ToString();
            var result = 
                "IF(AND($A"+ rnStr + "=\"\",$B" + rnStr + "=\"\",$D" + rnStr + "=\"\"),\"\",IF(AND(IF(OR($B" + rnStr + "<>\"\",$D" + rnStr + "<>\"\"),AND($A" + rnStr + "<>\"\",$B" + rnStr + "<>\"\",$D" + rnStr + "<>\"\"),1),OR($B" + rnStr + "<>\"\",$D" + rnStr + "<>\"\")),IF(COUNTIF(Инструкция!$B$9:$B$1000,$A" + rnStr + ")>0,\"Заполнены\",\"Не заполнены\"),IF(AND($A" + rnStr + "<>\"\",$B" + rnStr + "<>\"\",$D" + rnStr + "<>\"\"),\"Заполнены\",\"Не заполнены\")))";
            return result;
        }
        public Commodity(String vndorCode, String name, int rowNumber)
        {
            Warehouse = ConfigurationSettings.AppSettings["DefaultWarehouse"];
            VendorCode = vndorCode;
            Name = name;
            Count = "2";
            RequiredCellsAreFilled = GenerateLastCellFormula(rowNumber);
        }

        public Commodity(XLSFileRow inputRow, int rowNumber)
        {
            Warehouse = ConfigurationSettings.AppSettings["DefaultWarehouse"] ;
            VendorCode = inputRow.GetItemByIndex(1);
            Name = inputRow.GetItemByIndex(2);
            Count = inputRow.GetItemByIndex(3);
            RequiredCellsAreFilled = GenerateLastCellFormula(rowNumber);
        }

        public XLSFileRow ProduceOutputXLSRow()
        {
            XLSFileRow res = new XLSFileRow();

            res.AddItem(Warehouse);
            res.AddItem(VendorCode);
            res.AddItem(Name);
            res.AddItem(Count);
            res.AddItem(RequiredCellsAreFilled);

            return res;
        }

    }
}
