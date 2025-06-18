using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mshtml;
using System.IO;
using System.Windows.Forms;
using WindowsFormsApp.Model;
using WindowsFormsApp.Repository;
using System.Text.RegularExpressions;

namespace WindowsFormsApp.Service
{
    class TableService
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

        // sl hang
        public static int GetTableRowCount(IHTMLTable table)
        {
            return table.rows.length;
        }

        // sl cot
        public static int GetTableColumnCount(IHTMLTable table)
        {
            if (table == null || table.rows.length == 0) return 0;

            IHTMLTableRow firstRow = (IHTMLTableRow)table.rows.item(0, null);
            return firstRow.cells.length;
        }

        public static int GetRowColumnCount(IHTMLTableRow currentRow)
        {
            if (currentRow == null || currentRow.cells.length == 0) return 0;

            return currentRow.cells.length;
        }

        // xu ly cells
        public static void ProcessTableCells(IHTMLTable table, int tableId)
        {
            int rowCount = GetTableRowCount(table);
            Regex sentenceEnd = new Regex(@"[\.!\?]\s*$");
            //
            int sentenceCellIndex = 0;

            for (int j = 0; j < rowCount; j++)
            {
                IHTMLTableRow row = (IHTMLTableRow)table.rows.item(j, null);

                for (int k = 0; k < GetRowColumnCount(row); k++)
                {
                    // Tạo mới cell
                    TableCells tableCells = new TableCells
                    {
                        TableId = tableId,
                        RowInTable = j + 1,
                        ColumnInTable = k + 1
                    };
                    tabCelRepo.SaveTableCell(tableCells);
                    int tableCelId = tabCelRepo.GetLastInsertedTableCellId();
                    // reset
                    sentenceCellIndex = 0;

                    IHTMLTableCell cell = (IHTMLTableCell)row.cells.item(k, null);

                    var fragments = SentenceService.MergeIntoSentence(
                        HtmlHelper.ExtractOrderedFragments(cell as IHTMLDOMNode)
                    );

                    int OrderInTableCell = 0;
                    int sentenceIdInDb = 0;
                    bool isEnd = true;

                    if (fragments != null)
                    {
                        foreach (var fragment in fragments)
                        {
                            if (fragment != null){

                                if (fragment.Type == "text" && !string.IsNullOrWhiteSpace(fragment.Content))
                                {
                                    var tcs = new TableCellSentences
                                    {
                                        TableCellId = tableCelId,
                                        OrderInTableCell = ++OrderInTableCell,
                                        TextContent = fragment.Content.Trim()
                                    };

                                    tabCelSenRepo.SaveTableCellSentence(tcs);
                                    sentenceIdInDb = tabCelSenRepo.GetLastInsertedTableCellSentenceId();

                                    isEnd = sentenceEnd.IsMatch(fragment.Content.Trim());
                                }
                                else if (fragment.Type == "image")
                                {
                                    var tci = new TableCellImages
                                    {
                                        TableCellId = tableCelId,
                                        OrderInTableCell = ++OrderInTableCell,
                                        ImagePath = fragment.Content.Substring(6),
                                        ImageType = Path.GetExtension(fragment.Content).Trim('.'),
                                        Title = "Ảnh trong bảng",
                                        Style = fragment.Style
                                    };

                                    tabCelImgRepo.SaveTableCellImage(tci);
                                }
                                else if (fragment.Type == "equation")
                                {
                                    var tce = new TableCellEquations
                                    {
                                        TableCellId = tableCelId,
                                        TableCellSentenceId = isEnd ? null : (int?)sentenceIdInDb,
                                        OrderInTableCell = ++OrderInTableCell,
                                        OrderInSentence = isEnd ? null : (int?)fragment.OrderInSentence,
                                        EquationContent = fragment.Content.Substring(6),
                                        Style = fragment.Style
                                    };

                                    tabCelEquaRepo.SaveTableCellEquation(tce);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
