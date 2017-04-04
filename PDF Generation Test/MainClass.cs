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
            //string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = "Tables.pdf";

            // basic elements
            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font timesH1 = new Font(bfTimes, 22, Font.BOLD, Color.BLACK);
            Font timesH2 = new Font(bfTimes, 14, Font.BOLD, Color.BLACK);
            Paragraph pTitle = new Paragraph("Product Data", timesH1);
            Paragraph spacer_0 = new Paragraph("\n\n");

            // generate csv
            TempDB.tableToCSV("SAMPLE_TESTS", "sample_tests.csv");

            // generate plots & such
            TempDB.R_Plots("sample_tests.csv");

            // generate tables
            OutputTable otParts = new OutputTable("PARTS");
            OutputTable otVendors = new OutputTable("VENDORS");
            OutputTable otShipment = new OutputTable("SHIPMENT");
            OutputTable otSampleTests = new OutputTable("SAMPLE_TESTS");

            // Add elements to array and generate pdf
            IElement[] elements = 
            { 
                pTitle, spacer_0,
                otParts.getTable(), spacer_0,
                otVendors.getTable(), spacer_0,
                otShipment.getTable(), spacer_0,
                otSampleTests.getTable()
            };

            generateDocument(fileName, elements, true);
        }

        class OutputTable
        {
            string strTableName;
            int nRows, nCols, nTotalCells;

            List<string> arrProductNum;
            PdfPTable table;
            PdfPCell header;

            public List<string> getListProductNum() { return arrProductNum; }
            public PdfPTable getTable() { return table; }

            public OutputTable(string tn)
            {
                strTableName = tn;

                arrProductNum = TempDB.selectAllFrom(strTableName);

                nCols = TempDB.getFieldCount(strTableName);
                nTotalCells = arrProductNum.Count;
                nRows = nTotalCells / nCols;

                table = new PdfPTable(nCols);
                header = new PdfPCell(new Phrase(strTableName));
            
                // fill table
                header.Colspan = nCols;
                header.HorizontalAlignment = 1;
                table.AddCell(header);
                for (int i = 0; i < nTotalCells; i += nCols)
                {
                    for (int j = 0; j < nCols; j++)
                    {
                        table.AddCell(arrProductNum[i + j]);
                    }
                }
            }
        }


        public static void generateDocument(string filePath, IElement[] elements, bool isLandscape=false)
        {
            // Working document
            Document doc = new Document();
            if(isLandscape)
                doc.SetPageSize(PageSize.LETTER.Rotate());

            // PDF writer
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            
            try
            {
                doc.Open();
                for (int i = 0; i < elements.Length; i++)
                {
                    // Each element is added sequentially
                    doc.Add(elements[i]);
                }
                doc.Close(); // throws IOException if no pages
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }

        }
    }
}
