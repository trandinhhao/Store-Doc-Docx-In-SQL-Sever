using System;
using System.IO;
using System.Text;
using Microsoft.Office.Interop.Word; // Thư viện để tương tác với Microsoft Word
using System.Windows.Forms;

namespace WindowsFormsApp.Service
{
    public class WordToHtmlConverter
    {
        /// <summary>
        /// (.doc, .docx) -> folder(.html, .img)
        /// </summary>
        /// input: wordFilePath - File Path
        /// output: string .html
        public string ConvertToHtmlString(string wordFilePath)
        {
            // Khởi tạo ứng dụng Word mới
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            Document wordDoc = null;

            // Lấy tên file không có phần mở rộng để tạo tên folder
            string baseFileName = Path.GetFileNameWithoutExtension(wordFilePath);

            // Lấy đường dẫn Desktop và tạo đường dẫn đến thư mục DB
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputFolder = Path.Combine(desktopPath, "DB", baseFileName);

            // Tạo thư mục DB nếu chưa tồn tại
            if (!Directory.Exists(Path.Combine(desktopPath, "DB")))
            {
                Directory.CreateDirectory(Path.Combine(desktopPath, "DB"));
            }

            // Kiểm tra nếu file đã tồn tại trong DB
            if (Directory.Exists(outputFolder))
            {
                MessageBox.Show("File đã tồn tại trong CSDL, không thể chuyển đổi thêm lần nữa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            // Tạo thư mục mới cho file và ảnh
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            // Tạo đường dẫn cho file HTML output
            string htmlFilePath = Path.Combine(outputFolder, baseFileName + ".html");

            try
            {
                // Các tham số cần thiết cho việc mở và lưu file Word
                object readOnly = true; // Chỉ cho phép đọc file
                object isVisible = false; // Ẩn cửa sổ Word khi xử lý
                object missing = Type.Missing; // Giá trị mặc định cho các tham số không bắt buộc
                object inputPath = wordFilePath; // Đường dẫn file Word đầu vào
                object outputPath = htmlFilePath; // Đường dẫn file HTML đầu ra
                object format = WdSaveFormat.wdFormatFilteredHTML; // Định dạng lưu là HTML đã được lọc (loại bỏ các thẻ không cần thiết)

                // Mở file Word với các tham số đã cấu hình
                // Các tham số ref missing là các tham số tùy chọn không cần thiết
                wordDoc = wordApp.Documents.Open(ref inputPath, ref missing, ref readOnly,
                                                 ref missing, ref missing, ref missing,
                                                 ref missing, ref missing, ref missing,
                                                 ref missing, ref missing, ref isVisible,
                                                 ref missing, ref missing, ref missing, ref missing);

                // Lưu file Word dưới dạng HTML
                // Các tham số ref missing là các tham số tùy chọn không cần thiết
                wordDoc.SaveAs(ref outputPath, ref format, ref missing, ref missing,
                               ref missing, ref missing, ref missing, ref missing,
                               ref missing, ref missing, ref missing, ref missing,
                               ref missing, ref missing, ref missing, ref missing);

                // Đóng file Word và thoát ứng dụng Word
                wordDoc.Close(false, ref missing, ref missing); // false: không lưu thay đổi
                wordApp.Quit(false, ref missing, ref missing); // false: không lưu thay đổi

                // Đọc nội dung file HTML đã tạo với encoding UTF-8
                return File.ReadAllText(htmlFilePath, Encoding.UTF8);
            }
            finally
            {
                // Giải phóng bộ nhớ COM objects để tránh memory leak
                if (wordDoc != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(wordDoc);
                if (wordApp != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
            }
        }
    }
}
