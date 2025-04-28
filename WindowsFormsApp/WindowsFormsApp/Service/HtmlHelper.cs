using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mshtml;
using System.IO;
using System.Windows.Forms;
using WindowsFormsApp.Model;
using System.Text.RegularExpressions;

namespace WindowsFormsApp.Service
{
    public class HtmlHelper
    {
        // Lấy heading level
        public static int GetHeadingLevel(string tag) // ok
        {
            if (tag.Length == 2 && tag[0] == 'h' && Char.IsDigit(tag[1]))
            {
                int level;
                if (int.TryParse(tag[1].ToString(), out level))
                {
                    return level;
                }
            }
            return 1;
        }

        public static bool IsParagraphInTable(IHTMLElement element) // ok
        {
            var parent = element.parentElement;
            string parentTag = "";
            if (parent != null && parent.tagName != null)
            {
                parentTag = parent.tagName.ToString().ToLower();
            }

            if (parentTag == "td")
            {
                return true;
            }
            return false;
        }

        // ---------------------------------------------------- Xử lý tách para, img, equa --------------------------------------------

        public class ContentFragment // object return
        {
            public string Type { get; set; } // "text", "image", "equation"
            public string Content { get; set; }
            public string Style { get; set; }
            public int? OrderInSentence { get; set; }
        }

        // Hàm tách các fragment trong 1 thẻ <p>
        public static List<ContentFragment> ExtractOrderedFragments(IHTMLDOMNode node)
        {
            List<ContentFragment> fragments = new List<ContentFragment>();

            // Duyệt qua cây DOM bắt đầu từ node hiện tại
            TraverseNode(node, null, fragments);

            return fragments;
        }

        /// <summary>
        /// Duyệt qua từng node trong cây DOM để trích xuất nội dung
        /// </summary>
        private static void TraverseNode(IHTMLDOMNode current, string parentStyle, List<ContentFragment> fragments)
        {
            if (current.nodeType == 3) // TEXT_NODE
            {
                string text = current.nodeValue as string;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    fragments.Add(new ContentFragment
                    {
                        Type = "text",
                        Content = text,
                        Style = parentStyle
                    });
                }
                return;
            }

            if (current.nodeType == 1) // ELEMENT_NODE
            {
                var element = current as IHTMLElement;
                string tagStyle = TagToStyle(element.tagName);
                //string inlineStyle = element.getAttribute("style") != null ? element.getAttribute("style").ToString() : null;
                string inlineStyle = (element.style != null) ? element.style.cssText : null;
                string currentStyle = MergeStyle(tagStyle, inlineStyle);
                string mergedStyle = MergeStyle(parentStyle, currentStyle);

                // Heading
                if (element != null && element.tagName.StartsWith("h", StringComparison.OrdinalIgnoreCase))
                {
                    string headingStyle = "";

                    ExtractHeadingStyle(element, ref headingStyle);

                    string headingText = element.innerText;
                    if (!string.IsNullOrWhiteSpace(headingText))
                    {
                        fragments.Add(new ContentFragment
                        {
                            Type = "text",
                            Content = headingText.Trim(),
                            Style = headingStyle
                        });
                    }
                    return;
                }

                // EQUATION: <span> chỉ chứa <img>
                if (element != null && element.tagName != null &&
                    element.tagName.Equals("SPAN", StringComparison.OrdinalIgnoreCase))
                {
                    var children = (element as IHTMLElement).children;
                    if (children.length == 1)
                    {
                        var child = children.item(0) as IHTMLElement;
                        if (child != null && child.tagName != null &&
                            child.tagName.Equals("IMG", StringComparison.OrdinalIgnoreCase))
                        {
                            string src = child.getAttribute("src") != null ? child.getAttribute("src").ToString() : null;
                            string width = child.getAttribute("width") != null ? child.getAttribute("width").ToString() : null;
                            string height = child.getAttribute("height") != null ? child.getAttribute("height").ToString() : null;

                            string spanStyle = (element.style != null) ? element.style.cssText : null;

                            string imgStyle = "";
                            if (!string.IsNullOrEmpty(width)) imgStyle += "width:" + width + ";";
                            if (!string.IsNullOrEmpty(height)) imgStyle += "height:" + height + ";";

                            string finalStyle = MergeStyle(spanStyle, imgStyle);

                            fragments.Add(new ContentFragment
                            {
                                Type = "equation",
                                Content = src,
                                Style = finalStyle
                            });
                            return;
                        }
                    }
                }

                // <img> độc lập
                if (element != null && element.tagName != null &&
                    element.tagName.Equals("IMG", StringComparison.OrdinalIgnoreCase))
                {
                    string src = element.getAttribute("src") != null ? element.getAttribute("src").ToString() : null;
                    string width = element.getAttribute("width") != null ? element.getAttribute("width").ToString() : null;
                    string height = element.getAttribute("height") != null ? element.getAttribute("height").ToString() : null;
                    string alt = element.getAttribute("alt") != null ? element.getAttribute("alt").ToString() : null;

                    string imgStyle = "";
                    if (!string.IsNullOrEmpty(width)) imgStyle += "width:" + width + ";";
                    if (!string.IsNullOrEmpty(height)) imgStyle += "height:" + height + ";";
                    if (!string.IsNullOrEmpty(alt)) imgStyle += "alt:\"" + alt + "\";";

                    string finalStyle = MergeStyle(mergedStyle, imgStyle);

                    fragments.Add(new ContentFragment
                    {
                        Type = "image",
                        Content = src,
                        Style = finalStyle
                    });
                    return;
                }

                // Các thẻ khác: duyệt tiếp
                foreach (IHTMLDOMNode child in current.childNodes)
                {
                    TraverseNode(child, mergedStyle, fragments);
                }
            }
        }

        // Hàm đệ quy lấy style trong heading
        private static void ExtractHeadingStyle(IHTMLElement el, ref string headingStyle)
        {
            if (el == null || el.tagName == null) return;

            string tagStyle = TagToStyle(el.tagName);
            if (!string.IsNullOrEmpty(tagStyle))
            {
                headingStyle += tagStyle;
            }

            if (el.tagName.Equals("SPAN", StringComparison.OrdinalIgnoreCase))
            {
                if (el.style != null && !string.IsNullOrEmpty(el.style.cssText))
                {
                    headingStyle += el.style.cssText;
                }
            }

            var children = el.children;
            for (int i = 0; i < children.length; i++)
            {
                ExtractHeadingStyle(children.item(i) as IHTMLElement, ref headingStyle);
            }
        }

        // Gộp style cha-con
        private static string MergeStyle(string parent, string child)
        {
            Dictionary<string, string> styleDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            Action<string> parse = delegate(string style)
            {
                if (string.IsNullOrWhiteSpace(style)) return;

                string[] parts = style.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    var kv = part.Split(new[] { ':' }, 2);
                    if (kv.Length == 2)
                    {
                        styleDict[kv[0].Trim()] = kv[1].Trim();
                    }
                }
            };

            parse(parent);
            parse(child);

            return string.Join(";", styleDict.Select(kv => kv.Key + ":" + kv.Value));
        }

        // Map các tag HTML đặc biệt thành style
        private static string TagToStyle(string tagName)
        {
            if (string.IsNullOrEmpty(tagName)) return null;

            switch (tagName.ToLower())
            {
                case "b": return "font-weight:bold;";
                case "strong": return "font-weight:bold;";
                case "i": return "font-style:italic;";
                case "em": return "font-style:italic;";
                case "u": return "text-decoration:underline;";
                case "s":
                case "strike":
                case "del": return "text-decoration:line-through;";
                default: return null;
            }
        }
    }
}
