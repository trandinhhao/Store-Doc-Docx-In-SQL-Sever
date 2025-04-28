using System.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using WindowsFormsApp.Model;

namespace WindowsFormsApp.Repository
{
    class HeadRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";

        // Lưu Heading vào cơ sở dữ liệu
        public void SaveHead(Head head) // ok
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO Head (DocumentId, HeadHTML)
                              VALUES (@DocumentId, @HeadHTML)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DocumentId", head.DocumentId);
                command.Parameters.AddWithValue("@HeadHTML", head.HeadHTML);

                command.ExecuteNonQuery();
            }
        }

        // Lấy Head theo DocumentId
        public Head GetHeadByDocumentId(int documentId) //ok
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"SELECT HeadId, DocumentId, HeadHTML FROM Head WHERE DocumentId = @DocumentId";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DocumentId", documentId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Head
                        {
                            HeadId = reader.GetInt32(0),
                            DocumentId = reader.GetInt32(1),
                            HeadHTML = reader.IsDBNull(2) ? null : reader.GetString(2)
                        };
                    }
                }
            }
            return null;
        }
    }
}
