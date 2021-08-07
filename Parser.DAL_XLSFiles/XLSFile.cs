using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace Parser.DAL_XLSFiles
{
    public class XLSFile
    {        
        IWorkbook wb;
        ISheet ws;
        int currRowNumber;
        public int CurrRowNumber { get { return currRowNumber; } }
        public XLSFile(String fileName)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                wb = new HSSFWorkbook(file);
                ws = wb.GetSheetAt(1);  // Внимание ! Всегда работаем со второй страницей...
            }
            currRowNumber = 0; // начинается все с 1 строки (там заголовок)
        }
        public void SkipRow()
        {
            currRowNumber++;
        }
        public XLSFileRow GetRow()
        {
            IRow ir = ws.GetRow(currRowNumber);
            if (ir != null)
            {
                var xRow = new XLSFileRow(ir);
                currRowNumber++;
                return xRow;
            }

            return null;
        }
        public void PutRow(XLSFileRow xRow)
        {
            IRow ir = ws.CreateRow(currRowNumber);
            xRow.Save2IRow(ir);
            currRowNumber++;
        }
        public void RemoveRowsAfterHeader()
        {
            for(int i=2; i<=ws.LastRowNum; i++)
            {
                IRow ir = ws.GetRow(i);
                ws.RemoveRow(ir);
            }
        }
        public void Save(String fileName)
        {
            // тут создаем дроп даун листы на 1 колонке
            CellRangeAddressList rangeList = new CellRangeAddressList();
            rangeList.AddCellRangeAddress(new CellRangeAddress(1, ws.LastRowNum, 0, 0));
            DVConstraint dvconstraint = DVConstraint.CreateFormulaListConstraint("Инструкция!$B$11:$B$14");
            HSSFDataValidation dataValidation = new HSSFDataValidation(rangeList, dvconstraint);
            ws.AddValidationData(dataValidation);

            ws.ForceFormulaRecalculation = true;

            using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                wb.Write(file);
            }
        }
        public void Close()
        {
            ws = null;
            wb = null;
        }
    }
}
