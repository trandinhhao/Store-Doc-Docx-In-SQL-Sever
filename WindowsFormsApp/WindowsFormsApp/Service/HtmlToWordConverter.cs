using System;
using System.IO;
using System.Text;
using Microsoft.Office.Interop.Word;
using System.Windows.Forms;

namespace WindowsFormsApp.Service
{
    public class HtmlToWordConverter
    {
        /// <summary>
        /// Convert HTML string to Word file (.doc or .docx), embed toàn bộ ảnh vào Word file.
        /// </summary>
        /// <param name="htmlContent">HTML content as string</param>
        /// <param name="fileType">"doc" or "docx"</param>
        /// <param name="fileName">Tên file (không đuôi)</param>
        public static void ConvertHtmlStringToWord(string htmlContent, string fileType, string fileName)
        {
            if (string.IsNullOrEmpty(htmlContent))
                throw new ArgumentException("htmlContent is empty!");

            if (fileType != "doc" && fileType != "docx")
                throw new ArgumentException("Invalid fileType. Must be 'doc' or 'docx'.");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName is empty!");

            // Xác định thư mục lưu file
            string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DB", fileName);
            string outputWordFilePath = Path.Combine(basePath, fileName + "." + fileType);

            // Đảm bảo thư mục chính đã tồn tại, không thì tạo
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            // Kiểm tra file đã tồn tại chưa (đã khôi phục chưa)
            string checkDocxPath = Path.Combine(basePath, fileName + ".docx");
            string checkDocPath = Path.Combine(basePath, fileName + ".doc");

            if ((fileType == "docx" && File.Exists(checkDocxPath)) || (fileType == "doc" && File.Exists(checkDocPath)))
            {
                MessageBox.Show("File Word đã được khôi phục trước đó!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Xử lý nội dung HTML (sửa link ảnh thành tương đối nếu cần)
            htmlContent = ProcessHtmlContent(htmlContent, fileName);

            // Tạo file tạm .html
            string tempHtmlPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".html");
            File.WriteAllText(tempHtmlPath, htmlContent, Encoding.UTF8);

            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            Document wordDoc = null;

            try
            {
                object missing = Type.Missing;
                object readOnly = false;
                object isVisible = false;
                object inputPath = tempHtmlPath;

                // Mở file HTML tạm
                wordDoc = wordApp.Documents.Open(ref inputPath, ref missing, ref readOnly,
                                                 ref missing, ref missing, ref missing,
                                                 ref missing, ref missing, ref missing,
                                                 ref missing, ref missing, ref isVisible,
                                                 ref missing, ref missing, ref missing, ref missing);

                // 🌟 Nhúng toàn bộ ảnh vào file Word
                foreach (InlineShape inlineShape in wordDoc.InlineShapes)
                {
                    if (inlineShape.LinkFormat != null)
                    {
                        inlineShape.LinkFormat.SavePictureWithDocument = true; // Embed ảnh
                        inlineShape.LinkFormat.BreakLink(); // Xóa liên kết ngoài
                    }
                }

                // Lưu file Word cuối cùng
                WdSaveFormat saveFormat = (fileType == "doc") ? WdSaveFormat.wdFormatDocument : WdSaveFormat.wdFormatXMLDocument;
                object outputPath = outputWordFilePath;
                wordDoc.SaveAs(ref outputPath, saveFormat, ref missing, ref missing,
                               ref missing, ref missing, ref missing, ref missing,
                               ref missing, ref missing, ref missing, ref missing,
                               ref missing, ref missing, ref missing, ref missing);
            }
            finally
            {
                // Close and cleanup
                if (wordDoc != null)
                {
                    wordDoc.Close(false);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(wordDoc);
                }

                if (wordApp != null)
                {
                    wordApp.Quit(false);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                }

                // Xóa file HTML tạm
                if (File.Exists(tempHtmlPath))
                {
                    File.Delete(tempHtmlPath);
                }
            }
        }

        // Hàm xử lý nội dung HTML
        private static string ProcessHtmlContent(string htmlContent, string fileName)
        {
            // Thay thế các đường dẫn tuyệt đối nếu cần thiết
            var updatedHtmlContent = htmlContent;

            updatedHtmlContent = updatedHtmlContent.Replace(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DB", fileName + @"\", fileName + "_files\\"),
                fileName + "/" + fileName + "_files/"
            );

            return updatedHtmlContent;
        }
    }
}
