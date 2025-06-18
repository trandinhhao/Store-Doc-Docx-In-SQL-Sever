using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WindowsFormsApp.Model;

namespace WindowsFormsApp.Repository
{
    public class TableCellImageRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";


        public void SaveTableCellImage(TableCellImages tableCellImage)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO TableCellImages (TableCellId, OrderInTableCell, ImagePath, ImageType, Title, Style)
                              VALUES (@TableCellId, @OrderInTableCell, @ImagePath, @ImageType, @Title, @Style)";
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@TableCellId", tableCellImage.TableCellId);
                cmd.Parameters.AddWithValue("@OrderInTableCell", tableCellImage.OrderInTableCell);
                cmd.Parameters.AddWithValue("@ImagePath", tableCellImage.ImagePath);
                cmd.Parameters.AddWithValue("@ImageType", tableCellImage.ImageType);
                cmd.Parameters.AddWithValue("@Title", (object)tableCellImage.Title ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Style", (object)tableCellImage.Style ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public int GetLastInsertedTableCellImageId()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT TOP 1 TableCellImageId FROM TableCellImages ORDER BY TableCellImageId DESC", conn);
                return (int)cmd.ExecuteScalar();
            }
        }

        public List<TableCellImages> GetImagesByTableCellId(int tableCellId)
        {
            List<TableCellImages> images = new List<TableCellImages>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM TableCellImages WHERE TableCellId = @TableCellId ORDER BY OrderInTableCell";
                    command.Parameters.AddWithValue("@TableCellId", tableCellId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TableCellImages image = new TableCellImages
                            {
                                TableCellImageId = Convert.ToInt32(reader["TableCellImageId"]),
                                TableCellId = Convert.ToInt32(reader["TableCellId"]),
                                OrderInTableCell = Convert.ToInt32(reader["OrderInTableCell"]),
                                ImagePath = reader["ImagePath"].ToString(),
                                ImageType = reader["ImageType"].ToString(),
                                Title = reader["Title"] != DBNull.Value ? reader["Title"].ToString() : null,
                                Style = reader["Style"] != DBNull.Value ? reader["Style"].ToString() : null
                            };
                            images.Add(image);
                        }
                    }
                }
            }
            return images;
        }
    }
}
