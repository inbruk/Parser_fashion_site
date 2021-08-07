using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Parser.DAL_XLSFiles
{
    public class XLSFileRow
    {
        private List<String> _items;
        public XLSFileRow()
        {
            _items = new List<String>();
        }
        public XLSFileRow(int startItemsCount)
        {
            _items = new List<String>();
            for(int i=0;i<startItemsCount;i++)
            {
                AddItem("");
            }
        }
        public XLSFileRow(IRow ir)
        {
            String value;
            _items = new List<String>();

            for (int i = 0; i < ir.Cells.Count; i++) 
            {
                ICell cell = ir.Cells[i];
                if (i==4)
                {
                    value = cell.CellFormula;
                }
                else
                {
                    value = cell.ToString();
                }                
                AddItem(value);
            }
        }

        public void AddItem(string currItem)
        {
            _items.Add(currItem);
        }
        public void SetItemByIndex(int index, String item)
        {
            _items[index] = item;
        }
        public String GetItemByIndex(int index)
        {
            return _items[index];
        }
        public int GetItemsCount()
        {
            return _items.Count;
        }

        public void Save2IRow(IRow ir)
        {
            for (int i = 0; i < _items.Count; i++ )
            {
                ICell cell = ir.CreateCell(i);

                if (i == 3)
                {
                    double num = Double.Parse(_items[i]);
                    cell.SetCellValue(_items[i]);
                    cell.CellStyle.Alignment = HorizontalAlignment.Right;
                }
                if (i == 4)
                {
                    cell.CellFormula = _items[i];
                    cell.CellStyle.Alignment = HorizontalAlignment.Left;
                }
                else
                {
                    cell.SetCellValue(_items[i]);
                }

            }
        }
    }
}
