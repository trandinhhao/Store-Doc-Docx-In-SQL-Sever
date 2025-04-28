using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp.Model;
using System.Data.SqlClient;

namespace WindowsFormsApp.Repository
{
    public class ParagraphRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";

        // New Paragraph
        public void SaveParagraph(Paragraph paragraph) // ok
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO Paragraph (SectionId, OrderInSection, ParagraphHTML) VALUES (@SectionId, @OrderInSection, @ParagraphHTML)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SectionId", paragraph.SectionId);
                command.Parameters.AddWithValue("@OrderInSection", paragraph.OrderInSection);
                command.Parameters.AddWithValue("@ParagraphHTML", (object)paragraph.ParagraphHTML ?? DBNull.Value);

                command.ExecuteNonQuery();
            }
        }

        public int GetLastInsertedParagraphId() // ok
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TOP 1 ParagraphId FROM Paragraph ORDER BY ParagraphId DESC";
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

        public List<Paragraph> GetParagraphsBySection(int sectionId) // ok
        {
            var paragraphs = new List<Paragraph>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"SELECT ParagraphId, SectionId, OrderInSection, ParagraphHTML
                      FROM Paragraph
                      WHERE SectionId = @SectionId
                      ORDER BY OrderInSection ASC"; // lấy đúng thứ tự xuất hiện

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SectionId", sectionId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            paragraphs.Add(new Paragraph
                            {
                                ParagraphId = reader.GetInt32(0),
                                SectionId = reader.GetInt32(1),
                                OrderInSection = reader.GetInt32(2),
                                ParagraphHTML = reader.IsDBNull(3) ? null : reader.GetString(3)
                            });
                        }
                    }
                }
            }

            return paragraphs;
        }

        
    }
}
