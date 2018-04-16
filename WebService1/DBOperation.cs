using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebService1
{
    public class DBOperation
    {
        public static MySqlConnection sqlCon;  //用于连接数据库  

        //将下面的引号之间的内容换成上面记录下的属性中的连接字符串  
        private String ConServerStr = @"server=47.100.46.16;user id=piggy;password=888666;Port=3306;database=test;";

        //默认构造函数  
        public DBOperation()
        {
            if (sqlCon == null)
            {
                sqlCon = new MySqlConnection();
                sqlCon.ConnectionString = ConServerStr;
                sqlCon.Open();
            }
        }

        //关闭/销毁函数，相当于Close()  
        public void Dispose()
        {
            if (sqlCon != null)
            {
                sqlCon.Close();
                sqlCon = null;
            }
        }
        //jin yong yu ce shi
        public List<UserModel> selectAllUser()
        {
            List<UserModel> list = new List<UserModel>();
            string sql = "select * from user_info";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                MySqlDataReader reader = cmd.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    list.Add(new UserModel());
                    list.ElementAt(i).user_id = reader[0].ToString();
                    list.ElementAt(i).password = reader[1].ToString();
                    list[i].nickname = reader[2].ToString();
                    list[i].sex = reader[3].ToString();
                    i++;
                }
                reader.Close();
                cmd.Dispose();
            }
            catch (Exception)
            {

            }
            return list;

        }
        public List<BlogModel> selectBlogListByUserId(string user_id)
        {
            List<BlogModel> list = new List<BlogModel>();
            string sql = "select * from blogs where writer_id=?writer_id";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?writer_id", MySqlDbType.String)).Value = user_id;
                MySqlDataReader reader = cmd.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    list.Add(new BlogModel());
                    list.ElementAt(i).blog_id = reader[0].ToString();
                    list.ElementAt(i).title = reader[1].ToString();
                    list[i].Writer_id = reader[2].ToString();
                    list[i].Create_time = reader[3].ToString();
                    i++;
                }
                reader.Close();
                cmd.Dispose();
            }
            catch (Exception)
            {

            }
            return list;

        }
        public UserModel selectUserById(String id)
        {
            UserModel um=new UserModel();
            string sql = "select * from user_info where user_id=?id;";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?id", MySqlDbType.String)).Value = id;
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    um.user_id = reader[0].ToString();
                    um.password = reader[1].ToString();
                    um.nickname = reader[2].ToString();
                    um.sex = reader[3].ToString();
                }
                reader.Close();
                cmd.Dispose();
            }
            catch (Exception)
            {

            }
            return um;

        }
        public bool insertUser(String user_id, String password, String nickname,String phone,String email)
        {
            try
            {

                string sql = "INSERT INTO test.user_info(user_id,password,user_name,phone,email) VALUES(?user_id, ?password,?user_name,?phone,?email);";
                
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?user_id", MySqlDbType.String)).Value = user_id;
                cmd.Parameters.Add(new MySqlParameter("?password", MySqlDbType.String)).Value = password;
                cmd.Parameters.Add(new MySqlParameter("?user_name", MySqlDbType.String)).Value = nickname;
                cmd.Parameters.Add(new MySqlParameter("?phone", MySqlDbType.String)).Value = phone;
                cmd.Parameters.Add(new MySqlParameter("?email", MySqlDbType.String)).Value = email;



                cmd.ExecuteNonQuery();
                cmd.Dispose();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public String insertBlog(String title, String writer_id)
        {
            try
            {
                string sql = "insert into blogs(title,writer_id,create_time) values(" + "'" + title + "','" + writer_id + "',"
                    +"now()" + ");select LAST_INSERT_ID();";
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                //cmd.ExecuteNonQuery();
                MySqlDataReader reader = cmd.ExecuteReader();
                String rs = null;
                while (reader.Read())
                {
                    rs = reader[0].ToString();
                }
                reader.Close();
                cmd.Dispose();

                return rs;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public String insertBlogWithoutTitle(String writer_id)
        {
            try
            {
                string sql = "insert into blogs(writer_id,create_time) values(" + "'" + writer_id + "',"
                    + "now()" + ");select LAST_INSERT_ID();";
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                //cmd.ExecuteNonQuery();
                MySqlDataReader reader = cmd.ExecuteReader();
                String rs = null;
                while (reader.Read())
                {
                    rs = reader[0].ToString();
                }
                reader.Close();
                cmd.Dispose();

                return rs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool deleteByUserId(String user_id)
        {
            try
            {
                string sql = "delete from user_info where user_id='"+user_id+"';";
                    
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        ///insert pic
        public bool insertImg(int blog_id,int index,byte[] fileBytes)
        {
            

                string insertStr = "INSERT INTO blog_content2(blog_id,pos,content,tags) VALUES (?blog_id,?index,?imageByte,?tags);";
                MySqlCommand comm = new MySqlCommand(insertStr, sqlCon);

                //设置数据库字段类型MediumBlob的值为图片字节数组imageByte 
                comm.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.Int32)).Value = blog_id;
                comm.Parameters.Add(new MySqlParameter("?index", MySqlDbType.Int32)).Value = index;
                comm.Parameters.Add(new MySqlParameter("?imageByte", MySqlDbType.LongBlob)).Value = fileBytes;
                comm.Parameters.Add(new MySqlParameter("?tags", MySqlDbType.Int32)).Value = 1;
                //执行命令 
                try
                {
                    comm.ExecuteNonQuery();
                    comm.Dispose();
                    return true;
                }
                catch (Exception ex)
                {

                }
            
                return false;
        }
        public bool insertText(int blog_id,int pos,string content,int flag)
        {
            string insertStr = "INSERT INTO blog_content1 VALUES (?blog_id,?index,?content,?tags);";
            MySqlCommand comm = new MySqlCommand(insertStr, sqlCon);

            //设置数据库字段类型MediumBlob的值为图片字节数组imageByte 
            comm.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.Int32)).Value = blog_id;
            comm.Parameters.Add(new MySqlParameter("?index", MySqlDbType.Int32)).Value = pos;
            comm.Parameters.Add(new MySqlParameter("?content", MySqlDbType.Text)).Value = content;
            comm.Parameters.Add(new MySqlParameter("?tags", MySqlDbType.Int32)).Value = flag;
            //执行命令 
            try
            {
                comm.ExecuteNonQuery();
                comm.Dispose();
                return true;
            }
            catch (Exception ex)
            {

            }

            return false;
        }
        public bool insertTxt(int id,string txt)
        {
            string updatestr = "insert into test values(?id,?txt);";
            MySqlCommand comm = new MySqlCommand(updatestr, sqlCon);

            //设置数据库字段类型MediumBlob的值为图片字节数组imageByte 
            comm.Parameters.Add(new MySqlParameter("?id", MySqlDbType.Int32)).Value = id;
            comm.Parameters.Add(new MySqlParameter("?txt", MySqlDbType.String)).Value = txt;
            //执行命令 
            try
            {
                comm.ExecuteNonQuery();
                comm.Dispose();
                return true;
            }
            catch (Exception ex)
            {

            }

            return false;

        }
        public List<String> selectTest()
        {
            List<String> list = new List<String>();
            string sql = "select txt from test;";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
               // cmd.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.Int32)).Value = blog_id;
                MySqlDataReader reader = cmd.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    list.Add(reader[0].ToString());
                    i++;
                }
                reader.Close();
                cmd.Dispose();
            }
            catch (Exception)
            {

            }
            return list;

        }
        public bool insertTitle(int blog_id,string title)
        {
            string updatestr = "update blogs set title=?title where blog_id=?blog_id;";
            MySqlCommand comm = new MySqlCommand(updatestr, sqlCon);

            //设置数据库字段类型MediumBlob的值为图片字节数组imageByte 
            comm.Parameters.Add(new MySqlParameter("?title", MySqlDbType.String)).Value = title;
            comm.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.Int32)).Value = blog_id;
            //执行命令 
            try
            {
                comm.ExecuteNonQuery();
                comm.Dispose();
                return true;
            }
            catch (Exception ex)
            {

            }

            return false;
        }
        public List<ParaModel> getContentTextByBlogId(int blog_id)
        {
            List<ParaModel> list = new List<ParaModel>();
            string sql = "select pos,content,tag from blog_content1 where blog_id=?blog_id";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.Int32)).Value =blog_id;
                MySqlDataReader reader = cmd.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    list.Add(new ParaModel());
                    list.ElementAt(i).pos = reader[0].ToString();
                    list.ElementAt(i).content = reader[1].ToString();
                    list[i].tag = reader[2].ToString();
                    i++;
                }
                reader.Close();
                cmd.Dispose();
            }
            catch (Exception)
            {

            }
            return list;
        }
        public List<String> getImgPosByBlogId(int blog_id)
        {
            List<String> list = new List<String>();
            string sql = "select pos from blog_content2 where blog_id=?blog_id";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.Int32)).Value = blog_id;
                MySqlDataReader reader = cmd.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    list.Add(reader[0].ToString());
                    
                }
                reader.Close();
                cmd.Dispose();
            }
            catch (Exception)
            {

            }
            return list;
        }
        public List<String> getImgByPosAndBlogId(int blog_id)
        {
            List<String> list = new List<String>();
            String[] a = getImgPosByBlogId(blog_id).ToArray();
            string sql = "select content from blog_content2 where  blog_id=?blog_id and pos=?pos";
            for(int i = 0; i < a.Length; i++)
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.Int32)).Value = blog_id;
                cmd.Parameters.Add(new MySqlParameter("?pos", MySqlDbType.Int32)).Value =Int32.Parse(a[i]);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    byte[] imageByte = new byte[dr.GetBytes(0, 0, null, 0, int.MaxValue)];
                    dr.GetBytes(0, 0, imageByte, 0, imageByte.Length);
                    //将图片字节数组加载入缓冲流 
                    MemoryStream imageStream = new MemoryStream(imageByte);
                    byte[] bytes = new byte[imageStream.Length];

                    imageStream.Position = 0;
                    imageStream.Read(bytes, 0, (int)imageStream.Length);
                    imageStream.Close();
                    String result = Convert.ToBase64String(bytes);
                    list.Add(result);
                }
                dr.Close();
                cmd.Dispose();
            }
            return list;
        }
        public bool insertURL(string user_id, int blog_id) {
            string ToBeInserted = "http://wz66.top:86/UserDirectories/" + user_id + "/" + blog_id + "/";
            string sql = "update blogs set blog_url = ?url WHERE blog_id=?blog_id;";
            MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
            cmd.Parameters.Add(new MySqlParameter("?url", MySqlDbType.String)).Value = ToBeInserted;
            cmd.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.Int32)).Value = blog_id;
            try
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;
            }
            catch (Exception ex)
            {

            }

            return false;

        }
        public string getBlogURL(string blog_id) {
            string sql = "SELECT blog_url FROM test.blogs where blog_id=?blog_id";
            string result = "";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.String)).Value =blog_id ;
                MySqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    result = reader[0].ToString();
                    
                }
                reader.Close();
                cmd.Dispose();
            }
            catch (Exception)
            {

            }
            return result ;


        }
        public string getBlogAuthor(string blog_id)
        {
            string sql = "SELECT writer_id FROM test.blogs where blog_id=?blog_id";
            string result = "";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.String)).Value = blog_id;
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    result = reader[0].ToString();

                }
                reader.Close();
                cmd.Dispose();
            }
            catch (Exception)
            {

            }
            return result;


        }

        public List<BlogModel> getListBlogs() {
            List<BlogModel> list = new List<BlogModel>();
            string sql = "SELECT test.blogs.blog_id,title,create_time,user_name,likes ,test.blog_content1.content FROM (test.blogs left join(test.user_info) on(test.blogs.writer_id = test.user_info.user_id)) left join(test.blog_content1) on(test.blogs.blog_id = test.blog_content1.blog_id) where title<> '' and test.blog_content1.pos = 1 limit 5; ";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                MySqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    BlogModel bm = new BlogModel();
                    bm.blog_id = reader[0].ToString();
                    bm.title = reader[1].ToString();
                    bm.Create_time = reader[2].ToString();
                    bm.Writer_name = reader[3].ToString();
                    bm.likes = reader[4].ToString();
                    bm.paragraph = reader[5].ToString();
                    /*byte
                     * [] imageByte = new byte[reader[4](0, 0, null, 0, int.MaxValue)];
                    dr.GetBytes(0, 0, imageByte, 0, imageByte.Length);
                    //将图片字节数组加载入缓冲流 
                    MemoryStream imageStream = new MemoryStream(imageByte);
                    byte[] bytes = new byte[imageStream.Length];

                    imageStream.Position = 0;
                    imageStream.Read(bytes, 0, (int)imageStream.Length);
                    imageStream.Close();*/
   
                    list.Add(bm);

                }
                reader.Close();
                cmd.Dispose();
            }
            catch (Exception)
            {

            }
             return list;
        }

        public List<BlogModel> getMyLikeBlogs(string user_id) {
            List<BlogModel> list = new List<BlogModel>();
            string sql = "SELECT test.blogs.blog_id,title,create_time,writer_id FROM test.blogs left join(test.my_likes) on (test.blogs.blog_id = test.my_likes.blog_id) where user_id=?user_id;";
            try {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?user_id", MySqlDbType.String)).Value = user_id;

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    BlogModel bm = new BlogModel();
                    bm.blog_id = reader[0].ToString();
                    bm.title = reader[1].ToString();
                    bm.Create_time = reader[2].ToString();
                    bm.Writer_name = reader[3].ToString();
                    list.Add(bm);

                }
                reader.Close();
                cmd.Dispose();

            } catch (Exception e) { }
            return list;
        }

        public bool addLikes(string blog_id)
        {
            string sql = "UPDATE test.blogs SET likes = likes+1 WHERE blog_id = ?blog_id;";
            try {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.String)).Value = blog_id;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;

            }catch(Exception e)
            {
                return false;
            }
        }
        public bool addFocus(string user_id, string focused_id) {
            string sql = "INSERT INTO test.my_focus VALUES(?user_id,?focused_id); ";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?user_id", MySqlDbType.String)).Value = user_id;
                cmd.Parameters.Add(new MySqlParameter("?focused_id", MySqlDbType.String)).Value = focused_id;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool deleteFocus(string user_id, string focused_id)
        {
            string sql = "DELETE FROM test.my_focus WHERE user_id=?user_id AND focused_id=?focused_id; ";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?user_id", MySqlDbType.String)).Value = user_id;
                cmd.Parameters.Add(new MySqlParameter("?focused_id", MySqlDbType.String)).Value = focused_id;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool minusLikes(String blog_id) {
            string sql = "UPDATE test.blogs SET likes = likes-1 WHERE blog_id = ?blog_id;";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.String)).Value = blog_id;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool addmylikeblog(string blog_id,string user_id)
        {
            string sql = "INSERT INTO test.my_likes VALUES (?blog_id,?user_id);";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.String)).Value = blog_id;
                cmd.Parameters.Add(new MySqlParameter("?user_id", MySqlDbType.String)).Value = user_id;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool deletemylikes(string blog_id, string user_id) {
            string sql = "DELETE FROM test.my_likes WHERE blog_id=?blog_id AND user_id=?user_id;";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.String)).Value = blog_id;
                cmd.Parameters.Add(new MySqlParameter("?user_id", MySqlDbType.String)).Value = user_id;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }
        public string getLikesCount(string blog_id)
        {
            try
            {
                string sql = "SELECT likes FROM test.blogs where blog_id = ?blog_id;";
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.String)).Value = blog_id;
                //cmd.ExecuteNonQuery();
                MySqlDataReader reader = cmd.ExecuteReader();
                String rs = null;
                while (reader.Read())
                {
                    rs = reader[0].ToString();
                }
                reader.Close();
                cmd.Dispose();

                return rs;
            }catch(Exception e)
            {
                return null;
            }
        }
        public string getTitle(string blog_id)
        {
            try
            {
                string sql = "SELECT title FROM test.blogs where blog_id = ?blog_id;";
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.String)).Value = blog_id;
                //cmd.ExecuteNonQuery();
                MySqlDataReader reader = cmd.ExecuteReader();
                String rs = null;
                while (reader.Read())
                {
                    rs = reader[0].ToString();
                }
                reader.Close();
                cmd.Dispose();

                return rs;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public bool isTheBlogLiked(string blog_id,string user_id) {
            string sql = "SELECT COUNT(*) FROM test.my_likes WHERE blog_id=?blog_id AND user_id=?user_id;";
            int flag=0;
            try {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?blog_id", MySqlDbType.String)).Value = blog_id;
                cmd.Parameters.Add(new MySqlParameter("?user_id", MySqlDbType.String)).Value = user_id;
                MySqlDataReader reader = cmd.ExecuteReader();
                String rs = null;
                while (reader.Read())
                {
                    rs = reader[0].ToString();
                    flag = Int32.Parse(rs); 
                }
                reader.Close();
                cmd.Dispose();
            } catch (Exception e) {
                return false;
            }
            return flag > 0;
        }
        public List<string> getMyFocuses(string user_id) {
            string sql = "SELECT user_name,phone,email FROM test.user_info left join(test.my_focus) on (test.my_focus.focused_id=test.user_info.user_id) where test.my_focus.user_id=?user_id;";
            List<string> list = new List<string>();
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?user_id", MySqlDbType.String)).Value = user_id;
                //cmd.ExecuteNonQuery();
                MySqlDataReader reader = cmd.ExecuteReader();
                String rs = null;
                while (reader.Read())
                {
                    rs = reader[0].ToString();
                    list.Add(rs);
                }
                reader.Close();
                cmd.Dispose();
            }
            catch (Exception e) {
                return null;
            }
            return list;
        }
        public bool updateAvater(string user_id,string file_name) {
            string ToBeInserted = "http://wz66.top:86/UserDirectories/" + user_id + "/" + "avatar" + "/" + file_name;
            string sql = "UPDATE test.user_info SET avatar_url=?avatar_url WHERE user_id =?user_id;";
            bool isSuccess = true;
            MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
            cmd.Parameters.Add(new MySqlParameter("?user_id", MySqlDbType.String)).Value = user_id;
            cmd.Parameters.Add(new MySqlParameter("?avatar_url", MySqlDbType.String)).Value = ToBeInserted;
            try
            {
                cmd.ExecuteNonQuery();
            } catch (Exception e) {
                isSuccess = false;
            }

            cmd.Dispose();
            return isSuccess;
        }
        public string selectBase64String(string user_id)
        {
            string sql = "SELECT avatar FROM test.user_info WHERE user_id =?user_id; ";
            String result = null;
            MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
            cmd.Parameters.Add(new MySqlParameter("?user_id", MySqlDbType.String)).Value = user_id;
            MySqlDataReader dr;
            try
            {
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = dr[0].ToString();
                }
                dr.Close();
            }
            catch (Exception e)
            {

            }

            cmd.Dispose();
            return result;
        }



        public int hun_login(string userName, string psd)
        {
            string sql = "SELECT identity FROM test.hunter_user "
               + "WHERE user_name =?user_name "
               + "AND password =?psd; ";

            int result = 0;
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?user_name", MySqlDbType.String)).Value = userName;
                cmd.Parameters.Add(new MySqlParameter("?psd", MySqlDbType.String)).Value = psd;
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result = reader.GetInt16(0);
                }
                reader.Close();
                cmd.Dispose();
            }
            catch (Exception e) { }
            return result;
        }


        public string hun_register(string userName, string psd, long phone)
        {
            string sql = "insert test.hunter_user(user_name,password,phone) " +
                "values(?userName, ?psd, ?phone);";

            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, sqlCon);
                cmd.Parameters.Add(new MySqlParameter("?userName", MySqlDbType.String)).Value = userName;
                cmd.Parameters.Add(new MySqlParameter("?psd", MySqlDbType.String)).Value = psd;
                cmd.Parameters.Add(new MySqlParameter("?phone", MySqlDbType.Int64)).Value = phone;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return "注册成功";

            }
            catch (Exception e)
            {
                return "用户名已存在";
            }
        }

    
}
   
}