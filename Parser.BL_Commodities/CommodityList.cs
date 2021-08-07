using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Parser.DAL_XLSFiles;

namespace Parser.BL_Commodities
{
    public class CommodityList
    {
        private List<Commodity> _items;
        public List<Commodity> Items       
        {
            get 
            {
                return _items;
            }
        }

        public void UpdateOrAdd(Commodity newCom)
        {
            Commodity exCom = _items.FirstOrDefault( x => x.VendorCode.Trim().Equals(newCom.VendorCode.Trim()) );
            // Commodity exCom = _items.FirstOrDefault(x => x.VendorCode.Equals(newCom.VendorCode));

            if ( exCom!=null )
            {
                exCom.Count = "2";
            }
            else
            {
                newCom.RequiredCellsAreFilled = newCom.GenerateLastCellFormula(_items.Count+2);
                _items.Add(newCom);
            }
        }

        public void FillAllWithZeroCounts()
        {
            foreach(Commodity currItem in _items)
            {
                currItem.Count = "0";
            }
        }

        public CommodityList()
        {
            _items = new List<Commodity>();
        }

        public void LoadFromXSLFile(String csvFileName)
        {
            var parsedDataFile = new XLSFile(csvFileName);
            parsedDataFile.SkipRow(); // пропускаем заголовок

            while (true)
            {
                XLSFileRow currInputRow = parsedDataFile.GetRow();

                // проверим на окончание входного файла, и выйдем если так
                if (currInputRow == null || currInputRow.GetItemsCount()==0) break;

                // такие строки считаем пустыми или битыми
                if (currInputRow.GetItemsCount() != 5)
                    continue; // и пропускаем их

                var currCommodity = new Commodity(currInputRow, parsedDataFile.CurrRowNumber);
                _items.Add(currCommodity);

            }
            parsedDataFile.Close();
            parsedDataFile = null;
        }

        public void SaveToXLSFile(String fileName)
        {
            var parsedDataFile = new XLSFile(fileName);
            parsedDataFile.RemoveRowsAfterHeader();
            parsedDataFile.SkipRow(); // пропускаем заголовок

            foreach (Commodity currCom in _items)
            {
                var currRow = currCom.ProduceOutputXLSRow();
                parsedDataFile.PutRow(currRow);
            }

            parsedDataFile.Save(fileName);
            parsedDataFile.Close();
            parsedDataFile = null;
        }
    }
}
