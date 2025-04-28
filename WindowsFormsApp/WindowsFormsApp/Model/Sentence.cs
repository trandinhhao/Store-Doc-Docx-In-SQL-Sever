using System;
using System.Collections.Generic;

namespace WindowsFormsApp.Model
{
    public class Sentence
    {
        public int SentenceId { get; set; }
        public int ParagraphId { get; set; }
        public int OrderInParagraph { get; set; }
        public string TextContent { get; set; }
        public string Style { get; set; }
    }
}

