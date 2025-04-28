using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using WindowsFormsApp.Model;

namespace WindowsFormsApp.Repository
{
    public class ImageRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";

        // Lưu Image vào cơ sở dữ liệu
        public void SaveImage(Image image)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO Image (ParagraphId, OrderInParagraph, ImageContent, ImageType, Style)
                              VALUES (@ParagraphId, @OrderInParagraph, @ImageContent, @ImageType, @Style)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ParagraphId", image.ParagraphId);
                command.Parameters.AddWithValue("@OrderInParagraph", image.OrderInParagraph);
                command.Parameters.AddWithValue("@ImageContent", (object)image.ImageContent ?? DBNull.Value);
                command.Parameters.AddWithValue("@ImageType", (object)image.ImageType ?? DBNull.Value);
                command.Parameters.AddWithValue("@Style", (object)image.Style ?? DBNull.Value);

                command.ExecuteNonQuery();
            }
        }

        public List<Image> GetImagesByParagraphId(int paragraphId)
        {
            List<Image> images = new List<Image>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"SELECT ImageId, ParagraphId, OrderInParagraph, ImageContent, ImageType, Style
                      FROM Image
                      WHERE ParagraphId = @ParagraphId
                      ORDER BY OrderInParagraph"; // Sắp xếp theo thứ tự trong đoạn văn
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ParagraphId", paragraphId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var image = new Image
                        {
                            ImageId = reader.GetInt32(reader.GetOrdinal("ImageId")),
                            ParagraphId = reader.GetInt32(reader.GetOrdinal("ParagraphId")),
                            OrderInParagraph = reader.GetInt32(reader.GetOrdinal("OrderInParagraph")),
                            ImageContent = reader.IsDBNull(reader.GetOrdinal("ImageContent")) ? null : reader.GetString(reader.GetOrdinal("ImageContent")),
                            ImageType = reader.IsDBNull(reader.GetOrdinal("ImageType")) ? null : reader.GetString(reader.GetOrdinal("ImageType")),
                            Style = reader.IsDBNull(reader.GetOrdinal("Style")) ? null : reader.GetString(reader.GetOrdinal("Style"))
                        };
                        images.Add(image);
                    }
                }
            }

            return images;
        }

    }
}

