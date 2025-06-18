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
using WindowsFormsApp.Repository;

namespace WindowsFormsApp.Service
{
    class SentenceService
    {
        //Repo
        private static DocumentRepository docRepo = new DocumentRepository();
        private static ParagraphEquationRepository paraEquaRepo = new ParagraphEquationRepository();
        private static ParagraphImageRepository paraImgRepo = new ParagraphImageRepository();
        private static ParagraphRepository paraRepo = new ParagraphRepository();
        private static ParagraphSentenceRepository paraSenRepo = new ParagraphSentenceRepository();
        private static SectionRepository secRepo = new SectionRepository();
        private static TableCellEquationRepository tabCelEquaRepo = new TableCellEquationRepository();
        private static TableCellImageRepository tabCelImgRepo = new TableCellImageRepository();
        private static TableCellRepository tabCelRepo = new TableCellRepository();
        private static TableCellSentenceRepository tabCelSenRepo = new TableCellSentenceRepository();
        private static TableRepository tabRepo = new TableRepository();

        public static List<HtmlHelper.ContentFragment> MergeIntoSentence(List<HtmlHelper.ContentFragment> fragments) {
            List<HtmlHelper.ContentFragment> fragmentList = new List<HtmlHelper.ContentFragment>();
            StringBuilder currentSentence = new StringBuilder();
            Regex sentenceEnd = new Regex(@"[\.!\?]\s*$");
            
            int orderInSentence = 0;
            foreach (var fragment in fragments)
            {
                if(fragment.Type == "text"){
                    string trimmedContent = fragment.Content.Trim();
                    currentSentence.Append(trimmedContent);

                    if (sentenceEnd.IsMatch(trimmedContent))
                    {
                        fragmentList.Add(new HtmlHelper.ContentFragment
                        {
                            Type = "text",
                            Content = currentSentence.ToString().Trim(),
                            Style = ""

                        });
                        currentSentence.Clear();
                        orderInSentence = 0;
                    }
                    else
                    {
                        if (currentSentence.Length > 0 && !trimmedContent.StartsWith(" "))
                            currentSentence.Append(" ");
                    }
                }

                else if(fragment.Type == "image" || fragment.Type == "equation"){
                    if(currentSentence.Length > 0){
                        fragmentList.Add(new HtmlHelper.ContentFragment
                        {
                            Type = "text",
                            Content = currentSentence.ToString().Trim(),
                            Style = ""
                        });
                        currentSentence.Clear();
                    }

                    if(fragment.Type == "equation"){
                        fragment.OrderInSentence = ++orderInSentence;
                    }
                    fragmentList.Add(fragment);
                }
            }

            if (currentSentence.Length > 0)
            {
                fragmentList.Add(new HtmlHelper.ContentFragment
                {
                            Type = "text",
                            Content = currentSentence.ToString().Trim(),
                            Style = ""

                });
            }
            fragments.Clear();
            return fragmentList;
        }
    }
}
