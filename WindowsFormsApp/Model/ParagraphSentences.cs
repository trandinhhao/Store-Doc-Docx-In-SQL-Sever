using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp.Model
{
    public class ParagraphSentences
    {
        public int ParagraphSentenceId { get; set; }
        public int ParagraphId { get; set; }
        public int OrderInParagraph { get; set; }
        public string TextContent { get; set; }
    }
}
