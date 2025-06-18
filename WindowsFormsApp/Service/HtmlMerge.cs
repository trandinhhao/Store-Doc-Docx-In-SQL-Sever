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
        public static string GenerateFromEntities(int documentId)
        {
            //Repo
            DocumentRepository docRepo = new DocumentRepository();
            ParagraphEquationRepository paraEquaRepo = new ParagraphEquationRepository();
            ParagraphImageRepository paraImgRepo = new ParagraphImageRepository();
            ParagraphRepository paraRepo = new ParagraphRepository();
            ParagraphSentenceRepository paraSenRepo = new ParagraphSentenceRepository();
            SectionRepository secRepo = new SectionRepository();
            TableCellEquationRepository tabCelEquaRepo = new TableCellEquationRepository();
            TableCellImageRepository tabCelImgRepo = new TableCellImageRepository();
            TableCellRepository tabCelRepo = new TableCellRepository();
            TableCellSentenceRepository tabCelSenRepo = new TableCellSentenceRepository();
            TableRepository tabRepo = new TableRepository();

            // Lấy thông tin document
            Documents doc = docRepo.GetDocumentById(documentId);
            if (doc == null) return "";

            // Tạo HTML với layout 2 cột
            StringBuilder html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");

            //bắt đầu head
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='UTF-8'>");
            html.AppendLine("<title>" + doc.Title + "</title>");
            html.AppendLine("<style>");
            // CSS cho layout 2 cột và bảng
            html.AppendLine(@"
                body { margin: 0; padding: 0; font-family: Arial, sans-serif; }
                .container { display: flex; }
                .nav-panel { 
                    width: 250px; 
                    height: 100vh; 
                    background: #f5f5f5; 
                    padding: 20px; 
                    overflow-y: auto;
                    position: fixed;
                    left: 0;
                    top: 0;
                    border-right: 2px solid #ccc;
                    box-shadow: 2px 0 5px rgba(0,0,0,0.03);
                }
                .content { 
                    margin-left: 290px; 
                    padding: 40px 40px 40px 40px; 
                    width: calc(100% - 310px);
                    background: #fff;
                    min-height: 100vh;
                }
                .paragraph {
                    margin-bottom: 28px;
                    line-height: 1.7;
                }
                h1, h2, h3, h4, h5, h6 {
                    margin-top: 32px;
                    margin-bottom: 16px;
                }
                .nav-item { 
                    padding: 5px 0; 
                    cursor: pointer;
                }
                .nav-item:hover { 
                    color: #007bff; 
                }
                .nav-item.level-1 { padding-left: 10px; }
                .nav-item.level-2 { padding-left: 20px; }
                .nav-item.level-3 { padding-left: 30px; }
                .nav-item.level-4 { padding-left: 40px; }
                .nav-item.level-5 { padding-left: 50px; }
                .nav-item.level-6 { padding-left: 60px; }
                table {
                    border-collapse: collapse;
                    margin: 24px 0;
                    width: 100%;
                }
                td, th {
                    border: 1px solid #333;
                    padding: 8px 12px;
                    text-align: center;
                    vertical-align: middle;
                }
            ");
            html.AppendLine("</style>");
            html.AppendLine("</head>");

            //bắt đầu body
            html.AppendLine("<body>");
            html.AppendLine("<div class='container'>");
            
            // phần navigation
            html.AppendLine("<div class='nav-panel'>");
            html.AppendLine("<h3>Navigation</h3>");
            List<Sections> sections = secRepo.GetSectionsByDocumentId(documentId);
            foreach (var section in sections)
            {
                if(section.Level == 0) continue;
                string levelClass = string.Format("level-{0}", section.Level);
                html.AppendLine(string.Format("<div class='nav-item {0}' onclick='scrollToSection(\"{1}\")'> {2} </div>", 
                    levelClass, section.SectionId, section.Title));
            }
            html.AppendLine("</div>");

            // phần nội dung
            html.AppendLine("<div class='content'>");
            
            // duyệt từng section
            foreach (var section in sections)
            {
                if(section.Level == 0) continue;
                
                html.AppendLine(string.Format("<div id='section-{0}'>", section.SectionId));
                html.AppendLine(string.Format("<h{0}> {1} </h{0}>", section.Level, section.Title));
                
                // Lấy paragraphs của section
                List<Paragraphs> paragraphs = paraRepo.GetParagraphsBySection(section.SectionId);
                foreach (var para in paragraphs)
                {
                    html.AppendLine("<div class='paragraph'>");

                    var sentences = paraSenRepo.GetSentencesByParagraphId(para.ParagraphId);
                    var images = paraImgRepo.GetImagesByParagraphId(para.ParagraphId);
                    var equations = paraEquaRepo.GetEquationsByParagraphId(para.ParagraphId);

                    var fragList = new List<dynamic>();
                    foreach (var s in sentences) fragList.Add(new { Type = "Sentence", Order = s.OrderInParagraph, Obj = s });
                    foreach (var i in images) fragList.Add(new { Type = "Image", Order = i.OrderInParagraph, Obj = i });
                    foreach (var eq in equations) fragList.Add(new { Type = "Equation", Order = eq.OrderInParagraph, Obj = eq });

                    fragList = fragList.OrderBy(f => f.Order).ToList();

                    foreach (var frag in fragList)
                    {
                        if (frag.Type == "Sentence")
                        {
                            var s = (ParagraphSentences)frag.Obj;
                            html.AppendLine(System.Web.HttpUtility.HtmlEncode(s.TextContent));
                        }
                        else if (frag.Type == "Image")
                        {
                            var i = (ParagraphImages)frag.Obj;
                            html.Append("<img ");
                            html.Append(string.Format("src='{0}' ", i.ImagePath));
                            if (!string.IsNullOrEmpty(i.Style))
                            {
                                html.Append(string.Format("style='{0}' ", i.Style));
                            }
                            html.AppendLine(">");
                        }
                        else if (frag.Type == "Equation")
                        {
                            var eq = (ParagraphEquations)frag.Obj;
                            html.Append("<span ");
                            if (!string.IsNullOrEmpty(eq.Style))
                            {
                                html.Append(string.Format("style='{0}' ", eq.Style));
                            }
                            html.Append(">");
                            html.Append(string.Format("<img src='{0}' ", eq.EquationContent));
                            html.AppendLine("></span>");
                        }
                        html.AppendLine();
                    }

                    // Lấy tables
                    List<Tables> tables = tabRepo.GetTablesByParagraph(para.ParagraphId);
                    foreach (var table in tables)
                    {
                        html.AppendLine("<table>");
                        List<TableCells> cells = tabCelRepo.GetCellsByTableId(table.TableId);
                        
                        // Sắp xếp cells theo row và column
                        var orderedCells = cells.OrderBy(c => c.RowInTable).ThenBy(c => c.ColumnInTable);
                        int currentRow = 0;
                        
                        foreach (var cell in orderedCells)
                        {
                            if (cell.RowInTable > currentRow)
                            {
                                if (currentRow > 0) html.AppendLine("</tr>");
                                html.AppendLine("<tr>");
                                currentRow = cell.RowInTable;
                            }

                            html.AppendLine("<td>");

                            var cellSentences = tabCelSenRepo.GetSentencesByTableCellId(cell.TableCellId);
                            var cellImages = tabCelImgRepo.GetImagesByTableCellId(cell.TableCellId);
                            var cellEquations =  tabCelEquaRepo.GetEquationsByTableCellId(cell.TableCellId);

                            var cellFragList = new List<dynamic>();
                            foreach (var s in cellSentences) cellFragList.Add(new { Type = "Sentence", Order = s.OrderInTableCell, Obj = s });
                            foreach (var i in cellImages) cellFragList.Add(new { Type = "Image", Order = i.OrderInTableCell, Obj = i });
                            foreach (var eq in cellEquations) cellFragList.Add(new { Type = "Equation", Order = eq.OrderInTableCell, Obj = eq });

                            cellFragList = cellFragList.OrderBy(f => f.Order).ToList();
                            
                            foreach (var frag in cellFragList)
                            {
                                if (frag.Type == "Sentence")
                                {
                                    var s = (TableCellSentences)frag.Obj;
                                    html.AppendLine(System.Web.HttpUtility.HtmlEncode(s.TextContent));

                                }
                                else if (frag.Type == "Image")
                                {
                                    var i = (TableCellImages)frag.Obj;
                                    html.Append("<img ");
                                    html.Append(string.Format("src='{0}' ", i.ImagePath));
                                    if (!string.IsNullOrEmpty(i.Style))
                                    {
                                        html.Append(string.Format("style='{0}' ", i.Style));
                                    }
                                    html.AppendLine(">");
                                    }
                                else if (frag.Type == "Equation")
                                {
                                    var eq = (TableCellEquations)frag.Obj;

                                    html.Append("<span ");
                                    if (!string.IsNullOrEmpty(eq.Style))
                                    {
                                        html.Append(string.Format("style='{0}' ", eq.Style));
                                    }
                                    html.Append(">");
                                    html.Append(string.Format("<img src='{0}' ", eq.EquationContent));
                                    html.AppendLine("></span>");
                                }
                                    
                                html.AppendLine();
                            }
                            html.AppendLine("</td>");
                        }
                        if (currentRow > 0) html.AppendLine("</tr>");
                        html.AppendLine("</table>");
                    }

                    html.AppendLine("</div>");
                }
                
                html.AppendLine("</div>");
            }

            html.AppendLine("</div>"); // đóng thẻ div nội dung
            html.AppendLine("</div>"); // đóng thẻ div container

            // JavaScript cho navigation
            html.AppendLine(@"
                <script>
                function scrollToSection(sectionId) {
                    document.getElementById('section-' + sectionId).scrollIntoView({ behavior: 'smooth' });
                }
                </script>
            ");

            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }
    }
}
