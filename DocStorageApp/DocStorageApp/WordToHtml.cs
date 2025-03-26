using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocStorageApp
{
    public class WordToHtml
    {
        private Application wordApp;
        private Document wordDoc;

        public WordToHtml()
        {
            wordApp = new Application();
        }

        public string ConvertToHtml(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("Không tìm thấy file: " + filePath);

                string outputHtmlPath = Path.ChangeExtension(filePath, ".html");

                // Mở file Word (hỗ trợ cả .doc và .docx)
                wordDoc = wordApp.Documents.Open(filePath);

                // Lưu thành file HTML
                wordDoc.SaveAs2(outputHtmlPath, WdSaveFormat.wdFormatFilteredHTML);

                return outputHtmlPath;
            }
            catch (Exception ex)
            {
                return "Lỗi: " + ex.Message;
            }
            finally
            {
                // Đóng tài liệu và ứng dụng Word
                wordDoc.Close(false);
                wordApp.Quit();
            }
        }
    }
}
