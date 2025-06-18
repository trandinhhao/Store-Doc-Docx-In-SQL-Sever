using System.Threading.Tasks;
using mshtml;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApp.Model;
using WindowsFormsApp.Service;
using WindowsFormsApp.Repository;

namespace WindowsFormsApp.Service
{
    class SectionService
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

        public static int? GetParentSectionId(int documentId, int currentLevel)
        {
            List<Sections> sectionList = secRepo.GetSectionsByDocumentId(documentId);
            int? parentSectionId = null;

            for (int j = sectionList.Count - 1; j >= 0; j--)
            {
                if (sectionList[j].Level < currentLevel)
                {
                    parentSectionId = sectionList[j].SectionId;
                    break;
                }
            }

            return parentSectionId;
        }
    }
}
