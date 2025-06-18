using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WindowsFormsApp.Model;

namespace WindowsFormsApp.Repository
{
    public class ParagraphEquationRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";


        public void SaveParaEquation(ParagraphEquations paraEquation) // ok
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO ParagraphEquations (ParagraphId, ParagraphSentenceId, OrderInParagraph, OrderInSentence, EquationContent, Style) 
                              VALUES (@ParagraphId, @ParagraphSentenceId, @OrderInParagraph, @OrderInSentence, @EquationContent, @Style)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ParagraphId", paraEquation.ParagraphId);
                command.Parameters.AddWithValue("@ParagraphSentenceId", paraEquation.ParagraphSentenceId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@OrderInParagraph", paraEquation.OrderInParagraph);
                command.Parameters.AddWithValue("@OrderInSentence", paraEquation.OrderInSentence ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@EquationContent", paraEquation.EquationContent);
                command.Parameters.AddWithValue("@Style", paraEquation.Style);
                // save
                command.ExecuteNonQuery();
            }
        }

        public List<ParagraphEquations> GetEquationsByParagraphId(int paragraphId)
        {
            List<ParagraphEquations> equations = new List<ParagraphEquations>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT ParagraphEquationId, ParagraphId, ParagraphSentenceId, OrderInParagraph, OrderInSentence, EquationContent, Style FROM ParagraphEquations 
                                WHERE ParagraphId = @ParagraphId ORDER BY OrderInParagraph";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ParagraphId", paragraphId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            equations.Add(new ParagraphEquations
                            {
                                ParagraphEquationId = reader.GetInt32(0),
                                ParagraphId = reader.GetInt32(1),
                                ParagraphSentenceId = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                                OrderInParagraph = reader.GetInt32(3),
                                OrderInSentence = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                                EquationContent = reader.GetString(5),
                                Style = reader.IsDBNull(6) ? null : reader.GetString(6)
                            });
                        }
                    }
                }
            }
            return equations;
        }
    }
}
