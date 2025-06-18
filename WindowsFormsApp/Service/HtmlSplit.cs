using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using mshtml;
using System.IO;
using System.Windows.Forms;
using WindowsFormsApp.Model;
using WindowsFormsApp.Service;
using WindowsFormsApp.Repository;


namespace WindowsFormsApp.Service
{
    public class HtmlSplit
    {
        /// <summary>
        /// Tách html -> save vào DB
        /// </summary>
        /// input: filePath: link file - htmlContent: file .html vua chuyen doi
        /// output: void: save data to DB
        public static void ParseHtmlToEntities(string filePath, string htmlContent)
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

            // Tạo WebBrowser để load nội dung HTML
            WebBrowser browser = new WebBrowser();
            browser.DocumentText = htmlContent;

            // Đợi cho đến khi trang load xong hoàn toàn
            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            // Lấy DOM của tài liệu HTML để dễ dàng truy cập và phân tích các phần tử
            IHTMLDocument2 doc = (IHTMLDocument2)browser.Document.DomDocument;

            // Tao moi Document
            var fileInfo = new FileInfo(filePath);
            Documents document = new Documents
            {
                Title = Path.GetFileNameWithoutExtension(filePath),
                OriginalFormat = Path.GetExtension(filePath).Trim('.'),
                Author = WordHelper.GetAuthor(filePath),
                FileSize = (long)Math.Round(fileInfo.Length / 1024.0, 2),
                UploadDate = DateTime.Now,
                PublishDate = WordHelper.GetCreateDate(filePath),
                FilePath = filePath
            };
            docRepo.SaveDocument(document);
            int documentId = docRepo.GetLastInsertedDocumentId();

            // <body>...</body>---------------------------------------------------------------------------------------------------------------------

            //IHTMLDOMNode bodyNode = (IHTMLDOMNode)doc.body;
            //IHTMLDOMChildrenCollection bodyChildren = (IHTMLDOMChildrenCollection)bodyNode.childNodes;

            // Index
            int sectionIndex = 0;
            int paragraphIndex = 0;
            int sentenceIndex = 0;
            int equationIndex = 0;
            // check dấu câu
            Regex sentenceEnd = new Regex(@"[\.!\?]\s*$");
            // count
            int tabCelSenOrEquaOrImg = 0;
            // Id
            int sectionId = -1;
            int paragraphId = -1;

            // bat dau tao 1 section moi luon, voi title la tieude file word, chua tat ca cac de muc chinh
            sectionIndex++; // section moi
            Sections section = new Sections
            {
                DocumentId = documentId,
                ParentSectionId = null,
                Level = 0,
                Title = Path.GetFileNameWithoutExtension(filePath),
                OrderInDocument = sectionIndex
            };
            secRepo.SaveSection(section);
            sectionId = secRepo.GetLastInsertedSectionId();

            // lay het div
            IHTMLElementCollection allDivs = ((IHTMLDocument3)doc).getElementsByTagName("div");
            foreach (IHTMLElement divElement in allDivs)
            {
                IHTMLDOMNode divNode = (IHTMLDOMNode)divElement;
                IHTMLDOMChildrenCollection divChildren = (IHTMLDOMChildrenCollection)divNode.childNodes;

                for (int i = 0; i < divChildren.length; i++)
                {
                    IHTMLDOMNode childNode = divChildren.item(i);
                    if (childNode.nodeType != 1) continue;

                    IHTMLElement element = (IHTMLElement)childNode;
                    string tag = element.tagName.ToLower();

                    // Section moi
                    if (tag.StartsWith("h"))
                    {
                        sectionIndex++;
                        int level = HtmlHelper.GetHeadingLevel(tag); // level heading
                        string title = element.innerText; // tieu de
                        int? parentSectionId = SectionService.GetParentSectionId(documentId, level); // lay parentHeadingId

                        Sections sec = new Sections
                        {
                            DocumentId = documentId,
                            ParentSectionId = parentSectionId,
                            Level = level,
                            Title = element.innerText,
                            OrderInDocument = sectionIndex
                        };
                        secRepo.SaveSection(sec);
                        sectionId = secRepo.GetLastInsertedSectionId();
                        // reset
                        paragraphIndex = 0;
                    }
                    else if (tag == "p" || tag == "table") // deu coi la paragraph
                    {
                        paragraphIndex++;
                        Paragraphs paragraph = new Paragraphs
                        {
                            SectionId = sectionId,
                            OrderInSection = paragraphIndex
                        };
                        paraRepo.SavePragraph(paragraph);
                        paragraphId = paraRepo.GetLastInsertedParagraphId();
                        // reset
                        sentenceIndex = 0;
                        equationIndex = 0;

                        if (tag == "table")
                        {
                            IHTMLTable htmlTable = element as IHTMLTable;
                            if (htmlTable == null) return;

                            // tao moi table
                            Tables table = new Tables
                            {
                                ParagraphId = paragraphId,
                                Title = "Bảng biểu",
                                OrderInParagraph = 1, // 1 para - 1 table
                                NumRow = TableService.GetTableRowCount(htmlTable),
                                NumColumn = TableService.GetTableColumnCount(htmlTable)
                            };
                            tabRepo.SaveTable(table);
                            int tableId = tabRepo.GetLastInsertedTableId();

                            // xu ly tung cell
                            TableService.ProcessTableCells(htmlTable, tableId); // ?
                        }
                        else if (tag == "p")
                        {
                            int OrderInParagraph = 0;

                            var fragments = SentenceService.MergeIntoSentence(HtmlHelper.ExtractOrderedFragments(element as IHTMLDOMNode));
                            int? sentenceIdInDb = null;
                            bool isEnd = true;
                            
                            if (fragments != null)
                            {
                                foreach (var fragment in fragments)
                                {
                                    if (fragment != null)
                                    {
                                        if (fragment.Type == "text" && !string.IsNullOrWhiteSpace(fragment.Content))
                                        {
                                            ParagraphSentences ps = new ParagraphSentences
                                            {
                                                ParagraphId = paragraphId,
                                                OrderInParagraph = ++OrderInParagraph,
                                                TextContent = fragment.Content.Trim()
                                            };
                                            paraSenRepo.SaveParagraphSentence(ps);
                                            sentenceIdInDb = paraSenRepo.GetLastInsertedParagraphSentenceId();
                                            
                                            isEnd = sentenceEnd.IsMatch(fragment.Content.Trim()) ? true : false;
                                        }
                                        else if (fragment.Type == "image")
                                        {
                                            ParagraphImages pi = new ParagraphImages
                                            {
                                                ParagraphId = paragraphId,
                                                OrderInParagraph = ++OrderInParagraph,
                                                ImagePath = fragment.Content.Substring(6), // loại bỏ "file:/"
                                                ImageType = Path.GetExtension(fragment.Content).Trim('.'), 
                                                Title = "Ảnh",
                                                Style = fragment.Style
                                            };
                                            paraImgRepo.SaveParaImage(pi);
                                        }
                                        else if (fragment.Type == "equation")
                                        {
                                            ParagraphEquations pe = new ParagraphEquations
                                            {
                                                ParagraphId = paragraphId,
                                                ParagraphSentenceId = isEnd ? null : (int?)sentenceIdInDb,
                                                OrderInParagraph = ++OrderInParagraph,
                                                OrderInSentence = isEnd ? null : (int?)fragment.OrderInSentence,
                                                EquationContent = fragment.Content.Substring(6),
                                                Style = fragment.Style
                                            };
                                            paraEquaRepo.SaveParaEquation(pe);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

// . ? !