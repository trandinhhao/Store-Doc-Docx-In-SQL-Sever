using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using WindowsFormsApp.Model;
namespace WindowsFormsApp.Repository
{
    public class TableCellSentenceRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";


        public void SaveTableCellSentence(TableCellSentences sentence)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO TableCellSentences (TableCellId, OrderInTableCell, TextContent)
                              VALUES (@TableCellId, @OrderInTableCell, @TextContent)";
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@TableCellId", sentence.TableCellId);
                cmd.Parameters.AddWithValue("@OrderInTableCell", sentence.OrderInTableCell);
                cmd.Parameters.AddWithValue("@TextContent", sentence.TextContent);
                cmd.ExecuteNonQuery();
            }
        }

        public List<TableCellSentences> GetSentencesByTableCellId(int tableCellId)
        {
            List<TableCellSentences> sentences = new List<TableCellSentences>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT TableCellSentenceId, TableCellId, OrderInTableCell, TextContent FROM TableCellSentences 
                                WHERE TableCellId = @TableCellId ORDER BY OrderInTableCell";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TableCellId", tableCellId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sentences.Add(new TableCellSentences
                            {
                                TableCellSentenceId = reader.GetInt32(0),
                                TableCellId = reader.GetInt32(1),
                                OrderInTableCell = reader.GetInt32(2),
                                TextContent = reader.GetString(3)
                            });
                        }
                    }
                }
            }
            return sentences;
        }

        public int GetLastInsertedTableCellSentenceId()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TOP 1 TableCellSentenceId FROM TableCellSentences ORDER BY TableCellSentenceId DESC";
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
    }
}
