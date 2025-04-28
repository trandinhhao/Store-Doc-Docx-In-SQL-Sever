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
    public class HtmlSplit
    {
        /// <summary>
        /// Tách html -> save vào DB
        /// </summary>
        /// input: filePath: link file - htmlContent: file .html vua chuyen doi
        /// output: void: save data to DB
        public static void ParseHtmlToEntities(string filePath, string htmlContent)
        {

            // Khoi tao Repo
            DocumentRepository docRepo = new DocumentRepository();
            HeadRepository headRepo = new HeadRepository();
            SectionRepository sectionRepo = new SectionRepository();
            HeadingRepository headingRepo = new HeadingRepository();
            TableElementRepository tableRepo = new TableElementRepository();
            ParagraphRepository paraRepo = new ParagraphRepository();
            SentenceRepository sentenceRepo = new SentenceRepository();
            ImageRepository imageRepo = new ImageRepository();
            EquationRepository equationRepo = new EquationRepository();

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

// B1 - Khởi tạo Document------------------------------------------------------------------------------------------------------------------------
            var fileInfo = new FileInfo(filePath);
            Document document = new Document
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

            // Lấy id của cái Document vừa tạo để thao tác
            int documentId = docRepo.GetLastInsertedDocumentId();

// B2: Với 1 file .html thì có <head> với <body> nhưng nội dung tập chung xử lý body, nên lưu hết <head>...</head> sau còn xử lý-------------------
            
            // Xử lý phần <head>...</head>
            string headContent = "";
            IHTMLDocument3 doc3 = (IHTMLDocument3)doc;
            IHTMLElementCollection headCollection = doc3.getElementsByTagName("head");
            if (headCollection.length > 0)
            {
                IHTMLElement headElement = (IHTMLElement)headCollection.item(0, 0);
                headContent = headElement.outerHTML; // Lấy toàn bộ HTML của head: <head>...</head>
            }
            // tạo mới Head
            Head head = new Head
            {
                DocumentId = documentId,
                HeadHTML = headContent
            };
            headRepo.SaveHead(head);

// B3: Xử lý <body>...</body>---------------------------------------------------------------------------------------------------------------------

            IHTMLElementCollection elements = doc.body.all; // lấy hết body

            // Khai báo các biến đếm để đánh số thứ tự
            int sectionIndex = 0;
            int headingOrTableorParagraphIndex = 0;
            int sentenceOrImageOrEquationIndex = 0;

            // tạo thêm 1 biến để LƯU ID trong csdl
            int sectionIdInDb = -1;
            
            //
            HashSet<int> processedSourceIndexes = new HashSet<int>();

            foreach (IHTMLElement element in elements)
            {
                // ktra trung
                if (processedSourceIndexes.Contains(element.sourceIndex)) continue;

                string tag = element.tagName != null ? element.tagName.ToLower() : ""; // lấy cái tag

                // div: section moi --- done
                if (tag == "div")
                {
                    string className = element.className.ToString(); // className = WordSection1, WordSection2
                    if (className.StartsWith("WordSection")) // ktra xem dung chuan section moi khong
                    {
                        sectionIndex++;
                        Section sec = new Section
                        {
                            DocumentId = documentId,
                            Title = className,
                            OrderInDocument = sectionIndex
                        };
                        sectionRepo.SaveSection(sec);
                        // lay id section trong db
                        sectionIdInDb = sectionRepo.GetLastInsertedSectionId();
                        // reset count, vì section mới thì tất cả cái kia phải đếm lại
                        headingOrTableorParagraphIndex = 0;
                    }
                }
                // heading trong section: <h1>, <h2>, <h3> --- done
                else if (tag.StartsWith("h"))
                {
                    headingOrTableorParagraphIndex++;
                    int level = HtmlHelper.GetHeadingLevel(tag); // Xác định cấp độ của heading (h1 = 1, h2 = 2, ...)
                    string headingTitle = element.innerText;

                    List<Heading> headingsInSection = headingRepo.GetHeadingsBySection(sectionIdInDb); // lay het heading ra de ktra xem cai nao la cha
                    // lay parentHeadingId
                    int? parentHeadingId = null;
                    for (int i = 0; i < headingsInSection.Count; i++)
                    {
                        if (headingsInSection[i].Level < level)
                        {
                            parentHeadingId = headingsInSection[i].HeadingId;
                        }
                    }

                    List<HtmlHelper.ContentFragment> fragments = HtmlHelper.ExtractOrderedFragments((IHTMLDOMNode)element);

                    Heading heading = new Heading
                    {
                        SectionId = sectionIdInDb,
                        ParentHeadingId = parentHeadingId,
                        Title = headingTitle,
                        Level = level,
                        OrderInSection = headingOrTableorParagraphIndex,
                        Style = fragments[0].Style
                    };
                    headingRepo.SaveHeading(heading);
                }
                //table --- done
                else if (tag == "table")
                {
                    headingOrTableorParagraphIndex++;
                    TableElement table = new TableElement
                    {
                        SectionId = sectionIdInDb,
                        OrderInSection = headingOrTableorParagraphIndex,
                        TableHTML = element.outerHTML
                    };
                    tableRepo.SaveTable(table);
                }
                // paragraph, image, equation đều có chung thẻ p ở đầu, có thể test thử -> chỗ này khá khó
                else if (tag == "p")
                {
                    // kiểm tra xem <p> có phải là <p> trong table không, p không được là p trong table không sẽ trùng
                    // -> cha trực tiếp có phải là <td> không
                    if (HtmlHelper.IsParagraphInTable(element))
                    {
                        continue;
                    }

                    headingOrTableorParagraphIndex++;
                    Paragraph paragraph = new Paragraph
                    {
                        SectionId = sectionIdInDb,
                        OrderInSection = headingOrTableorParagraphIndex,
                        ParagraphHTML = element.outerHTML
                    };
                    paraRepo.SaveParagraph(paragraph);
                    // lay id para vua tao
                    int paragraphIdInDBb = paraRepo.GetLastInsertedParagraphId();

                    // reset count
                    sentenceOrImageOrEquationIndex = 0; // đếm cả 3 cùng lúc luôn

                    // Phân tích nội dung <p> này theo đúng thứ tự
                    List<HtmlHelper.ContentFragment> fragments = HtmlHelper.ExtractOrderedFragments((IHTMLDOMNode)element);

                    //List<Sentence> sentenceList = new List<Sentence>();
                    foreach (var frag in fragments)
                    {
                        sentenceOrImageOrEquationIndex++;
                        if (frag.Type == "text")
                        {
                            Sentence sentence = new Sentence
                            {
                                ParagraphId = paragraphIdInDBb,
                                OrderInParagraph = sentenceOrImageOrEquationIndex,
                                TextContent = frag.Content,
                                Style = frag.Style
                            };
                            sentenceRepo.SaveSentence(sentence);
                            //sentenceList.Add(sentence);
                        }
                        else if (frag.Type == "image")
                        {
                            Image image = new Image
                            {
                                ParagraphId = paragraphIdInDBb,
                                OrderInParagraph = sentenceOrImageOrEquationIndex,
                                ImageContent = frag.Content.Substring(6),
                                ImageType = Path.GetExtension(frag.Content).Trim('.'),
                                Style = frag.Style
                            };
                            imageRepo.SaveImage(image);
                        }
                        else if (frag.Type == "equation")
                        {
                            Equation  equation = new Equation
                            {
                                ParagraphId = paragraphIdInDBb,
                                OrderInParagraph = sentenceOrImageOrEquationIndex,
                                EquationContent = frag.Content.Substring(6),
                                Style = frag.Style
                            };
                            equationRepo.SaveEquation(equation);
                        }
                    }

                }

            }
        }
    }
}
