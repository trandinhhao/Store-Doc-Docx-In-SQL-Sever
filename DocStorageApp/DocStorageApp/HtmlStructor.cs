using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocStorageApp
{
    public class HtmlStructor // luu cau truc cua 1 file html, sau khi co file html thi tach no ra vut vao day
    {
        public string Title { get; set; } // Tiêu đề tài liệu
        public string Author { get; set; } // Tên tác giả
        public DateTime? PublishDate { get; set; } // Ngày xuất bản (có thể null nếu không có thông tin)

        public List<TableOfContentItem> TableOfContents { get; set; }
        public List<HtmlSection> Sections { get; set; }

        public HtmlStructor() // giống kiểu hàm khởi tạo ý, dành cho mấy cái dạng list
        {
            TableOfContents = new List<TableOfContentItem>();
            Sections = new List<HtmlSection>();
        }
    }

    public class TableOfContentItem
    {
        public string Title { get; set; } // Tiêu đề mục lục
        public int Level { get; set; } // Cấp độ của mục lục (ví dụ: 1, 2, 3)
        public int Order { get; set; } // Thứ tự xuất hiện trong tài liệu
    }

    public class HtmlSection
    {
        public string Title { get; set; } // Tiêu đề mục
        public List<HtmlContent> Contents { get; set; } // Danh sách nội dung

        public HtmlSection()
        {
            Contents = new List<HtmlContent>(); // 
        }
    }

    public class HtmlContent
    {
        public string Text { get; set; } // Nội dung đoạn văn
        public List<string> Images { get; set; } // Danh sách ảnh (lưu đường dẫn hoặc base64)
        public List<string> Tables { get; set; } // Danh sách bảng (có thể lưu dưới dạng HTML)
        public List<string> Equations { get; set; } // Danh sách công thức toán học/hóa học

        public HtmlContent()
        {
            Images = new List<string>(); // 
            Tables = new List<string>(); // 
            Equations = new List<string>(); // 
        }
    }
}
