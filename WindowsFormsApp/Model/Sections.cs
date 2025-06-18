using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp.Model
{
    public class Sections
    {
        public int SectionId { get; set; }
        public int DocumentId { get; set; }
        public int? ParentSectionId { get; set; }
        public int Level { get; set; }
        public string Title { get; set; }
        public int OrderInDocument { get; set; }
    }
}
