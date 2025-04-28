using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using WindowsFormsApp.Model;

namespace WindowsFormsApp.Repository
{
    class HeadingRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";

        // New Heading
        public void SaveHeading(Heading heading)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO Heading (SectionId, OrderInSection, ParentHeadingId, Level, Title, Style)
                              VALUES (@SectionId, @OrderInSection, @ParentHeadingId, @Level, @Title, @Style)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SectionId", heading.SectionId);
                command.Parameters.AddWithValue("@OrderInSection", heading.OrderInSection);
                command.Parameters.AddWithValue("@ParentHeadingId", heading.ParentHeadingId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Level", heading.Level);
                command.Parameters.AddWithValue("@Title", heading.Title);
                command.Parameters.AddWithValue("@Style", (object)(heading.Style ?? (object)DBNull.Value));

                command.ExecuteNonQuery();
            }
        }

        public List<Heading> GetHeadingsBySection(int sectionId) // ok
        {
            List<Heading> headings = new List<Heading>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT HeadingId, SectionId, OrderInSection, ParentHeadingId, Level, Title, Style FROM Heading WHERE SectionId = @SectionId ORDER BY OrderInSection";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SectionId", sectionId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Heading heading = new Heading
                            {
                                HeadingId = reader.GetInt32(0),
                                SectionId = reader.GetInt32(1),
                                OrderInSection = reader.GetInt32(2),
                                ParentHeadingId = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                Level = reader.GetInt32(4),
                                Title = reader.GetString(5),
                                Style = reader.IsDBNull(6) ? null : reader.GetString(6)
                            };
                            headings.Add(heading);
                        }
                    }
                }
            }
            return headings;
        }
    }
}
