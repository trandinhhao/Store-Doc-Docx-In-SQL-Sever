using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WindowsFormsApp.Model;

namespace WindowsFormsApp.Repository
{
    public class ParagraphImageRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";


        public void SaveParaImage(ParagraphImages paragraphImage)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO ParagraphImages (ParagraphId, OrderInParagraph, ImagePath, ImageType, Title, Style) 
                              VALUES (@ParagraphId, @OrderInParagraph, @ImagePath, @ImageType, @Title, @Style)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ParagraphId", paragraphImage.ParagraphId);
                command.Parameters.AddWithValue("@OrderInParagraph", paragraphImage.OrderInParagraph);
                command.Parameters.AddWithValue("@ImagePath", paragraphImage.ImagePath);
                command.Parameters.AddWithValue("@ImageType", paragraphImage.ImageType);
                command.Parameters.AddWithValue("@Title", paragraphImage.Title ?? (object)DBNull.Value); // Xử lý null
                command.Parameters.AddWithValue("@Style", paragraphImage.Style ?? (object)DBNull.Value); // Xử lý null
                command.ExecuteNonQuery();
            }
        }

        // Lấy bản ghi ParagraphImages được chèn cuối cùng
        public ParagraphImages GetLastInsertedParaImage()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"SELECT TOP 1 * FROM ParagraphImages ORDER BY ParagraphImageId DESC";
                var command = new SqlCommand(query, connection);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ParagraphImages
                        {
                            ParagraphImageId = reader.GetInt32(reader.GetOrdinal("ParagraphImageId")),
                            ParagraphId = reader.GetInt32(reader.GetOrdinal("ParagraphId")),
                            OrderInParagraph = reader.GetInt32(reader.GetOrdinal("OrderInParagraph")),
                            ImagePath = reader.GetString(reader.GetOrdinal("ImagePath")),
                            ImageType = reader.GetString(reader.GetOrdinal("ImageType")),
                            Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                            Style = reader.IsDBNull(reader.GetOrdinal("Style")) ? null : reader.GetString(reader.GetOrdinal("Style"))
                        };
                    }
                    return null; // Không tìm thấy bản ghi nào
                }
            }
        }

        // Cập nhật thông tin ParagraphImages
        public void UpdateParaImage(ParagraphImages paragraphImage)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"UPDATE ParagraphImages 
                              SET Title = @Title, 
                              WHERE ParagraphImageId = @ParagraphImageId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ParagraphImageId", paragraphImage.ParagraphImageId);
                command.Parameters.AddWithValue("@Title", paragraphImage.Title ?? (object)DBNull.Value);
                command.ExecuteNonQuery();
            }
        }

        public List<ParagraphImages> GetImagesByParagraphId(int paragraphId)
        {
            List<ParagraphImages> images = new List<ParagraphImages>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT ParagraphImageId, ParagraphId, OrderInParagraph, ImagePath, ImageType, Title, Style FROM ParagraphImages 
                                WHERE ParagraphId = @ParagraphId ORDER BY OrderInParagraph";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ParagraphId", paragraphId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            images.Add(new ParagraphImages
                            {
                                ParagraphImageId = reader.GetInt32(0),
                                ParagraphId = reader.GetInt32(1),
                                OrderInParagraph = reader.GetInt32(2),
                                ImagePath = reader.GetString(3),
                                ImageType = reader.GetString(4),
                                Title = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Style = reader.IsDBNull(6) ? null : reader.GetString(6)
                            });
                        }
                    }
                }
            }
            return images;
        }
    }
}