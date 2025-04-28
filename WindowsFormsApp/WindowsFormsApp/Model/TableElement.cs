using System;
using System.Collections.Generic;

namespace WindowsFormsApp.Model
{
    public class TableElement
    {
        public int TableId { get; set; }
        public int SectionId { get; set; }
        public int OrderInSection { get; set; }
        public string TableHTML { get; set; }
    }
}

