using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Configuration;


namespace DocStorageApp
{
    public class DatabaseHelper // phan nay import vao db nhung dang bi loi
    {
        private string connectionString;

        public DatabaseHelper()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DocStorageDB"].ConnectionString;
        }

        public int SaveDocument(HtmlStructor doc)
        {
            int docId = -1;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 1️⃣ Lưu tài liệu vào bảng Documents
                string query = @"
                INSERT INTO Documents (Title, Author, PublishDate) 
                OUTPUT INSERTED.DocumentID 
                VALUES (@Title, @Author, @PublishDate)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", doc.Title);
                    cmd.Parameters.AddWithValue("@Author", doc.Author);
                    cmd.Parameters.AddWithValue("@PublishDate", (object)doc.PublishDate ?? DBNull.Value);

                    docId = (int)cmd.ExecuteScalar();
                }

                // 2️⃣ Lưu mục lục vào TableOfContents
                foreach (var item in doc.TableOfContents)
                {
                    string tocQuery = @"
                    INSERT INTO TableOfContents (DocumentID, Title, Level, [Order]) 
                    VALUES (@DocumentID, @Title, @Level, @Order)";

                    using (SqlCommand cmd = new SqlCommand(tocQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@DocumentID", docId);
                        cmd.Parameters.AddWithValue("@Title", item.Title);
                        cmd.Parameters.AddWithValue("@Level", item.Level);
                        cmd.Parameters.AddWithValue("@Order", item.Order);
                        cmd.ExecuteNonQuery();
                    }
                }

                // 3️⃣ Lưu các phần nội dung vào bảng Sections và Contents
                foreach (var section in doc.Sections)
                {
                    int sectionId = -1;

                    // Lưu mục vào bảng Sections
                    string sectionQuery = @"
                    INSERT INTO Sections (DocumentID, Title) 
                    OUTPUT INSERTED.SectionID 
                    VALUES (@DocumentID, @Title)";

                    using (SqlCommand cmd = new SqlCommand(sectionQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@DocumentID", docId);
                        cmd.Parameters.AddWithValue("@Title", section.Title);
                        sectionId = (int)cmd.ExecuteScalar();
                    }

                    // 4️⃣ Lưu nội dung từng mục vào bảng Contents
                    foreach (var content in section.Contents)
                    {
                        int contentId = -1;

                        string contentQuery = @"
                        INSERT INTO Contents (SectionID, TextContent) 
                        OUTPUT INSERTED.ContentID 
                        VALUES (@SectionID, @TextContent)";

                        using (SqlCommand cmd = new SqlCommand(contentQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@SectionID", sectionId);
                            cmd.Parameters.AddWithValue("@TextContent", content.Text);
                            contentId = (int)cmd.ExecuteScalar();
                        }

                        // 5️⃣ Lưu hình ảnh vào bảng Images
                        foreach (var image in content.Images)
                        {
                            string imageQuery = @"
                            INSERT INTO Images (ContentID, ImagePath) 
                            VALUES (@ContentID, @ImagePath)";

                            using (SqlCommand cmd = new SqlCommand(imageQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@ContentID", contentId);
                                cmd.Parameters.AddWithValue("@ImagePath", image);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 6️⃣ Lưu bảng biểu vào bảng Tables
                        foreach (var table in content.Tables)
                        {
                            string tableQuery = @"
                            INSERT INTO Tables (ContentID, TableHTML) 
                            VALUES (@ContentID, @TableHTML)";

                            using (SqlCommand cmd = new SqlCommand(tableQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@ContentID", contentId);
                                cmd.Parameters.AddWithValue("@TableHTML", table);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 7️⃣ Lưu công thức toán học vào bảng Equations
                        foreach (var equation in content.Equations)
                        {
                            string equationQuery = @"
                            INSERT INTO Equations (ContentID, EquationText) 
                            VALUES (@ContentID, @EquationText)";

                            using (SqlCommand cmd = new SqlCommand(equationQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@ContentID", contentId);
                                cmd.Parameters.AddWithValue("@EquationText", equation);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }

            return docId;
        }
    }
}
