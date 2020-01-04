using Common;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Helpers
{
    public static class DBHelper
    {
        static String connectionString = @"Server=tcp:charlysvice.database.windows.net,1433;Initial Catalog=tiragenoel;Persist Security Info=False;User ID=wizata;Password=Password!2020;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
      
        public static void AddBatch(BatchInfo batchInfo)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                StringBuilder sb = new StringBuilder();
                command.CommandText = "INSERT INTO batches(batchid,datajson,timestamp) VALUES(@batchid,@batchjson,@timestamp)";

                command.Parameters.AddWithValue("@batchid", batchInfo.BatchId);
                command.Parameters.AddWithValue("@batchjson", JObject.FromObject(batchInfo).ToString());
                command.Parameters.AddWithValue("@timestamp", batchInfo.TimeStamp);

                command.ExecuteNonQuery();
            }
        }

        public static List<BatchInfo> GetLastXBatches(int count)
        {
            List<BatchInfo> batchInfos = new List<BatchInfo>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT TOP (@count) datajson FROM batches ORDER BY timestamp DESC";
                command.Parameters.AddWithValue("@count", count);
                DataTable table = new DataTable();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }
                foreach (DataRow row in table.Rows)
                {
                    String batchjson = (String)row["datajson"];
                    JObject jObject = JObject.Parse(batchjson);
                    BatchInfo batchInfo = jObject.ToObject<BatchInfo>();
                    batchInfos.Add(batchInfo);
                }
            }
            batchInfos.Reverse();
            return batchInfos;
        }
    }
}
