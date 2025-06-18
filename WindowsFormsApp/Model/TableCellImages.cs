using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp.Model
{
    public class TableCellImages
    {
        public int TableCellImageId {get; set;}
        public int TableCellId {get; set;}
        public int OrderInTableCell {get; set;}
        public string ImagePath {get; set;}
        public string ImageType {get; set;}
        public string Title {get; set;}
        public string Style {get; set;} 
    }
}
