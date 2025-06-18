using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp.Model
{
    public class Tables
    {
        public int TableId { get; set; }
        public int ParagraphId { get; set; }
        public string Title { get; set; }
        public int OrderInParagraph { get; set; }
        public int NumRow { get; set; }
        public int NumColumn { get; set; }
    }
}
