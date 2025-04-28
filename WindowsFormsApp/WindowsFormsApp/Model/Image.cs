using System;
using System.Collections.Generic;

namespace WindowsFormsApp.Model
{
    public class Image
    {
        public int ImageId { get; set; }
        public int ParagraphId { get; set; }
        public int OrderInParagraph { get; set; }
        public string ImageContent { get; set; }
        public string ImageType { get; set; }
        public string Style { get; set; }
    }
}

