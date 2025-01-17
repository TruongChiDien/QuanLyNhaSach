﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyNhaSach.DA
{
    public class DataProvider
    {
        private static DataProvider instance;

        public static DataProvider Instance 
        {
          get { if (instance == null) instance = new DataProvider(); return instance; }
          private set => instance = value; 
        }

        private DataProvider() { }
        
        private string stringconnection = @"Data Source=MSI\ANHDUNGSQL;Initial Catalog=E:\QUANLYNHASACH\QLNS.MDF;Integrated Security=True";

        public DataTable ExecuteQuery(string query, object[] paramater =null)
        {
            DataTable data = new DataTable();

            using (SqlConnection connection = new SqlConnection(stringconnection))
            {
                connection.Open();
                SqlDataAdapter da;
                da = new SqlDataAdapter(query, connection);
                connection.Close();
                da.Fill(data);
            }

            return data;
        }

        public int ExecuteNonQuery(string query)
        {
            int i = 0;
            using (SqlConnection connection = new SqlConnection(stringconnection))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                i= command.ExecuteNonQuery();
                connection.Close();
            }
            return i;
        }

        public object ExecuteScalar(string query)
        {
            object i;
            using (SqlConnection connection = new SqlConnection(stringconnection))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                i = command.ExecuteScalar();
                connection.Close();
            }
            return i;
        }
    }
}
