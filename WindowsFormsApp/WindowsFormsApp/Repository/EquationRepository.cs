using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using WindowsFormsApp.Model;

namespace WindowsFormsApp.Repository
{
    public class EquationRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";

        // New Equation
        public void SaveEquation(Equation equation) // ok
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO Equation (ParagraphId, OrderInParagraph, EquationContent, Style)
                              VALUES (@ParagraphId, @OrderInParagraph, @EquationContent, @Style)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ParagraphId", equation.ParagraphId);
                command.Parameters.AddWithValue("@OrderInParagraph", equation.OrderInParagraph);
                command.Parameters.AddWithValue("@EquationContent", equation.EquationContent);
                command.Parameters.AddWithValue("@Style", equation.Style ?? (object)DBNull.Value);

                command.ExecuteNonQuery();
            }
        }

        public List<Equation> GetEquationsByParagraphId(int paragraphId)
        {
            List<Equation> equations = new List<Equation>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"SELECT EquationId, ParagraphId, OrderInParagraph, EquationContent, Style
                      FROM Equation
                      WHERE ParagraphId = @ParagraphId
                      ORDER BY OrderInParagraph"; // Sắp xếp theo thứ tự trong đoạn văn
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ParagraphId", paragraphId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var equation = new Equation
                        {
                            EquationId = reader.GetInt32(reader.GetOrdinal("EquationId")),
                            ParagraphId = reader.GetInt32(reader.GetOrdinal("ParagraphId")),
                            OrderInParagraph = reader.GetInt32(reader.GetOrdinal("OrderInParagraph")),
                            EquationContent = reader.IsDBNull(reader.GetOrdinal("EquationContent")) ? null : reader.GetString(reader.GetOrdinal("EquationContent")),
                            Style = reader.IsDBNull(reader.GetOrdinal("Style")) ? null : reader.GetString(reader.GetOrdinal("Style"))
                        };
                        equations.Add(equation);
                    }
                }
            }

            return equations;
        }

    }
}
