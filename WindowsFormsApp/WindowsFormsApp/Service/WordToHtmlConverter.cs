using System;
using System.IO;
using System.Text;
using Microsoft.Office.Interop.Word;
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
            // run Word
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            Document wordDoc = null;

            // Tạo tên folder output
            string baseFileName = Path.GetFileNameWithoutExtension(wordFilePath);

            // Đặt folder đó trong folder "DB" trên Desktop
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputFolder = Path.Combine(desktopPath, "DB", baseFileName);

            // Nếu "DB" chưa có -> tạo mới
            if (!Directory.Exists(Path.Combine(desktopPath, "DB")))
            {
                Directory.CreateDirectory(Path.Combine(desktopPath, "DB"));
            }

            // Nếu thư mục DB\tênfile đã tồn tại thì báo lỗi và dừng
            if (Directory.Exists(outputFolder))
            {
                MessageBox.Show("File đã tồn tại trong CSDL, không thể chuyển đổi thêm lần nữa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            // Tạo thư mục cho file và ảnh
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            // Đặt đường dẫn cho file HTML
            string htmlFilePath = Path.Combine(outputFolder, baseFileName + ".html");

            try
            {
                // Setup
                object readOnly = true; // Only read
                object isVisible = false; // Khong hien thi Word
                object missing = Type.Missing; // Mac dinh cho tham so khong can thiet
                object inputPath = wordFilePath;
                object outputPath = htmlFilePath;
                object format = WdSaveFormat.wdFormatFilteredHTML; // html format

                // open
                wordDoc = wordApp.Documents.Open(ref inputPath, ref missing, ref readOnly,
                                                 ref missing, ref missing, ref missing,
                                                 ref missing, ref missing, ref missing,
                                                 ref missing, ref missing, ref isVisible,
                                                 ref missing, ref missing, ref missing, ref missing);

                // save
                wordDoc.SaveAs(ref outputPath, ref format, ref missing, ref missing,
                               ref missing, ref missing, ref missing, ref missing,
                               ref missing, ref missing, ref missing, ref missing,
                               ref missing, ref missing, ref missing, ref missing);

                wordDoc.Close(false, ref missing, ref missing);
                wordApp.Quit(false, ref missing, ref missing);

                // Trả về chuỗi HTML, có utf8 (để ý file .html output phải có utf-8, không thì setting lại word)
                return File.ReadAllText(htmlFilePath, Encoding.UTF8);
            }
            finally
            {
                // giải phóng COM
                if (wordDoc != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(wordDoc);
                if (wordApp != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
            }
        }
    }
}
