using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp.Model;
using System.Data.SqlClient;

namespace WindowsFormsApp.Repository
{
    public class SectionRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";

        // Lưu Section vào cơ sở dữ liệu
        public void SaveSection(Section section)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO Section (DocumentId, Title, OrderInDocument)
                              VALUES (@DocumentId, @Title, @OrderInDocument)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DocumentId", section.DocumentId);
                command.Parameters.AddWithValue("@Title", section.Title);
                command.Parameters.AddWithValue("@OrderInDocument", section.OrderInDocument);

                command.ExecuteNonQuery();
            }
        }

        // Thêm hàm lấy danh sách Section theo DocumentId
        public List<Section> GetSectionsByDocumentId(int documentId)
        {
            List<Section> sections = new List<Section>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"SELECT SectionId, DocumentId, Title, OrderInDocument
                              FROM Section
                              WHERE DocumentId = @DocumentId
                              ORDER BY OrderInDocument ASC"; // lấy theo thứ tự

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DocumentId", documentId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var section = new Section
                        {
                            SectionId = reader.GetInt32(0),
                            DocumentId = reader.GetInt32(1),
                            Title = reader.IsDBNull(2) ? null : reader.GetString(2),
                            OrderInDocument = reader.GetInt32(3)
                        };
                        sections.Add(section);
                    }
                }
            }

            return sections;
        }

        // lấy cái vừa thêm mới
        public int GetLastInsertedSectionId() // ok
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TOP 1 SectionId FROM Section ORDER BY SectionId DESC";
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
    }
}

