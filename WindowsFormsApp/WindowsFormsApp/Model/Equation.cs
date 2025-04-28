using System;
using System.Collections.Generic;

namespace WindowsFormsApp.Model
{
    public class Equation
    {
        public int EquationId { get; set; }
        public int ParagraphId { get; set; }
        public int OrderInParagraph { get; set; }
        public string EquationContent { get; set; }
        public string Style { get; set; }
    }
}

