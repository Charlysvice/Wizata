using Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace Client
{
    public static class DBHelper
    {
        static String connectionString = @"Server=tcp:charlysvice.database.windows.net,1433;Initial Catalog=tiragenoel;Persist Security Info=False;User ID=wizata;Password=Password!2020;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public static int GetLastBatchId()
        {
            int batchid = 0;
            List<BatchInfo> batchInfos = new List<BatchInfo>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT TOP (1) batchid FROM batches ORDER BY batchid DESC";
                
                DataTable table = new DataTable();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }

                foreach (DataRow row in table.Rows)
                {
                    batchid = (int)row["batchid"];
                  
                }
            }
            
            return batchid;
        }

    }
}
