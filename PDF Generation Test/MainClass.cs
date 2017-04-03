using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PDF_Generation_Test
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string filePath = @"C:\Users\Tim\Desktop\";
            string fileName = "Test.pdf";

            List<IElement> elementsList = new List<IElement>();
            int numOfCols = 3;
            int numOfRows = 3;
            PdfPTable table = new PdfPTable(numOfCols);
            PdfPCell header = new PdfPCell(new Phrase("Table header"));
            header.Colspan = 3;
            header.HorizontalAlignment = 1;
            table.AddCell(header);
            for (int i = 1; i <= numOfRows; i++)
            {
                for (int j = 1; j <= numOfRows; j++)
                {
                    table.AddCell("Row " + i + " Col " + j);
                }
            }
            elementsList.Add(table);
            IElement[] elements = elementsList.ToArray();

            generateDocument(filePath+fileName, elements);
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
