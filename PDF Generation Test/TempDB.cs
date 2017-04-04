using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;

namespace PDF_Generation_Test
{
    static class TempDB
    {
        /* tempdb tables:
         * 
         * VENDORS
         * PARTS 
         * SHIPMENT
         * 
        */

        // DB Connection Stuff
        static string connectionString = Properties.Settings.Default.tempdbConnectionString;
        static SqlConnection sqlConnect = new SqlConnection(connectionString);

        // functions
        public static int getFieldCount(string table)
        {
            int result = 0;


            sqlConnect.Open();

            SqlCommand query = new SqlCommand("SELECT * FROM " + table, sqlConnect);

            SqlDataReader reader = query.ExecuteReader();

            try
            {
                reader.Read();
                result = reader.FieldCount;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message+'\n'+e.StackTrace);
            }
            finally
            {
                reader.Close();
            }

            sqlConnect.Close();

            return result;
        }

        public static List<string> selectAllFrom(string table)
        {
            List<string> arrResult = new List<string>();
            SqlDataReader reader = null;

            sqlConnect.Open();

            SqlCommand query = new SqlCommand("SELECT * FROM " + table, sqlConnect);

            try
            {
                reader = query.ExecuteReader();
                for (int i = 0; i < reader.FieldCount; i++)
                    arrResult.Add(reader.GetName(i));
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        /* getting data:
                         * can GetInt32, GetString, GetDateTime, etc.
                         *   or
                         * can GetValue().ToString() and parse later.
                         * both equal-opportunity choices.
                         */
                        Console.WriteLine(reader.GetValue(i).ToString());
                        arrResult.Add(reader.GetValue(i).ToString());
                    }
                }         
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message + '\n' + e.StackTrace);
            }

            if(reader!=null)
                reader.Close();

            sqlConnect.Close();

            return arrResult;
        }

        public static void tableToCSV(string table, string outfile)
        {
            if (!File.Exists(outfile + ".old"))
                File.Create(outfile + ".old");
            if (!File.Exists(outfile + ".old.backup"))
                File.Create(outfile + ".old.backup");

            if(File.Exists(outfile))
            {
                try
                {
                    File.Replace(outfile, outfile+".old", outfile+".old.backup");
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            // write to current directory: (project)\bin\Debug
           // StreamWriter writer = new StreamWriter(outfile, true);
            StringBuilder sb = new StringBuilder();
            SqlDataReader reader = null;

            sqlConnect.Open();

            SqlCommand query = new SqlCommand("SELECT * FROM " + table, sqlConnect);

            try
            {
                reader = query.ExecuteReader();

                // write column headers
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (i > 0)
                        sb.Append(",");
                   sb.Append("\"" + reader.GetName(i)+"\"");
                }
                sb.AppendLine();

                // write field values
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        /* getting data:
                         * can GetInt32, GetString, GetDateTime, etc.
                         *   or
                         * can GetValue().ToString() and parse later.
                         * both equal-opportunity choices.
                         */
                        if (i > 0)
                            sb.Append(",");
                        sb.Append("\"" + reader.GetValue(i).ToString() + "\"");
                    }
                    sb.AppendLine();
                }
                sb.AppendLine();

                File.WriteAllText(outfile, sb.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + '\n' + e.StackTrace);
            }

            if (reader != null)
                reader.Close();

           // writer.Close();
            //writer.Dispose();

            sqlConnect.Close();

        }

        public static void R_Plots(string strFile)
        {
            if (!File.Exists(strFile))
                return;
            else
            {
                REngine.SetEnvironmentVariables();

                REngine rEng = REngine.GetInstance();

                rEng.Evaluate("dd<-read.csv('" + strFile + "', header=TRUE, sep=',')" );
                rEng.Evaluate("attach(dd)");

                // stuff

                /* good to have:
                 *      ggplot2
                 *      sqldf
                 *      gridExtra
                 */

                rEng.Evaluate("plot(1:12,qPassed,main='Wire Supplier', xlab='Month',ylab='# passed testing', type='b')");
                rEng.Evaluate("dev.copy(pdf, 'plotSampleTests.pdf')");
                rEng.Evaluate("dev.off()");

                rEng.Evaluate("detach(dd)");

                rEng.Dispose();
            }
        }
    }
}
