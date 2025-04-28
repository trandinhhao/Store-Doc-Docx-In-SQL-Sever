using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp.Model
{
    class Heading
    {
        public int HeadingId { get; set; }
        public int SectionId { get; set; }
        public int OrderInSection { get; set; }
        public int? ParentHeadingId { get; set; }
        public int Level { get; set; }
        public string Title { get; set; }
        public string Style { get; set; }
    }
}
