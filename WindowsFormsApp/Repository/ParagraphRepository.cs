using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WindowsFormsApp.Model;

namespace WindowsFormsApp.Repository
{
    public class ParagraphRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";


        public void SavePragraph(Paragraphs paragraph)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "INSERT INTO Paragraphs (SectionId, OrderInSection) VALUES (@SectionId, @OrderInSection)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SectionId", paragraph.SectionId);
                command.Parameters.AddWithValue("@OrderInSection", paragraph.OrderInSection);
                command.ExecuteNonQuery();
            }
        }

        public int GetLastInsertedParagraphId()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TOP 1 ParagraphId FROM Paragraphs ORDER BY ParagraphId DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                    return 1; // Trả về giá trị mặc định nếu không có bản ghi nào
                }
            }
        }

        public List<Paragraphs> GetParagraphsBySection(int sectionId)
        {
            List<Paragraphs> paragraphs = new List<Paragraphs>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT ParagraphId, SectionId, OrderInSection FROM Paragraphs 
                                WHERE SectionId = @SectionId ORDER BY OrderInSection";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SectionId", sectionId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            paragraphs.Add(new Paragraphs
                            {
                                ParagraphId = reader.GetInt32(0),
                                SectionId = reader.GetInt32(1),
                                OrderInSection = reader.GetInt32(2)
                            });
                        }
                    }
                }
            }
            return paragraphs;
        }
    }
}
