using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp.Model
{
    public class ParagraphImages
    {
        public int ParagraphImageId { get; set; }
        public int ParagraphId { get; set; }
        public int OrderInParagraph { get; set; }
        public string ImagePath { get; set; }
        public string ImageType { get; set; }
        public string Title { get; set; }
        public string Style { get; set; }
    }
}
