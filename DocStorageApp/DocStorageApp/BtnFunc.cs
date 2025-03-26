using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace DocStorageApp
{
    class BtnFunc
    {
        public static HtmlStructor ParseHtml() // nhận đầu vào là file doc / docx sang ép sang html
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Chọn file Word",
                Filter = "Word Documents (*.doc;*.docx)|*.doc;*.docx|All Files (*.*)|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                string fileExtension = Path.GetExtension(filePath).ToLower();

                if (fileExtension == ".doc" || fileExtension == ".docx")
                {
                    try
                    {
                        // Chuyển đổi file Word sang HTML
                        WordToHtml converter = new WordToHtml();
                        string htmlContent = converter.ConvertToHtml(filePath);

                        // Trích xuất thông tin từ HTML , hình như đang bị trích thiếu hay sao đó ?
                        HtmlStructor docData = new HtmlStructor
                        {// ép từ file html sang
                            Title = HtmlExtractor.ExtractTitle(htmlContent),
                            Author = HtmlExtractor.ExtractAuthor(htmlContent),
                            PublishDate = HtmlExtractor.ExtractPublishDate(htmlContent),
                            TableOfContents = HtmlExtractor.ExtractTableOfContents(htmlContent),
                            Sections = HtmlExtractor.ExtractContent(htmlContent)
                        };

                        MessageBox.Show("File đã được chuyển đổi thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return docData;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi chuyển đổi hoặc trích xuất dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("File không hợp lệ, vui lòng chọn file DOC hoặc DOCX.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            return null;
        }
    }
}