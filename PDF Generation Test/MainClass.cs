using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            // Finds User's desktop directory
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = @"\Test.pdf";

            // Paragraphs
            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font times = new Font(bfTimes, 12, Font.ITALIC, Color.RED);
            Paragraph p1 = new Paragraph("This is the first paragraph");
            Paragraph p2 = new Paragraph("This is the second paragraph\n\n", times);

            // Programatically generate table
            List<string> arrProductNum = TempDB.selectAllFrom("PARTS");
            int nCols = TempDB.getFieldCount("PARTS");
<<<<<<< HEAD
            int nTotalCells = arrProductNum.Count;
            int nRows = nTotalCells/nCols;
=======
            int nRows = arrProductNum.Count/nCols;
>>>>>>> 479e61725ce10c6637c7911164070fa42e43b0f8

            PdfPTable table = new PdfPTable(nCols);
            PdfPCell header = new PdfPCell(new Phrase(@"(LocalDB)\v11"));
            
            header.Colspan = nCols;
            header.HorizontalAlignment = 1;
            table.AddCell(header);

<<<<<<< HEAD
            // fill table
            for (int i = 0; i < nTotalCells; i+=nCols)
            {
                for (int j = 0; j < nCols; j++)
                {
                    table.AddCell(arrProductNum[i + j]);
=======
            // PdfPTable is 1-based-indexed
            for (int i = 0; i <= nRows+1; i+=nCols)
            {
                for (int j = 1; j <= nCols+1; j++)
                {
                    // but everything else is 0-based
                    table.AddCell(arrProductNum[(i) + (j-1)]);
>>>>>>> 479e61725ce10c6637c7911164070fa42e43b0f8
                }
            }
            
            // Add elements to array and generate pdf
            IElement[] elements = { p1, p2, table };
            generateDocument(filePath + fileName, elements);
        }
        
        public static void generateDocument(string filePath, IElement[] elements, bool isLandscape=false)
        {
            // Working document
            Document doc = new Document();
            if(isLandscape)
                doc.SetPageSize(PageSize.LETTER.Rotate());

            // PDF writer
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            
            doc.Open();
            for (int i = 0; i < elements.Length; i++)
            {
                // Each element is added to document
                doc.Add(elements[i]);
            }
            doc.Close();
        }
    }
}
