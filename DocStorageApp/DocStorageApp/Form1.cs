using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;

namespace DocStorageApp
{
    public partial class Form1 : Form
    {

        public Form1() // cái form app
        {
            InitializeComponent();
        }

        private void BtnLoadFile_Click(object sender, EventArgs e) // action nút nhận vào doc docx
        {
            HtmlStructor docData = BtnFunc.ParseHtml();

            if (docData != null)
            {
                DatabaseHelper dbHelper = new DatabaseHelper();
                int docId = dbHelper.SaveDocument(docData);
                MessageBox.Show("Dữ liệu đã lưu vào SQL Server với DocumentID: " + docId);
            }
        }

    }
}
