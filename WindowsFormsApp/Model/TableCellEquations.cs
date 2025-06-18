using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp.Model
{
    public class TableCellEquations
    {
        public int TableCellEquationId { get; set; }
        public int? TableCellId { get; set; }
        public int? TableCellSentenceId { get; set; }
        public int? OrderInTableCell { get; set; }
        public int? OrderInSentence { get; set; }
        public string EquationContent { get; set; }
        public string Style { get; set; }
    }
}
