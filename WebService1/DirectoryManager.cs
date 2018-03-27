using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace WebService1
{
    public class DirectoryManager
    {
        public Boolean createDirectory(string userfolder)
        {
            string path = @"C:\webservice\UserDirectories";
            string final = path + "/" + userfolder;
            try
            {

                // Determine whether the directory exists.
                if (!Directory.Exists(final))
                {
                    // Create the directory it does not exist.
                    Directory.CreateDirectory(final);
                  //  File.CreateText(final + @"\myfile.docx");
                    return true;
                }

               


            }
            catch (Exception e)
            {
              
            }
            finally { }
            return false;
        }
        public Boolean saveImageFile(string user_id, string blog_id, string file_name, string base64string) {
           bool flag1=createImgFile(user_id, blog_id, file_name);
           bool flag2=saveImage(user_id, blog_id, file_name, base64string);
            return flag1 && flag2;
        }
        public Boolean createImgFile(string user_id,string blog_id,string file_name)
        {
            string path = @"C:\webservice\UserDirectories";
            string final = path + "/" + user_id + "/" + blog_id;
            try
            {

                // Determine whether the directory exists.
                if (Directory.Exists(final))
                {
                    // Create the directory it does not exist.
                   // Directory.CreateDirectory(final);
                      File.CreateText(final + @"\"+file_name);
                    return true;
                }




            }
            catch (Exception e)
            {

            }
            finally { }
            return false;



        }
        public Boolean createDirectoryOfBlog(string userfolder,string blog_id)
        {
            string path = @"C:\webservice\UserDirectories";
            string final = path + "/" + userfolder+"/"+blog_id;
       
            try
            {

                // Determine whether the directory exists.
                if (!Directory.Exists(final))
                {
                    // Create the directory it does not exist.
                    Directory.CreateDirectory(final);
                  //  File.CreateText(final + @"\myfile.docx");
                    return true;
                }




            }
            catch (Exception e)
            {

            }
            finally { }
            return false;
        }
        public Boolean saveImage(string user_id, string blog_id, string file_name,string base64string)
        {
            string path = @"C:\webservice\UserDirectories";
            string final = path + "/" + user_id + "/" + blog_id;
            try
            {
                if (Directory.Exists(final))
                {
                    string filepath = final + "/" + file_name;
                    byte[] arr = Convert.FromBase64String(base64string);
                    MemoryStream ms = new MemoryStream(arr);
                    Bitmap bmp = new Bitmap(ms);

                    bmp.Save(filepath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ms.Close();
                    return true;

                }
            }
            catch (Exception e)
            {

            }
            finally { }
            return false;
        }
        public Boolean createHTML(string user_id,int blog_id,List<ParaModel> list) {
            bool result = false;
            string templatePath = "C:/webservice/UserDirectories/a.html";
            string mapTemplatePath = MapPath(templatePath);
            string path = "C:/webservice/UserDirectories/" + user_id + "/" + blog_id+"/";
            path = MapPath(path);
            string htmlname = "index.html";
            string htmlpath = Path.Combine(path, htmlname);

            Encoding encode = Encoding.UTF8;
            StreamReader reader = null;
            string data = "";

            try
            {
                reader = new StreamReader(mapTemplatePath, encode);
                data = reader.ReadToEnd();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].tag.Equals("1")) data = AppendImgTag(data, list[i].content);
                    else data = AppendPTag(data, list[i].content);
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                reader.Close();
            }
            try
            {
                //写入html文件
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                File.WriteAllText(htmlpath, data, encode);
                result = true;
            }
            catch (IOException ex)
            {
                
                return false;
            }

            return result;


        }
        private string AppendPTag(string html,string inner) {
            int index = html.LastIndexOf("</body>");
            string ToInsert = "<p>" + inner + "</p>";
            return html.Insert(index, ToInsert);
           
        }
        private string AppendImgTag(string html, string src) {
            int index = html.LastIndexOf("</body>");
            string ToInsert = "<img src=" + '"' + src + '"' + "/>";
            return html.Insert(index, ToInsert);
        }
        private string MapPath(string strPath)
        {
            
                strPath = strPath.Replace("/", "\\");
                if (strPath.StartsWith("\\"))
                {
                    //strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');   
                    strPath = strPath.TrimStart('\\');
                }
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
           
        }
    }
}