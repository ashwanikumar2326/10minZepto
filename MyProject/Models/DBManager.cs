using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Net.Configuration;

namespace MyProject.Models
{
    public class DBManager
    {
        SqlConnection connection = new SqlConnection("Data Source=ASHWIN1222\\SQLEXPRESS;Initial Catalog=Zepto;Integrated Security=True");
        public int ExecuteInsertUpdateDelet(string query)
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
           int result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        public DataTable ExecuteSelect(string query)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(query,connection);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }
}