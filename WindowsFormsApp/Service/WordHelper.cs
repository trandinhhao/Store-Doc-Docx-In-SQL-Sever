using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;

namespace WindowsFormsApp.Service
{
    class WordHelper
    {
        public static string GetAuthor(string filePath) // ok
        {
            Word.Application wordApp = new Word.Application();
            Word.Document doc = null;
            string author = "author";

            try
            {
                object missing = Type.Missing;
                object readOnly = true;
                object isVisible = false;
                object path = filePath;

                // Mở file Word dưới dạng ẩn và chỉ đọc
                doc = wordApp.Documents.Open(ref path, ReadOnly: ref readOnly, Visible: ref isVisible);
                wordApp.Visible = false;

                // Lấy author từ thuộc tính của file
                object authorProp = doc.BuiltInDocumentProperties["Author"];
                if (authorProp != null)
                {
                    author = authorProp.GetType().InvokeMember("Value",
                        System.Reflection.BindingFlags.GetProperty,
                        null, authorProp, null) as string ?? "author";
                }
            }
            catch
            {
                
            }
            finally
            {
                if (doc != null) doc.Close(false);
                wordApp.Quit(false);
            }

            return author;
        }

        public static DateTime? GetCreateDate(string filePath) //ok
        {
            Word.Application wordApp = new Word.Application();
            Word.Document doc = null;

            try
            {
                object missing = Type.Missing;
                object readOnly = true;
                object isVisible = false;
                object path = filePath;

                doc = wordApp.Documents.Open(ref path, ReadOnly: ref readOnly, Visible: ref isVisible);
                wordApp.Visible = false;

                object createdProp = doc.BuiltInDocumentProperties["Creation Date"];
                if (createdProp != null)
                {
                    object value = createdProp.GetType().InvokeMember("Value",
                        System.Reflection.BindingFlags.GetProperty,
                        null, createdProp, null);

                    if (value != null && value is DateTime)
                        return (DateTime)value;

                }
            }
            catch
            {
                
            }
            finally
            {
                if (doc != null) doc.Close(false);
                wordApp.Quit(false);
            }

            return null;
        }
    }
}
