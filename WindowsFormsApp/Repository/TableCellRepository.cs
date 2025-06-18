using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using WindowsFormsApp.Model;

namespace WindowsFormsApp.Repository
{
    public class TableCellRepository
    {
        private readonly string connectionString = "Data Source=LAPTOP-P88P5NCF\\CSDLPTNHOM1;Initial Catalog=DocStorageDB;Integrated Security=True";


        public void SaveTableCell(TableCells cell)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var query = @"INSERT INTO TableCells (TableId, RowInTable, ColumnInTable)
                              VALUES (@TableId, @RowInTable, @ColumnInTable)";
                var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TableId", cell.TableId);
                cmd.Parameters.AddWithValue("@RowInTable", cell.RowInTable);
                cmd.Parameters.AddWithValue("@ColumnInTable", cell.ColumnInTable);
                cmd.ExecuteNonQuery();
            }
        }

        public int GetLastInsertedTableCellId()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT TOP 1 TableCellId FROM TableCells ORDER BY TableCellId DESC", conn);
                return (int)cmd.ExecuteScalar();
            }
        }

        public List<TableCells> GetCellsByTableId(int tableId)
        {
            List<TableCells> cells = new List<TableCells>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT TableCellId, TableId, RowInTable, ColumnInTable FROM TableCells 
                                WHERE TableId = @TableId ORDER BY RowInTable, ColumnInTable";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TableId", tableId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cells.Add(new TableCells
                            {
                                TableCellId = reader.GetInt32(0),
                                TableId = reader.GetInt32(1),
                                RowInTable = reader.GetInt32(2),
                                ColumnInTable = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }
            return cells;
        }
    }
}
