using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp.Model
{
    public class TableCells
    {
        public int TableCellId { get; set; }
        public int TableId { get; set; }
        public int RowInTable { get; set; }
        public int ColumnInTable { get; set; }
    }
}
