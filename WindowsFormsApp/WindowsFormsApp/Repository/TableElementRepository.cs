using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using WindowsFormsApp.Model;

namespace WindowsFormsApp.Repository
{
    public class TableElementRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";

        // New TableElement
        public void SaveTable(TableElement tableElement) // ok
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("INSERT INTO TableElement (SectionId, OrderInSection, TableHTML) VALUES (@SectionId, @OrderInSection, @TableHTML)", connection);
                command.Parameters.AddWithValue("@SectionId", tableElement.SectionId);
                command.Parameters.AddWithValue("@OrderInSection", tableElement.OrderInSection);
                command.Parameters.AddWithValue("@TableHTML", (object)tableElement.TableHTML ?? DBNull.Value);

                command.ExecuteNonQuery();
            }
        }

        public List<TableElement> GetTablesBySection(int sectionId) //ok
        {
            var tables = new List<TableElement>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT TableId, SectionId, OrderInSection, TableHTML FROM TableElement WHERE SectionId = @SectionId ORDER BY OrderInSection ASC";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SectionId", sectionId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var table = new TableElement
                        {
                            TableId = reader.GetInt32(0),
                            SectionId = reader.GetInt32(1),
                            OrderInSection = reader.GetInt32(2),
                            TableHTML = reader.IsDBNull(3) ? null : reader.GetString(3)
                        };
                        tables.Add(table);
                    }
                }
            }

            return tables;
        }

    }
}
