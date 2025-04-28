using System;
using System.Collections.Generic;

namespace WindowsFormsApp.Model
{
    public class Section
    {
        public int SectionId { get; set; }
        public int DocumentId { get; set; }
        public string Title { get; set; }
        public int OrderInDocument { get; set; }
    }
}

