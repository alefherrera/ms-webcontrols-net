using System.IO;
using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using ClosedXML.Excel;

namespace AjaxServerControl
{
    public class ExcelDocument
    {

        public GridView Grid { get; set; }

        public List<string> Letras { get; set; }

        public ExcelDocument(GridView Grid)
        {
            this.Grid = Grid;
            string[] rango = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            Letras = new List<string>();
            Letras.AddRange(rango);
        }

        public byte[] CreateExcel()
        {
            var memoryStream = new MemoryStream();
            CreateParts(memoryStream);
            return memoryStream.ToArray();
        }

        public void CreateParts(MemoryStream memoryStream)
        {
            int[] Celda = new int[Grid.Columns.Count];
            int tiempo = Environment.TickCount;
            var workbook = new XLWorkbook();
            System.Threading.Thread th;
            var worksheet = workbook.Worksheets.Add("Excel");
            List<System.Threading.Thread> ll = new List<System.Threading.Thread>();
            for (int j = 0; j < Grid.Columns.Count; j++)
            {
                int MaxporCol = 0;
                Columna CurrentCol;
                Celda[j] = 1;
                var celdaHeader = worksheet.Cell(Letras[j] + "1");
                celdaHeader.SetValue(Grid.HeaderRow.Cells[j].Text);
                celdaHeader.DataType = XLCellValues.Text;
                celdaHeader.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                celdaHeader.Style.Font.Bold = true;
                celdaHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                celdaHeader.Style.Fill.BackgroundColor = XLColor.LightGray;
                MaxporCol = celdaHeader.Value.ToString().Length;

                for (int i = 0; i < Grid.Rows.Count; i++)
                {
                    var rowNumber = (i + 2).ToString();
                    var cell = worksheet.Cell(Letras[j] + rowNumber);
                    

                    cell.SetValue(Grid.Rows[i].Cells[j].Text);
                    cell.DataType = XLCellValues.Text;
                    cell.Style.Fill.BackgroundColor = XLColor.FromColor(Grid.Rows[i].Cells[j].BackColor);
                    cell.Style.Alignment.WrapText = true;
                    cell.Style.Font.FontColor = XLColor.FromColor(Grid.Rows[i].Cells[j].ForeColor);
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    if (cell.Value.ToString().Length > MaxporCol)
                    {
                        MaxporCol = cell.Value.ToString().Length;
                        Celda[j] = i+2;
                    }

                }
                CurrentCol = new Columna(worksheet.Column(Letras[j]), Celda[j]);
                th = new System.Threading.Thread(CurrentCol.Do);
                ll.Add(th);
                th.Start();
            }
            foreach (System.Threading.Thread t in ll)
            {
                t.Join();
            }
            tiempo -= Environment.TickCount;
            workbook.SaveAs(memoryStream);
        }

    }

    public class Columna 
    {
        public Columna(IXLColumn col, int index)
        {
            this.col = col;
            this.index = index;
        }
        public void Do()
        {
            col.AdjustToContents(index,index);
        }
        public IXLColumn col;
        public int index;
    }
}
