using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace DocStorageApp
{
    public class HtmlExtractor // Bao gồm các hàm chuyển tách file html
    {
        public static string ExtractTitle(string html)
        {
            var match = Regex.Match(html, "<h1>(.*?)</h1>", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : "Không có tiêu đề";
        }

        public static string ExtractAuthor(string html)
        {
            var match = Regex.Match(html, "Tác giả:\\s*(.*?)<", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : "Không rõ tác giả";
        }

        public static DateTime? ExtractPublishDate(string html)
        {
            var match = Regex.Match(html, "Ngày xuất bản:\\s*(\\d{2}/\\d{2}/\\d{4})<", RegexOptions.IgnoreCase);
            DateTime date;
            if (match.Success && DateTime.TryParse(match.Groups[1].Value, out date))
            {
                return date;
            }
            return null; // Nếu không có ngày, trả về null
        }

        public static List<TableOfContentItem> ExtractTableOfContents(string html)
        {
            List<TableOfContentItem> tocItems = new List<TableOfContentItem>();
            var matches = Regex.Matches(html, "<h(?<level>\\d)>(?<title>.*?)</h\\d>", RegexOptions.IgnoreCase);

            int order = 1;
            foreach (Match match in matches)
            {
                tocItems.Add(new TableOfContentItem
                {
                    Title = match.Groups["title"].Value.Trim(),
                    Level = int.Parse(match.Groups["level"].Value),
                    Order = order++
                });
            }
            return tocItems;
        }

        public static List<HtmlSection> ExtractContent(string html)
        {
            List<HtmlSection> sections = new List<HtmlSection>();
            var matches = Regex.Matches(html, "<h(?<level>\\d)>(?<title>.*?)</h\\d>(?<content>.*?)(?=<h\\d>|$)", RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                HtmlSection section = new HtmlSection
                {
                    Title = match.Groups["title"].Value.Trim(),
                    Contents = ExtractHtmlContent(match.Groups["content"].Value)
                };
                sections.Add(section);
            }
            return sections;
        }

        private static List<HtmlContent> ExtractHtmlContent(string html)
        {
            List<HtmlContent> contents = new List<HtmlContent>();
            var paragraphs = Regex.Split(html, "<p>|</p>", RegexOptions.IgnoreCase);

            foreach (var para in paragraphs)
            {
                if (!string.IsNullOrWhiteSpace(para))
                {
                    contents.Add(new HtmlContent
                    {
                        Text = para.Trim(),
                        Images = ExtractImages(para),
                        Tables = ExtractTables(para),
                        Equations = ExtractEquations(para)
                    });
                }
            }
            return contents;
        }

        private static List<string> ExtractImages(string html)
        {
            List<string> images = new List<string>();
            var matches = Regex.Matches(html, "<img.*?src=['\"](.*?)['\"]", RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                images.Add(match.Groups[1].Value);
            }
            return images;
        }

        private static List<string> ExtractTables(string html)
        {
            List<string> tables = new List<string>();
            var matches = Regex.Matches(html, "<table.*?</table>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                tables.Add(match.Value);
            }
            return tables;
        }

        private static List<string> ExtractEquations(string html)
        {
            List<string> equations = new List<string>();
            var matches = Regex.Matches(html, "<math.*?</math>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                equations.Add(match.Value);
            }
            return equations;
        }
    }
}
