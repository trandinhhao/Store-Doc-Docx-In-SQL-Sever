using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApp.Service;
using WindowsFormsApp.Model;
using WindowsFormsApp.Repository;

namespace WindowsFormsApp
{
    public class MainForm : Form
    {
        private TextBox txtInput;
        private Button btnSelectFile;
        private Button btnSaveToDb;
        private Label lblInput;
        private DataGridView dgvDocuments;
        private Button btnDelete;
        private Button btnRefactor;

        private WordToHtmlConverter converter;
        private DocumentRepository documentRepo;

        private string currentHtmlContent;
        private string currentFilePath;

        public MainForm()
        {
            InitializeComponent();
            converter = new WordToHtmlConverter();
            documentRepo = new DocumentRepository();
            LoadDocumentList();
        }

        private void InitializeComponent()
        {
            this.txtInput = new TextBox();
            this.btnSelectFile = new Button();
            this.btnSaveToDb = new Button();
            this.lblInput = new Label();
            this.dgvDocuments = new DataGridView();
            this.btnDelete = new Button();
            this.btnRefactor = new Button();

            this.SuspendLayout();

            // Label
            this.lblInput.AutoSize = true;
            this.lblInput.Location = new System.Drawing.Point(20, 20);
            this.lblInput.Text = "File Word nguồn:";

            // TextBox
            this.txtInput.Location = new System.Drawing.Point(130, 17);
            this.txtInput.Size = new System.Drawing.Size(500, 20);

            // Button Chọn file
            this.btnSelectFile.Location = new System.Drawing.Point(640, 15);
            this.btnSelectFile.Size = new System.Drawing.Size(90, 23);
            this.btnSelectFile.Text = "Chọn file...";
            this.btnSelectFile.Click += new EventHandler(this.btnSelectFile_Click);

            // Button Lưu DB
            this.btnSaveToDb.Location = new System.Drawing.Point(740, 15);
            this.btnSaveToDb.Size = new System.Drawing.Size(100, 23);
            this.btnSaveToDb.Text = "Lưu trữ";
            this.btnSaveToDb.Click += new EventHandler(this.btnSaveToDb_Click);

            // DataGridView
            this.dgvDocuments.Location = new System.Drawing.Point(20, 60);
            this.dgvDocuments.Size = new System.Drawing.Size(950, 400);
            this.dgvDocuments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvDocuments.ReadOnly = true;
            this.dgvDocuments.MultiSelect = false;
            this.dgvDocuments.AutoGenerateColumns = true;

            //this.dgvDocuments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // kích thước tự động đều nhau

            // Button Xóa
            this.btnDelete.Location = new System.Drawing.Point(720, 480);
            this.btnDelete.Size = new System.Drawing.Size(120, 30);
            this.btnDelete.Text = "Xóa tài liệu";
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);

            // Button Refactor
            this.btnRefactor.Location = new System.Drawing.Point(850, 480);
            this.btnRefactor.Size = new System.Drawing.Size(120, 30);
            this.btnRefactor.Text = "Refactor";
            this.btnRefactor.Click += new EventHandler(this.btnRefactor_Click);

            // MainForm
            this.ClientSize = new System.Drawing.Size(1000, 550);
            this.Controls.Add(this.lblInput);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.btnSaveToDb);
            this.Controls.Add(this.dgvDocuments);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnRefactor);
            this.Text = "Lưu tài liệu Word vào SQL Server";

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Word Documents (*.doc;*.docx)|*.doc;*.docx";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtInput.Text = dialog.FileName;
                currentFilePath = dialog.FileName;
            }
        }

        private void btnSaveToDb_Click(object sender, EventArgs e)
        {
            if (!File.Exists(txtInput.Text))
            {
                MessageBox.Show("Vui lòng chọn file hợp lệ.");
                return;
            }

            try
            {
                currentHtmlContent = converter.ConvertToHtmlString(currentFilePath);

                if (string.IsNullOrWhiteSpace(currentHtmlContent))
                {
                    MessageBox.Show("Chuyển đổi thất bại!");
                    return;
                }

                File.WriteAllText("test-output.html", currentHtmlContent, Encoding.UTF8);

                HtmlSplit.ParseHtmlToEntities(currentFilePath, currentHtmlContent);

                MessageBox.Show("Đã phân tách, lưu trữ dữ liệu thành công!");
                LoadDocumentList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message);
            }
        }

        private void LoadDocumentList()
        {
            var docs = documentRepo.GetAllDocuments();
            dgvDocuments.DataSource = docs;

            // set size từng cột:
            if (dgvDocuments.Columns.Count > 0)
            {
                dgvDocuments.Columns[0].Width = 50;   // ID
                dgvDocuments.Columns[1].Width = 145;  // Title
                dgvDocuments.Columns[2].Width = 50;  // Format
                dgvDocuments.Columns[3].Width = 130;  // Author
                dgvDocuments.Columns[4].Width = 52;   // Size
                dgvDocuments.Columns[5].Width = 120;  // UploadDate
                dgvDocuments.Columns[6].Width = 120;  // PublishDate
                dgvDocuments.Columns[7].Width = 240;  // FilePath

                // rename
                dgvDocuments.Columns[0].HeaderText = "ID";   // ID
                dgvDocuments.Columns[1].HeaderText = "Title";  // Title
                dgvDocuments.Columns[2].HeaderText = "Format";   // Format
                dgvDocuments.Columns[3].HeaderText = "Author";  // Author
                dgvDocuments.Columns[4].HeaderText = "Size";   // Size
                dgvDocuments.Columns[5].HeaderText = "UploadDate";  // UploadDate
                dgvDocuments.Columns[6].HeaderText = "PublishDate";  // PublishDate
                dgvDocuments.Columns[7].HeaderText = "FilePath";  // FilePath
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn tài liệu để xóa.");
                return;
            }

            var doc = (Document)dgvDocuments.SelectedRows[0].DataBoundItem;

            try
            {
                documentRepo.DeleteDocument(doc.DocumentId);

                string folderPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "DB",
                    doc.Title
                );

                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }

                MessageBox.Show("Đã xóa tài liệu thành công!");
                LoadDocumentList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message);
            }
        }

        private void btnRefactor_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn tài liệu để refactor.");
                return;
            }

            var doc = (Document)dgvDocuments.SelectedRows[0].DataBoundItem;

            try
            {
                string htmlContent = HtmlMerge.GenerateHtmlFromEntities(doc.DocumentId);

                HtmlToWordConverter.ConvertHtmlStringToWord(htmlContent, doc.OriginalFormat, doc.Title);

                MessageBox.Show("Refactor thành công! Đã sinh file Word.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi refactor: " + ex.Message);
            }
        }
    }
}
