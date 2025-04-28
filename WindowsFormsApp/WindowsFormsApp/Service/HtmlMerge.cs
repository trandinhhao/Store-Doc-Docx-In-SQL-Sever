using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mshtml;
using System.IO;
using System.Windows.Forms;
using WindowsFormsApp.Model;
using WindowsFormsApp.Service;
using WindowsFormsApp.Repository;

namespace WindowsFormsApp.Service
{
    public class HtmlMerge
    {
        public static string GenerateHtmlFromEntities(int documentId)
        {
            // Khởi tạo các repository để lấy dữ liệu từ DB
            DocumentRepository docRepo = new DocumentRepository();
            HeadRepository headRepo = new HeadRepository();
            SectionRepository sectionRepo = new SectionRepository();
            HeadingRepository headingRepo = new HeadingRepository();
            TableElementRepository tableRepo = new TableElementRepository();
            ParagraphRepository paraRepo = new ParagraphRepository();
            SentenceRepository sentenceRepo = new SentenceRepository();
            ImageRepository imageRepo = new ImageRepository();
            EquationRepository equationRepo = new EquationRepository();

            // Bắt đầu html

            // StringBuilder để build HTML
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");

            // 1. Thêm phần head---------------------------------------------------------------------------------------
            Head head = headRepo.GetHeadByDocumentId(documentId);
            if (head != null && !string.IsNullOrEmpty(head.HeadHTML))
            {
                sb.AppendLine(head.HeadHTML);
            }
            else
            {
                sb.AppendLine("<head></head>");
            }

            // 2. Thêm phần body--------------------------------------------------------------------------------------
            sb.AppendLine("<body lang=EN-US style='word-wrap:break-word'>");

            // Lấy tất cả section theo thứ tự
            List<Section> sections = sectionRepo.GetSectionsByDocumentId(documentId);

            foreach (var section in sections)
            {
                // Thêm section
                sb.AppendLine("<div class=" + section.Title + ">");

                // Lấy tất cả các Heading, Table, Paragraph trong section này
                var headings = headingRepo.GetHeadingsBySection(section.SectionId);
                var tables = tableRepo.GetTablesBySection(section.SectionId);
                var paragraphs = paraRepo.GetParagraphsBySection(section.SectionId);

                // Gom hết các thành phần lại thành 1 danh sách, để sort đúng thứ tự xuất hiện
                var elements = new List<dynamic>();

                foreach (var heading in headings)
                    elements.Add(new { Type = "Heading", Order = heading.OrderInSection, Obj = heading });
                foreach (var table in tables)
                    elements.Add(new { Type = "Table", Order = table.OrderInSection, Obj = table });
                foreach (var paragraph in paragraphs)
                    elements.Add(new { Type = "Paragraph", Order = paragraph.OrderInSection, Obj = paragraph });

                // Sort theo OrderInSection
                elements = elements.OrderBy(e => e.Order).ToList();

                // Duyệt từng element
                foreach (var element in elements)
                {
                    if (element.Type == "Heading")
                    {
                        Heading heading = (Heading)element.Obj;
                        sb.Append("<h" + heading.Level + " ");
                        if (!string.IsNullOrEmpty(heading.Style))
                        {
                            sb.Append(" style='" + heading.Style + "'");
                        }
                        sb.Append(">");
                        sb.Append(System.Web.HttpUtility.HtmlEncode(heading.Title));
                        sb.AppendLine("</h" + heading.Level + ">");
                    }
                    else if (element.Type == "Table")
                    {
                        TableElement table = (TableElement)element.Obj;
                        sb.AppendLine(table.TableHTML);
                    }
                    else if (element.Type == "Paragraph")
                    {
                        var paragraph = (Paragraph)element.Obj;
                        // test------------------------------------------------
                        //sb.AppendLine(paragraph.ParagraphHTML);
                        //continue;

                        sb.Append("<p class=MsoNormal>");

                        // Lấy các sentence/image/equation trong paragraph theo thứ tự
                        var sentences = sentenceRepo.GetSentencesByParagraphId(paragraph.ParagraphId);
                        var images = imageRepo.GetImagesByParagraphId(paragraph.ParagraphId);
                        var equations = equationRepo.GetEquationsByParagraphId(paragraph.ParagraphId);

                        var fragList = new List<dynamic>();
                        foreach (var s in sentences) fragList.Add(new { Type = "Sentence", Order = s.OrderInParagraph, Obj = s });
                        foreach (var i in images) fragList.Add(new { Type = "Image", Order = i.OrderInParagraph, Obj = i });
                        foreach (var eq in equations) fragList.Add(new { Type = "Equation", Order = eq.OrderInParagraph, Obj = eq });

                        fragList = fragList.OrderBy(f => f.Order).ToList();

                        foreach (var frag in fragList)
                        {
                            if (frag.Type == "Sentence")
                            {
                                var s = (Sentence)frag.Obj;
                                if (!string.IsNullOrEmpty(s.Style))
                                {
                                    sb.Append("<span ");
                                    sb.Append("style= '" + s.Style + "'>");
                                    sb.Append(System.Web.HttpUtility.HtmlEncode(s.TextContent)); // Encodean  vì nếu nội dung câu có chứa các ký tự đặc biệt <, >, &, ", '
                                    sb.AppendLine("</span>");
                                }
                                else
                                {
                                    sb.Append(System.Web.HttpUtility.HtmlEncode(s.TextContent));
                                }
                            }
                            else if (frag.Type == "Image")
                            {
                                var i = (Image)frag.Obj;
                                sb.Append("<img ");
                                sb.Append(" src=\"" + i.ImageContent + "\" ");
                                if (!string.IsNullOrEmpty(i.Style))
                                {
                                    var parts = i.Style.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                    foreach (var part in parts)
                                    {
                                        sb.Append(part.Trim().Replace(":", "=") + " ");
                                    }
                                }
                                sb.AppendLine(">");
                            }
                            else if (frag.Type == "Equation")
                            {
                                var eq = (Equation)frag.Obj;
                                // tach style
                                var parts = eq.Style.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                sb.Append("<span style='");
                                foreach (var part in parts)
                                {
                                    if (!part.TrimStart().StartsWith("width", StringComparison.OrdinalIgnoreCase) &&
                                        !part.TrimStart().StartsWith("height", StringComparison.OrdinalIgnoreCase))
                                        sb.Append(part.Trim() + "; ");
                                }
                                sb.Remove(sb.Length - 1, 1); // xoa cai ;
                                sb.Append("'>");
                                sb.Append("<img src=\"" + eq.EquationContent + "\" ");
                                foreach (var part in parts)
                                {
                                    if (part.TrimStart().StartsWith("width", StringComparison.OrdinalIgnoreCase) ||
                                        part.TrimStart().StartsWith("height", StringComparison.OrdinalIgnoreCase))
                                        sb.Append(part.Trim().Replace(":", "=") + " ");
                                }
                                sb.AppendLine("></span>");
                            }
                            else
                            {
                                sb.Append("&nbsp;");
                            }
                        }

                        // Đóng thẻ </p>
                        sb.AppendLine("</p>");
                    }
                }

                // Đóng thẻ div section
                sb.AppendLine("</div>");
            }

            // Đóng body và html
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            // Return HTML đã build
            return sb.ToString();
        }
    }
}
