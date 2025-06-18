using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp.Model
{
    public class TableCellSentences
    {
        public int TableCellSentenceId { get; set; }
        public int TableCellId { get; set; }
        public int OrderInTableCell { get; set; }
        public string TextContent { get; set; }
    }
}
