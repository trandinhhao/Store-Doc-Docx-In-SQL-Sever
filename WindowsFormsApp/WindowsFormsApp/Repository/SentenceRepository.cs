using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using WindowsFormsApp.Model;

namespace WindowsFormsApp.Repository
{
    public class SentenceRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";

        // Lưu một câu vào cơ sở dữ liệu
        public void SaveSentence(Sentence sentence) // ok
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO Sentence (ParagraphId, OrderInParagraph, TextContent, Style) VALUES (@ParagraphId, @OrderInParagraph, @TextContent, @Style)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ParagraphId", sentence.ParagraphId);
                command.Parameters.AddWithValue("@OrderInParagraph", sentence.OrderInParagraph);
                command.Parameters.AddWithValue("@TextContent", sentence.TextContent ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Style", sentence.Style ?? (object)DBNull.Value);

                command.ExecuteNonQuery();
            }
        }

        public List<Sentence> GetSentencesByParagraphId(int paragraphId)
        {
            List<Sentence> sentences = new List<Sentence>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"SELECT SentenceId, ParagraphId, OrderInParagraph, TextContent, Style 
                      FROM Sentence 
                      WHERE ParagraphId = @ParagraphId
                      ORDER BY OrderInParagraph";  // Sắp xếp theo thứ tự trong đoạn văn
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ParagraphId", paragraphId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var sentence = new Sentence
                        {
                            SentenceId = reader.GetInt32(reader.GetOrdinal("SentenceId")),
                            ParagraphId = reader.GetInt32(reader.GetOrdinal("ParagraphId")),
                            OrderInParagraph = reader.GetInt32(reader.GetOrdinal("OrderInParagraph")),
                            TextContent = reader.GetString(reader.GetOrdinal("TextContent")),
                            Style = reader.IsDBNull(reader.GetOrdinal("Style")) ? null : reader.GetString(reader.GetOrdinal("Style"))
                        };
                        sentences.Add(sentence);
                    }
                }
            }

            return sentences;
        }

    }
}
