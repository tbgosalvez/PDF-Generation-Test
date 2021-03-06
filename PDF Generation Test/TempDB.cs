﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
