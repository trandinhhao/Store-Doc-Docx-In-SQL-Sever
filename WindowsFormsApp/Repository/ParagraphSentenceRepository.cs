using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using WindowsFormsApp.Model;

namespace WindowsFormsApp.Repository
{
    public class ParagraphSentenceRepository
    {
       private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";

        public void SaveParagraphSentence(ParagraphSentences sentence)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO ParagraphSentences (ParagraphId, OrderInParagraph, TextContent) 
                              VALUES (@ParagraphId, @OrderInParagraph, @TextContent)";
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ParagraphId", sentence.ParagraphId);
                cmd.Parameters.AddWithValue("@OrderInParagraph", sentence.OrderInParagraph);
                cmd.Parameters.AddWithValue("@TextContent", sentence.TextContent);
                cmd.ExecuteNonQuery();
            }
        }

        public List<ParagraphSentences> GetSentencesByParagraphId(int paragraphId)
        {
            List<ParagraphSentences> sentences = new List<ParagraphSentences>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT ParagraphSentenceId, ParagraphId, OrderInParagraph, TextContent FROM ParagraphSentences 
                                WHERE ParagraphId = @ParagraphId ORDER BY OrderInParagraph";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ParagraphId", paragraphId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sentences.Add(new ParagraphSentences
                            {
                                ParagraphSentenceId = reader.GetInt32(0),
                                ParagraphId = reader.GetInt32(1),
                                OrderInParagraph = reader.GetInt32(2),
                                TextContent = reader.GetString(3)
                            });
                        }
                    }
                }
            }
            return sentences;
        }

        public void UpdateTextContent(int paragraphSentenceId, string newTextContent)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"UPDATE ParagraphSentences 
                         SET TextContent = @TextContent 
                         WHERE ParagraphSentenceId = @ParagraphSentenceId";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@TextContent", newTextContent);
                    cmd.Parameters.AddWithValue("@ParagraphSentenceId", paragraphSentenceId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Lấy ID của câu vừa được thêm
        public int GetLastInsertedParagraphSentenceId()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TOP 1 ParagraphSentenceId FROM ParagraphSentences ORDER BY ParagraphSentenceId DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                    return -1; // hoặc trả về 0 nếu bạn muốn báo là không có bản ghi
                }
            }
        }

        public ParagraphSentences GetSentenceByParagraphAndOrder(int paragraphId, int orderInParagraph)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT ParagraphSentenceId, ParagraphId, OrderInParagraph, TextContent 
                                FROM ParagraphSentences 
                                WHERE ParagraphId = @ParagraphId AND OrderInParagraph = @OrderInParagraph";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ParagraphId", paragraphId);
                    cmd.Parameters.AddWithValue("@OrderInParagraph", orderInParagraph);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ParagraphSentences
                            {
                                ParagraphSentenceId = reader.GetInt32(0),
                                ParagraphId = reader.GetInt32(1),
                                OrderInParagraph = reader.GetInt32(2),
                                TextContent = reader.GetString(3)
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
