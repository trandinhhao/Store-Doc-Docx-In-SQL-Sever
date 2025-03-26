namespace DocStorageApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnLoadFile;
        private System.Windows.Forms.Button btnSaveToDB;
        private System.Windows.Forms.Button btnLoadFromDb;
        private System.Windows.Forms.DataGridView dgvFiles;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblTOC;
        private System.Windows.Forms.WebBrowser webBrowser1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnLoadFile = new System.Windows.Forms.Button();
            this.btnSaveToDB = new System.Windows.Forms.Button();
            this.btnLoadFromDb = new System.Windows.Forms.Button();
            this.dgvFiles = new System.Windows.Forms.DataGridView();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblTOC = new System.Windows.Forms.Label();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFiles)).BeginInit();
            this.SuspendLayout();

            // 
            // btnLoadFile
            // 
            this.btnLoadFile.Location = new System.Drawing.Point(20, 20);
            this.btnLoadFile.Size = new System.Drawing.Size(100, 30);
            this.btnLoadFile.Text = "Load File";
            this.btnLoadFile.Click += new System.EventHandler(this.BtnLoadFile_Click);

            // 
            // btnSaveToDB
            // 
            this.btnSaveToDB.Location = new System.Drawing.Point(140, 20);
            this.btnSaveToDB.Size = new System.Drawing.Size(100, 30);
            this.btnSaveToDB.Text = "Save to DB";
            this.btnSaveToDB.Click += new System.EventHandler(this.BtnLoadFile_Click);

            // 
            // btnLoadFromDb
            // 
            this.btnLoadFromDb.Location = new System.Drawing.Point(260, 20);
            this.btnLoadFromDb.Size = new System.Drawing.Size(100, 30);
            this.btnLoadFromDb.Text = "Load from DB";
            this.btnLoadFromDb.Click += new System.EventHandler(this.BtnLoadFile_Click);

            // 
            // dgvFiles
            // 
            this.dgvFiles.Location = new System.Drawing.Point(20, 70);
            this.dgvFiles.Size = new System.Drawing.Size(400, 150);
            this.dgvFiles.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.BtnLoadFile_Click);

            // 
            // lblTitle
            // 
            this.lblTitle.Location = new System.Drawing.Point(450, 70);
            this.lblTitle.Size = new System.Drawing.Size(300, 20);
            this.lblTitle.Text = "Title: ";

            // 
            // lblAuthor
            // 
            this.lblAuthor.Location = new System.Drawing.Point(450, 100);
            this.lblAuthor.Size = new System.Drawing.Size(300, 20);
            this.lblAuthor.Text = "Author: ";

            // 
            // lblDate
            // 
            this.lblDate.Location = new System.Drawing.Point(450, 130);
            this.lblDate.Size = new System.Drawing.Size(300, 20);
            this.lblDate.Text = "Date: ";

            // 
            // lblTOC
            // 
            this.lblTOC.Location = new System.Drawing.Point(450, 160);
            this.lblTOC.Size = new System.Drawing.Size(300, 20);
            this.lblTOC.Text = "Table of Contents: ";

            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(20, 250);
            this.webBrowser1.Size = new System.Drawing.Size(700, 300);

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.btnLoadFile);
            this.Controls.Add(this.btnSaveToDB);
            this.Controls.Add(this.btnLoadFromDb);
            this.Controls.Add(this.dgvFiles);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.lblTOC);
            this.Controls.Add(this.webBrowser1);
            this.Text = "Document Storage";
            ((System.ComponentModel.ISupportInitialize)(this.dgvFiles)).EndInit();
            this.ResumeLayout(false);
        }
        #endregion
    }
}
