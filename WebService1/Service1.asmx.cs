using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;

namespace WebService1
{
    /// <summary>
    /// Service1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {

        DBOperation dbOperation = new DBOperation();
        DirectoryManager directoryManager = new DirectoryManager();
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello 2018！";
        }
        [WebMethod(Description = "获取所有User的信息")]
        public UserModel[] selectAllUserInfo()
        {
            List<UserModel> list = new List<UserModel>();
            int j = 0;
            foreach (var i in dbOperation.selectAllUser().ToArray())
            {
                list.Add(new UserModel());
                list[j].user_id = i.user_id;
                list[j].nickname = i.nickname;
                list[j].password = i.password;
                list[j].sex = i.sex;
                j++;

            }
            return list.ToArray();
        }
        [WebMethod(Description = "获取一个User的所有部落格")]
        public BlogModel[] selectAllBlogsById(String user_id)
        {
            List<BlogModel> list = new List<BlogModel>();
            int j = 0;
            foreach (var i in dbOperation.selectBlogListByUserId(user_id).ToArray())
            {
                list.Add(new BlogModel());
                list[j].blog_id = i.blog_id;
                list[j].title = i.title;
                list[j].Writer_id = i.Writer_id;
                list[j].Create_time = i.Create_time;
                j++;
            }
            return list.ToArray();
        }

        [WebMethod(Description = "获取一条User的信息")]
        public UserModel selectUserInfoById(string id)
        {
            UserModel um = dbOperation.selectUserById(id);
            return um;
        }
        [WebMethod(Description = "获取所有文本信息信息")]
        public ParaModel[] selectAllTextContent(int blog_id)
        {
            return dbOperation.getContentTextByBlogId(blog_id).ToArray();
        }
        [WebMethod(Description = "获取图片位置")]
        public String[] selectImgById(int id)
        {
            return dbOperation.getImgPosByBlogId(id).ToArray();
        }
        [WebMethod(Description = "获取所有图片的Base64字符串")]
        public String[] selectAllImgById(int id)
        {
            return dbOperation.getImgByPosAndBlogId(id).ToArray();
        }

        [WebMethod(Description = "UserLogin")]
        public UserModel UserLogin(string User_id, string password)
        {
            UserModel userModel = dbOperation.selectUserById(User_id);
            Console.Write(userModel.password);
            if (userModel != null && password == userModel.password) return userModel;
            return null;
        }
        [WebMethod(Description = "增加一条User信息")]
        public Boolean insertUserInfo(string user_id, string password, string uname, string phone, string email)
        {
            if (dbOperation.insertUser(user_id, password, uname, phone, email) && directoryManager.createDirectory(user_id)) return true;
            else return false;
        }
        [WebMethod(Description = "增加一篇部落格")]
        public String insertBlog(string title, string writer_id)
        {
            return dbOperation.insertBlog(title, writer_id);
        }
        [WebMethod(Description = "增加一篇部落格noTitle")]
        public String insertBlogWithoutTitle(string writer_id)
        {
            return dbOperation.insertBlogWithoutTitle(writer_id);
        }
        [WebMethod(Description = "通过ID删除用户账户")]
        public Boolean deleteUserById(string user_id)
        {
            if (dbOperation.deleteByUserId(user_id)) return true;
            else return false;
        }
        [WebMethod(Description = "插入图片")]
        public Boolean insertAnImg(int blog_id, int index, string imgs)
        {
            byte[] buffer = Convert.FromBase64String(imgs);
            return dbOperation.insertImg(blog_id, index, buffer);
        }
        [WebMethod(Description = "插入文字")]
        public Boolean insertText(int blog_id, int pos, string content, int flag)
        {
            return dbOperation.insertText(blog_id, pos, content, flag);
        }
        [WebMethod(Description = "插入题目")]
        public Boolean insertTitle(string user_id, int blog_id, string title)
        {
            List<ParaModel> list = dbOperation.getContentTextByBlogId(blog_id);
            directoryManager.createHTML(user_id, blog_id, list);
            dbOperation.insertURL(user_id, blog_id);
            return dbOperation.insertTitle(blog_id, title);
        }
        [WebMethod(Description = "创建文件夹")]
        public Boolean createDirectoryOfBlog(string user_id, string blog_id)
        {
            return directoryManager.createDirectoryOfBlog(user_id, blog_id);
        }
        [WebMethod(Description = "保存图片")]
        public Boolean saveImage(string user_id, string blog_id, string filename, string base64string)
        {
            return directoryManager.saveImageFile(user_id, blog_id, filename, base64string);
        }
        [WebMethod(Description = "得到Blog的URL")]
        public string getBlogURLByID(string blog_id) {
            return dbOperation.getBlogURL(blog_id);
        }
        [WebMethod(Description = "得到许多博客")]
        public BlogModel[] getManyBlogs() {
            return dbOperation.getListBlogs().ToArray();
        }
        [WebMethod(Description = "Add Likes")]
        public bool addLikes(string blog_id, string user_id)
        {
            return dbOperation.addLikes(blog_id) && dbOperation.addmylikeblog(blog_id, user_id);
        }
        [WebMethod(Description = "Minus Likes")]
        public bool minusLikes(string blog_id, string user_id)
        {
            return dbOperation.minusLikes(blog_id) && dbOperation.deletemylikes(blog_id, user_id);
        }
        [WebMethod(Description = "Get Likes Count")]
        public string getLikesCount(string blog_id)
        {
            return dbOperation.getLikesCount(blog_id);
        }
        [WebMethod(Description = "得到Url&&Count")]
        public string[] getUrlAndLikes(string blog_id)
        {
            List<string> list = new List<string>();
            list.Add(dbOperation.getBlogURL(blog_id));
            list.Add(dbOperation.getTitle(blog_id));
            list.Add(dbOperation.getBlogAuthor(blog_id));
            list.Add(dbOperation.getLikesCount(blog_id));
            return list.ToArray();
        }
        [WebMethod(Description = "得到Title")]
        public string getTitle(string blog_id) {
            return dbOperation.getTitle(blog_id);
        }
        [WebMethod(Description = "得到我喜欢的博客立标")]
        public BlogModel[] getMyLikesBlogs(string user_id)
        {
            return dbOperation.getMyLikeBlogs(user_id).ToArray();
        }
        [WebMethod(Description = "Add Focus")]
        public bool addAFocus(string user_id, string focused_id) {
            return dbOperation.addFocus(user_id, focused_id);
        }
        [WebMethod(Description = "Delete Focus")]
        public bool deleteAFocus(string user_id, string focused_id) {
            return dbOperation.deleteFocus(user_id, focused_id);
        }
        [WebMethod(Description = "得到所有我喜欢的人的ID")]
        public string[] getMyFocuses(string user_id) {
            return dbOperation.getMyFocuses(user_id).ToArray();
        }
        [WebMethod(Description = "插入头像Avatar")]
        public bool insertAvatarById(string user_id, string base64string,string file_name) {
            /// byte[] buffer = Convert.FromBase64String(base64string);
            string subfolder = "avatar";
            return dbOperation.updateAvater(user_id, file_name)&&directoryManager.saveImageFile(user_id,subfolder,file_name,base64string);
        }
        [WebMethod(Description = "得到头像Avatar的Base64String")]
        public string getAvatarById(string user_id) {
            return dbOperation.selectBase64String(user_id);         
        }
        [WebMethod(Description = "判断博文是否被某个用户喜欢")]
        public bool getIsLiked(string blog_id, string user_id) {
            return dbOperation.isTheBlogLiked(blog_id, user_id);
        }
    }
}