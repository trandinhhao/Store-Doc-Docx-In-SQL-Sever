using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WindowsFormsApp.Model;

namespace WindowsFormsApp.Repository
{
    public class DocumentRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";
        
        // New Document
        public void SaveDocument(Document document) // ok
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO Document (Title, OriginalFormat, Author, FileSize, UploadDate, PublishDate, FilePath) 
                              VALUES (@Title, @OriginalFormat, @Author, @FileSize, @UploadDate, @PublishDate, @FilePath)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Title", document.Title);
                command.Parameters.AddWithValue("@OriginalFormat", document.OriginalFormat);
                command.Parameters.AddWithValue("@Author", document.Author);
                command.Parameters.AddWithValue("@FileSize", document.FileSize);
                command.Parameters.AddWithValue("@UploadDate", document.UploadDate);
                command.Parameters.AddWithValue("@PublishDate", document.PublishDate);
                command.Parameters.AddWithValue("@FilePath", document.FilePath);
                // save
                command.ExecuteNonQuery();
            }
        }

        // Get All
        public List<Document> GetAllDocuments() // ok
        {
            var documents = new List<Document>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Document";
                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var document = new Document
                    {
                        DocumentId = (int)reader["DocumentId"],
                        Title = reader["Title"].ToString(),
                        OriginalFormat = reader["OriginalFormat"].ToString(),
                        Author = reader["Author"].ToString(),
                        FileSize = reader["FileSize"] as int?,
                        UploadDate = (DateTime)reader["UploadDate"],
                        PublishDate = reader["PublishDate"] as DateTime?,
                        FilePath = reader["FilePath"].ToString()
                    };
                    documents.Add(document);
                }
            }
            return documents;
        }

        public int GetLastInsertedDocumentId()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TOP 1 DocumentId FROM Document ORDER BY DocumentId DESC";
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

        // lay docu de tai tao
        public Document GetDocumentById(int documentId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Document WHERE DocumentId = @DocumentId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DocumentId", documentId);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Document
                    {
                        DocumentId = (int)reader["DocumentId"],
                        Title = reader["Title"].ToString(),
                        Author = reader["Author"].ToString(),
                        OriginalFormat = reader["OriginalFormat"].ToString(),
                        FileSize = reader["FileSize"] as long?,
                        UploadDate = (DateTime)reader["UploadDate"],
                        FilePath = reader["FilePath"].ToString(),
                        PublishDate = reader["PublishDate"] as DateTime?,
                    };
                }
                return null;
            }
        }

        public int GetDocumentIdByFilePath(string filePath)
        {
            int documentId = 0;
            using (var connection = new SqlConnection(connectionString))
            {
                // Thay đổi query để lấy bản ghi mới nhất dựa trên DocumentId
                string query = "SELECT TOP 1 DocumentId FROM Document WHERE FilePath = @FilePath ORDER BY DocumentId DESC";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FilePath", filePath);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        documentId = (int)result;
                    }
                }
            }
            return documentId;
        }

        public void DeleteDocument(int documentId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;

                    // Xóa theo thứ tự phụ thuộc (con trước cha)
                    cmd.CommandText = @"
                -- Xóa Sentence
                DELETE FROM Sentence WHERE ParagraphId IN (
                    SELECT ParagraphId FROM Paragraph WHERE SectionId IN (
                        SELECT SectionId FROM Section WHERE DocumentId = @DocumentId
                    )
                );

                -- Xóa Image
                DELETE FROM Image WHERE ParagraphId IN (
                    SELECT ParagraphId FROM Paragraph WHERE SectionId IN (
                        SELECT SectionId FROM Section WHERE DocumentId = @DocumentId
                    )
                );

                -- Xóa Equation
                DELETE FROM Equation WHERE ParagraphId IN (
                    SELECT ParagraphId FROM Paragraph WHERE SectionId IN (
                        SELECT SectionId FROM Section WHERE DocumentId = @DocumentId
                    )
                );

                -- Xóa Paragraph
                DELETE FROM Paragraph WHERE SectionId IN (
                    SELECT SectionId FROM Section WHERE DocumentId = @DocumentId
                );

                -- Xóa TableElement
                DELETE FROM TableElement WHERE SectionId IN (
                    SELECT SectionId FROM Section WHERE DocumentId = @DocumentId
                );

                -- Xóa Heading
                DELETE FROM Heading WHERE SectionId IN (
                    SELECT SectionId FROM Section WHERE DocumentId = @DocumentId
                );

                -- Xóa Section
                DELETE FROM Section WHERE DocumentId = @DocumentId;

                -- Xóa Head
                DELETE FROM Head WHERE DocumentId = @DocumentId;

                -- Cuối cùng xóa Document
                DELETE FROM Document WHERE DocumentId = @DocumentId;
            ";

                    cmd.Parameters.AddWithValue("@DocumentId", documentId);
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}

