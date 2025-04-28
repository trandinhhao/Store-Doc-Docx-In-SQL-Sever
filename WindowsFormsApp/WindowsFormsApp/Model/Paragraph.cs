using System;
using System.Collections.Generic;

namespace WindowsFormsApp.Model
{
    public class Paragraph
    {
        public int ParagraphId { get; set; }
        public int SectionId { get; set; }
        public int OrderInSection { get; set; }
        public string ParagraphHTML { get; set; }
    }
}

