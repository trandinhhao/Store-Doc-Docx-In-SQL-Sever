using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp.Model
{
    public class ParagraphEquations
    {
        public int ParagraphEquationId { get; set; }
        public int? ParagraphId { get; set; }
        public int? ParagraphSentenceId { get; set; }
        public int? OrderInParagraph { get; set; }
        public int? OrderInSentence { get; set; }
        public string EquationContent { get; set; }
        public string Style { get; set; }
    }
}
