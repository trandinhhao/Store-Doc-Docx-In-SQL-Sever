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
        public void SaveDocument(Documents document) // ok
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO Documents (Title, OriginalFormat, Author, FileSize, UploadDate, PublishDate, FilePath) 
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
        public List<Documents> GetAllDocuments() // ok
        {
            var documents = new List<Documents>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Documents ORDER BY DocumentId";
                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var document = new Documents
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
                string query = "SELECT TOP 1 DocumentId FROM Documents ORDER BY DocumentId DESC";
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
        public Documents GetDocumentById(int documentId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"SELECT * FROM Documents 
                            WHERE DocumentId = @DocumentId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DocumentId", documentId);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Documents
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
                string query = @"SELECT TOP 1 DocumentId FROM Documents 
                                WHERE FilePath = @FilePath 
                                ORDER BY DocumentId DESC";
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
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Xóa TableCellImages
                        using (var command = new SqlCommand(
                            @"DELETE FROM TableCellImages 
                                WHERE TableCellId IN (SELECT TableCellId FROM TableCells 
                                WHERE TableId IN (SELECT TableId FROM Tables 
                                WHERE ParagraphId IN (SELECT ParagraphId FROM Paragraphs 
                                WHERE SectionId IN (SELECT SectionId FROM Sections 
                                WHERE DocumentId = @DocumentId))))",
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@DocumentId", documentId);
                            command.ExecuteNonQuery();
                        }

                        // 2. Xóa TableCellEquations
                        using (var command = new SqlCommand(
                            @"DELETE FROM TableCellEquations 
                                WHERE TableCellId IN (SELECT TableCellId FROM TableCells 
                                WHERE TableId IN (SELECT TableId FROM Tables 
                                WHERE ParagraphId IN (SELECT ParagraphId FROM Paragraphs 
                                WHERE SectionId IN (SELECT SectionId FROM Sections 
                                WHERE DocumentId = @DocumentId))))",
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@DocumentId", documentId);
                            command.ExecuteNonQuery();
                        }

                        // 3. Xóa TableCellSentences
                        using (var command = new SqlCommand(
                            @"DELETE FROM TableCellSentences 
                            WHERE TableCellId IN (SELECT TableCellId FROM TableCells 
                            WHERE TableId IN (SELECT TableId FROM Tables 
                            WHERE ParagraphId IN (SELECT ParagraphId FROM Paragraphs 
                            WHERE SectionId IN (SELECT SectionId FROM Sections 
                            WHERE DocumentId = @DocumentId))))",
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@DocumentId", documentId);
                            command.ExecuteNonQuery();
                        }

                        // 4. Xóa TableCells
                        using (var command = new SqlCommand(
                            @"DELETE FROM TableCells 
                            WHERE TableId IN (SELECT TableId FROM Tables 
                            WHERE ParagraphId IN (SELECT ParagraphId FROM Paragraphs 
                            WHERE SectionId IN (SELECT SectionId FROM Sections 
                            WHERE DocumentId = @DocumentId)))",
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@DocumentId", documentId);
                            command.ExecuteNonQuery();
                        }

                        // 5. Xóa Tables
                        using (var command = new SqlCommand(
                            @"DELETE FROM Tables 
                            WHERE ParagraphId IN (SELECT ParagraphId FROM Paragraphs 
                            WHERE SectionId IN (SELECT SectionId FROM Sections 
                            WHERE DocumentId = @DocumentId))",
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@DocumentId", documentId);
                            command.ExecuteNonQuery();
                        }

                        // 6. Xóa ParagraphImages
                        using (var command = new SqlCommand(
                            @"DELETE FROM ParagraphImages 
                            WHERE ParagraphId IN (SELECT ParagraphId FROM Paragraphs 
                            WHERE SectionId IN (SELECT SectionId FROM Sections 
                            WHERE DocumentId = @DocumentId))",
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@DocumentId", documentId);
                            command.ExecuteNonQuery();
                        }

                        // 7. Xóa ParagraphEquations
                        using (var command = new SqlCommand(
                            @"DELETE FROM ParagraphEquations 
                            WHERE ParagraphId IN (SELECT ParagraphId FROM Paragraphs 
                            WHERE SectionId IN (SELECT SectionId FROM Sections 
                            WHERE DocumentId = @DocumentId))",
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@DocumentId", documentId);
                            command.ExecuteNonQuery();
                        }

                        // 8. Xóa ParagraphSentences
                        using (var command = new SqlCommand(
                            @"DELETE FROM ParagraphSentences 
                            WHERE ParagraphId IN (SELECT ParagraphId FROM Paragraphs 
                            WHERE SectionId IN (SELECT SectionId FROM Sections 
                            WHERE DocumentId = @DocumentId))",
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@DocumentId", documentId);
                            command.ExecuteNonQuery();
                        }

                        // 9. Xóa Paragraphs
                        using (var command = new SqlCommand(
                            @"DELETE FROM Paragraphs 
                            WHERE SectionId IN (SELECT SectionId FROM Sections 
                            WHERE DocumentId = @DocumentId)",
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@DocumentId", documentId);
                            command.ExecuteNonQuery();
                        }

                        // 10. Xóa Sections
                        using (var command = new SqlCommand(
                            @"DELETE FROM Sections 
                            WHERE DocumentId = @DocumentId",
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@DocumentId", documentId);
                            command.ExecuteNonQuery();
                        }

                        // 11. Xóa Documents
                        using (var command = new SqlCommand(
                            @"DELETE FROM Documents 
                            WHERE DocumentId = @DocumentId",
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@DocumentId", documentId);
                            command.ExecuteNonQuery();
                        }

                        // Commit transaction nếu tất cả thành công
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        // Rollback transaction nếu có lỗi
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

    }
}

