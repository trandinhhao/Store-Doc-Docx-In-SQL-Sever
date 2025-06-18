using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WindowsFormsApp.Model;

namespace WindowsFormsApp.Repository
{
    public class SectionRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";


        public void SaveSection(Sections section)
        {
            using (var connection = new SqlConnection(connectionString)) {
                connection.Open();
                var query = "INSERT INTO Sections (DocumentId, ParentSectionId, [Level], Title, OrderInDocument) VALUES (@DocumentId, @ParentSectionId, @Level, @Title, @OrderInDocument)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DocumentId", section.DocumentId);
                command.Parameters.AddWithValue("@ParentSectionId", (object)section.ParentSectionId ?? DBNull.Value);
                command.Parameters.AddWithValue("@Level", section.Level);
                command.Parameters.AddWithValue("@Title", section.Title);
                command.Parameters.AddWithValue("@OrderInDocument", section.OrderInDocument);
                command.ExecuteNonQuery();
            }
        }

        public List<Sections> GetSectionsByDocumentId(int documentId)
        {
            List<Sections> sections = new List<Sections>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT SectionId, DocumentId, ParentSectionId, [Level], Title, OrderInDocument FROM Sections 
                                WHERE DocumentId = @DocumentId ORDER BY OrderInDocument";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DocumentId", documentId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sections.Add(new Sections
                            {
                                SectionId = reader.GetInt32(0),
                                DocumentId = reader.GetInt32(1),
                                ParentSectionId = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                                Level = reader.GetInt32(3),
                                Title = reader.GetString(4),
                                OrderInDocument = reader.GetInt32(5)
                            });
                        }
                    }
                }
            }
            return sections;
        }

        // Lấy ID của section vừa được thêm
        public int GetLastInsertedSectionId()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TOP 1 SectionId FROM Sections ORDER BY SectionId DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                    return 1; 
                }
            }
        }
    }
}
