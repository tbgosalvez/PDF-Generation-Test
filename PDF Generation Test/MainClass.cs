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
            string fileName = @"\Tables.pdf";
            string fileName2 = @"\tables_other.pdf";
            string fileNameVideonTemplate = @"\Videon_Template.pdf";
        /*
            // basic elements
            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font timesH1 = new Font(bfTimes, 22, Font.BOLD, Color.BLACK);
            Font timesH2 = new Font(bfTimes, 14, Font.BOLD, Color.BLACK);
            Paragraph pTitle = new Paragraph("Product Data", timesH1);
            Paragraph spacer_0 = new Paragraph("\n\n");

            // generate tables
            OutputTable otParts = new OutputTable("PARTS");
            OutputTable otVendors = new OutputTable("VENDORS");
            OutputTable otShipment = new OutputTable("SHIPMENT");

            // Add elements to array and generate pdf
            IElement[] elements = 
            { 
                pTitle, spacer_0,
                otParts.getTable(), spacer_0,
                otVendors.getTable(), spacer_0,
                otShipment.getTable(), spacer_0
            };

            generateDocument(filePath + fileName, elements, true);


            // Generate different pdf
            PdfPTable[] outputTables =
            {
                otParts.getTable(),
                otVendors.getTable(),
                otShipment.getTable()
            };

           // generateDocument2(filePath+fileName2, outputTables, true);
         * 
         */
            fillFormPDF(filePath + fileNameVideonTemplate, filePath + @"\testStamper.pdf");
        }

        class OutputTable
        {
            string strTableName;
            int nRows, nCols, nTotalCells;

            List<string> arrProductNum;
            PdfPTable table;
            PdfPCell header;

            public PdfPTable getTable() { return table; }

            public OutputTable(string tn)
            {
                strTableName = tn;

                arrProductNum = TempDB.selectAllFrom(strTableName);

                nCols = TempDB.getFieldCount(strTableName);
                nTotalCells = arrProductNum.Count;
                //nRows = nTotalCells / nCols;

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


        public static void generateDocument(string filePath, IElement[] elements, bool isLandscape = false)
        {
            // Working document
            Document doc = new Document();
            if (isLandscape)
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }

        }

        public static void generateDocument2(string filePath, PdfPTable[] elements, bool isLandscape=false)
        {
            // Working document
            Document doc = new Document();
            if(isLandscape)
                doc.SetPageSize(PageSize.LETTER.Rotate());
            doc.SetMargins(0f, 0f, 0f, 0f);

            /* another way
             * 
             * 
            Document doc;
            if (isLandscape)
            {
                doc = new Document(PageSize.LETTER.Rotate(), 5f,5f,10f,10f);
            }
            else
            {
                doc = new Document(PageSize.LETTER);
            }
            */

            // PDF writer
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));

            // generic stuff
            Paragraph spacer_0 = new Paragraph("\n\n");
            Paragraph pLine0 = new Paragraph("hello");

            // Layout Tables
            PdfPTable tblOverviews = new PdfPTable(3);
            PdfPCell header = new PdfPCell(new Phrase("header"));
            header.Colspan = 3;
            header.HorizontalAlignment = 1;
            //tblOverviews.SpacingBefore = 0f;
            //tblOverviews.SpacingAfter = 0f;
            tblOverviews.AddCell(header);
            
            try
            {
                doc.Open();
                for (int i = 0; i < elements.Length; i++)
                {
                    // Each element is added sequentially
                    tblOverviews.AddCell(elements[i]);
                }
                doc.Add(tblOverviews);
                doc.Add(spacer_0);
                doc.Add(tblOverviews);

                doc.Close(); // throws IOException if no pages
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }

        }

        public static void fillFormPDF(string inFile, string outFile)
        {
            PdfReader pr = new PdfReader(inFile);
            PdfStamper stamp = new PdfStamper(pr, new FileStream(outFile, FileMode.Create));
            AcroFields form = stamp.AcroFields;
           
            // match fields with DataTable query results
            string[] arrTemp = new string[3];

            foreach (var item in form.Fields.Keys)
            {
                form.SetField(item.ToString(), "hello");

                Console.WriteLine(item);
            }

            Console.Read();

            
            stamp.Close();
        }
    }
}

   
