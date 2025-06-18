using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WindowsFormsApp.Model;

namespace WindowsFormsApp.Repository
{
    public class TableCellEquationRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";


        public void SaveTableCellEquation(TableCellEquations tableCellEquation)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO TableCellEquations (TableCellId, TableCellSentenceId, OrderInTableCell, OrderInSentence, EquationContent, Style)
                              VALUES (@TableCellId, @TableCellSentenceId, @OrderInTableCell, @OrderInSentence, @EquationContent, @Style)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TableCellId", tableCellEquation.TableCellId);
                command.Parameters.AddWithValue("@TableCellSentenceId", tableCellEquation.TableCellSentenceId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@OrderInTableCell", tableCellEquation.OrderInTableCell);
                command.Parameters.AddWithValue("@OrderInSentence", tableCellEquation.OrderInSentence ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@EquationContent", tableCellEquation.EquationContent);
                command.Parameters.AddWithValue("@Style", (object)tableCellEquation.Style ?? DBNull.Value);
                command.ExecuteNonQuery();
            }
        }

        public List<TableCellEquations> GetEquationsByTableCellId(int tableCellId)
        {
            List<TableCellEquations> equations = new List<TableCellEquations>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT TableCellEquationId, TableCellId, TableCellSentenceId, OrderInTableCell, OrderInSentence, EquationContent, Style FROM TableCellEquations 
                                WHERE TableCellId = @TableCellId ORDER BY OrderInTableCell";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TableCellId", tableCellId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            equations.Add(new TableCellEquations
                            {
                                TableCellEquationId = reader.GetInt32(0),
                                TableCellId = reader.GetInt32(1),
                                TableCellSentenceId = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                                OrderInTableCell = reader.GetInt32(3),
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
