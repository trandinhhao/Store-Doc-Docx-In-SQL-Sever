using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using WindowsFormsApp.Model;

namespace WindowsFormsApp.Repository
{
    public class TableRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";


        public void SaveTable(Tables table)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO Tables (ParagraphId, Title, OrderInParagraph, NumRow, NumColumn)
                              VALUES (@ParagraphId, @Title, @OrderInParagraph, @NumRow, @NumColumn)";
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ParagraphId", table.ParagraphId);
                cmd.Parameters.AddWithValue("@Title", table.Title);
                cmd.Parameters.AddWithValue("@OrderInParagraph", table.OrderInParagraph);
                cmd.Parameters.AddWithValue("@NumRow", table.NumRow);
                cmd.Parameters.AddWithValue("@NumColumn", table.NumColumn);
                cmd.ExecuteNonQuery();
            }
        }

        public int GetLastInsertedTableId()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT TOP 1 TableId FROM Tables ORDER BY TableId DESC", conn);
                return (int)cmd.ExecuteScalar();
            }
        }

        public void UpdateTable(Tables table)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"Update Tables Set NumRow = @NumRow, NumColumn = @NumColumn Where TableId = @TableId";
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@NumRow", table.NumRow);
                cmd.Parameters.AddWithValue("@NumColumn", table.NumColumn);
                cmd.Parameters.AddWithValue("@TableId", table.TableId);
                cmd.ExecuteNonQuery();
            }
        }

        public List<Tables> GetTablesBySection(int sectionId)
        {
            List<Tables> tables = new List<Tables>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT t.* FROM Tables t
                                          INNER JOIN Paragraphs p ON t.ParagraphId = p.ParagraphId
                                          WHERE p.SectionId = @SectionId";
                    command.Parameters.AddWithValue("@SectionId", sectionId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Tables table = new Tables
                            {
                                TableId = Convert.ToInt32(reader["TableId"]),
                                ParagraphId = Convert.ToInt32(reader["ParagraphId"]),
                                Title = reader["Title"].ToString(),
                                OrderInParagraph = Convert.ToInt32(reader["OrderInParagraph"]),
                                NumRow = Convert.ToInt32(reader["NumRow"]),
                                NumColumn = Convert.ToInt32(reader["NumColumn"])
                            };
                            tables.Add(table);
                        }
                    }
                }
            }
            return tables;
        }

        public List<Tables> GetTablesByParagraph(int paragraphId)
        {
            List<Tables> tables = new List<Tables>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM Tables WHERE ParagraphId = @ParagraphId";
                    command.Parameters.AddWithValue("@ParagraphId", paragraphId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Tables table = new Tables
                            {
                                TableId = Convert.ToInt32(reader["TableId"]),
                                ParagraphId = Convert.ToInt32(reader["ParagraphId"]),
                                Title = reader["Title"].ToString(),
                                OrderInParagraph = Convert.ToInt32(reader["OrderInParagraph"]),
                                NumRow = Convert.ToInt32(reader["NumRow"]),
                                NumColumn = Convert.ToInt32(reader["NumColumn"])
                            };
                            tables.Add(table);
                        }
                    }
                }
            }
            return tables;
        }
    }
}
