﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Reflection;

namespace PDF_Generation_Test
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = @"\Test.pdf";

            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font times = new Font(bfTimes, 12, Font.ITALIC, Color.RED);
            Paragraph p1 = new Paragraph("This is the first paragraph");
            Paragraph p2 = new Paragraph("This is the second paragraph\n\n", times);

            int numOfCols = 3;
            int numOfRows = 4;
            PdfPTable table = new PdfPTable(numOfCols);
            PdfPCell header = new PdfPCell(new Phrase("This is the table header"));
            header.Colspan = numOfCols;
            header.HorizontalAlignment = 1;
            table.AddCell(header);
            for (int i = 1; i <= numOfRows; i++)
            {
                for (int j = 1; j <= numOfCols; j++)
                {
                    table.AddCell("Row " + i + " Column " + j);
                }
            }
            IElement[] elements = { p1, p2, table };
            generateDocument(filePath + fileName, elements);
        }
        
        public static void generateDocument(string filePath, IElement[] elements)
        {
            Document doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();
            for (int i = 0; i < elements.Length; i++)
            {
                doc.Add(elements[i]);
            }
            doc.Close();
        }
    }
}
