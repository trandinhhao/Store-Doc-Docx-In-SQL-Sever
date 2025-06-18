using System;
using System.Collections.Generic;

namespace WindowsFormsApp.Model
{
    public class Documents
    {
        public int DocumentId { get; set; }
        public string Title { get; set; }
        public string OriginalFormat { get; set; }
        public string Author { get; set; }
        public long? FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public string FilePath { get; set; }
    }
}

