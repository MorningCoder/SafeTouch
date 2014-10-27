using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Web.Script.Serialization;
using System.Threading;

namespace PhoneSafe
{
     /// <summary>
    /// 用于网络连接的类
    /// 使用了单例模式
    /// </summary>
    class NetWork
    {
        /// <summary>
        /// 下载图片存储地址
        /// </summary>
        const string picturepath = "F:\\DownloadPicture\\";
        /// <summary>
        /// 服务器根目录
        /// </summary>
        const string server = "http://1.remoteassist.vipsinaapp.com/";
        /// <summary>
        /// 当前时间的特定字符串表示
        /// </summary>
        string time;
        public string Time
        {
            get { return time; }
            set { time = value; }
        }
        //单例对象
        private static NetWork network;
        //记录是否被创建
        private static bool created = false;
        //私有构造函数
        private NetWork()
        {
            this.Time = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");
            network = this;
            created = true;
        }

        //全局访问点，获取对象实例
        public static NetWork Create()
        {
            if (!created)
                network = new NetWork();
            return network;
        }

        /// <summary>
        /// 异步发送只包含文本内容的POST请求并返回结果json
        /// </summary>
        /// <param name="url">请求的地址</param>
        /// <param name="content">需要发送的数据</param>
        /// <returns>响应结果</returns>
        private async Task<string> HttpPostText(string url, string content)
        {
            //构造http请求对象
            HttpWebRequest http = WebRequest.Create(url) as HttpWebRequest;
            //将待发送内容编码为utf-8
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            //设置http报头参数
            http.ContentLength = bytes.Length;
            http.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)";
            http.ContentType = "application/x-www-form-urlencoded";
            http.Method = "post";
            //获取返回的请求流
            try
            {
                using (Stream rs = await http.GetRequestStreamAsync())
                {
                    //将内容写入流
                    await rs.WriteAsync(bytes, 0, bytes.Length);
                    //获取响应对象
                    HttpWebResponse sr = await http.GetResponseAsync() as HttpWebResponse;
                    //获取响应流
                    using (Stream stream = sr.GetResponseStream())
                    {
                        //读取响应流内容
                        StreamReader streamreader = new StreamReader(stream);
                        return await streamreader.ReadToEndAsync();
                    }
                }
            }
            catch(Exception ee)
            {
                return "{\"result\":\"-1\",\"message\":\"网络连接错误，请检查网络设置\"}";
            }
        }

        /// <summary>
        /// 负责处理登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>登录结果json字符串</returns>
        public async Task<string> Login(string username,string password)
        {
            //首先加密密码
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] res = md5.ComputeHash(Encoding.Default.GetBytes(password), 0, password.Length);
            string str = "";
            for (int i = 0; i < res.Length; i++)
            {
                str += res[i].ToString("x").PadLeft(2, '0');
            }
            //构造post报文内容
            string content = string.Format("username={0}&password={1}",username,str);
            string url = "pc_check.php";
            //获取登录结果json字符串
            return await HttpPostText(server + url, content);

        }

        /// <summary>
        /// 用于发送验证码
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<string> Confirm(string username)
        {
            string content = "username=" + username;
            string url = "add_confirm_msg.php";
            return await HttpPostText(server + url, content);
        }

        /// <summary>
        /// 注册方法
        /// </summary>
        /// <param name="username">新用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<string> Register(string username, string password,string confirm)
        {
            //首先加密密码
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] res = md5.ComputeHash(Encoding.Default.GetBytes(password), 0, password.Length);
            string str = "";
            for (int i = 0; i < res.Length; i++)
            {
                str += res[i].ToString("x").PadLeft(2, '0');
            }
            //发送注册请求
            string content = string.Format("username={0}&password={1}&confirm={2}", username, str,confirm);
            string url = "register.php";
            return await HttpPostText(server + url, content);
        }
        
        /// <summary>
        /// 处理登出
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        public async Task<string> Signout(string username)
        {
            //构造报文内容
            string content = string.Format("username={0}", username);
            string url = "phone_signout.php";
            //执行请求
            return await HttpPostText(server + url, content);
        }

        /// <summary>
        /// 用于上传PC端操作指令
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="content">指令内容（枚举）</param>
        /// <returns>操作结果json</returns>
        public async Task<string> UploadInstructor(string username, string content)
        {
            //构造报文内容
            string con = string.Format("username={0}&content={1}", username, content);
            string url = "update_instructor.php";
            //执行请求
            return await HttpPostText(server + url, con);
        }

        /// <summary>
        /// 用于上传待发送短信数据
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="receiver_num">收信人号码</param>
        /// <param name="content">短信内容</param>
        /// <returns></returns>
        public async Task<string> UploadSendingMsg(string username, string receiver_num, string content)
        {
            //构造报文内容
            string con = string.Format("username={0}&content={1}&receiver_num={2}", username, content, content);
            string url = "update_sending_msg.php";
            //执行请求
            return await HttpPostText(server + url, con);
        }

        /// <summary>
        /// 获取用户所有短信列表
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>json字符串表示结果</returns>
        public async Task<string> GetMessageList(string username)
        {
            //构造url以及post内容
            string url = "get_message.php";
            string con = "username=" + username;
            //开始请求
            return await HttpPostText(server + url, con);
        }

        /// <summary>
        /// 获取联系人列表
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<string> GetContacterList(string username)
        {
            //构造url以及post内容
            string url = "get_contacter.php";
            string con = "username=" + username;
            //开始请求
            return await HttpPostText(server + url, con);
        }

        /// <summary>
        /// 获取系统通知
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<string> GetNoticeList(string username)
        {
            string url = "get_notice.php";
            string con = "username=" + username;
            return await HttpPostText(server + url, con);
        }

        public async Task<string> GetPictureList(string username)
        {
            string url = "get_picture.php";
            string con = "username=" + username;
            return await HttpPostText(server + url, con);
        }

        /// <summary>
        /// 读取手机基本信息
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<string> GetPhoneInfo(string username)
        {
            //构造url以及post内容
            string url = "get_phoneinfo.php";
            string con = "username=" + username;
            //开始请求
            return await HttpPostText(server + url, con);
        }
        /// <summary>
        /// 下载用户照片
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="picture_name">图片名</param>
        /// <returns></returns>
        public async Task<string> DownloadPicture(string username, string picture_name)
        {
            string url = string.Format("download_picture.php?username={0}&picture_name={1}", username, picture_name);
            WebClient client = new WebClient();
            //检测下载目录是否存在，不存在则创建
            if (!Directory.Exists(picturepath))
            {
                Directory.CreateDirectory(picturepath);
            }
            //执行下载任务
            try
            {
                await client.DownloadFileTaskAsync(new Uri(server + url), picturepath + picture_name);
                return "下载成功";
            }
            catch (Exception ee)
            {
                return "下载失败！" + ee.Message;
            }
        }

        /////////////////////////以下为测试方法，实际PC端无需使用/////////////////////////////////////////////////
        /// <summary>
        /// 上传用户图片
        /// </summary>
        /// <param name="username"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public string UploadPicture(string username, DateTime time, string filename)
        {
            //页面url
            string url = "update_picture.php";
            //图片拍摄时间字符串
            string uploadtime = time.ToString("yy-MM-dd hh:mm:ss");
            //http报文体边界字符串
            string boundary = "--fuck---" + DateTime.Now.Ticks.ToString("x");
            //设置请求参数
            WebRequest req = WebRequest.Create(server + url);
            req.Method = "POST";
            req.ContentType = "multipart/form-data; boundary=" + boundary;
            //模拟http请求报文体数据
            StringBuilder sb = new StringBuilder();
            sb.Append("--" + boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"username\"");
            sb.Append("\r\n\r\n");
            sb.Append(username);
            sb.Append("\r\n");
            sb.Append("--" + boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"time\"");
            sb.Append("\r\n\r\n");
            sb.Append(uploadtime);
            sb.Append("\r\n");
            sb.Append("--" + boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"picture\"; filename=\"" + filename + "\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: image/pjpeg");
            sb.Append("\r\n\r\n");
            string head = sb.ToString();
            byte[] form_data = Encoding.UTF8.GetBytes(head);
            //报文结尾
            byte[] foot_data = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            //上传的文件流
            FileStream fileStream = new FileStream(picturepath + filename, FileMode.Open, FileAccess.Read);
            //报文总长度
            long length = form_data.Length + fileStream.Length + foot_data.Length;
            req.ContentLength = length;
            //获取请求流
            Stream requestStream = req.GetRequestStream();
            //发送表单参数
            requestStream.Write(form_data, 0, form_data.Length);
            //发送文件内容
            byte[] buffer = new Byte[checked((uint)Math.Min(4096, (int)fileStream.Length))];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                requestStream.Write(buffer, 0, bytesRead);
            //发送结尾
            requestStream.Write(foot_data, 0, foot_data.Length);
            requestStream.Close();
            //获取响应流
            WebResponse pos = req.GetResponse();
            StreamReader sr = new StreamReader(pos.GetResponseStream(), Encoding.UTF8);
            //获取操作结果
            string html = sr.ReadToEnd().Trim();
            sr.Close();
            if (pos != null)
            {
                pos.Close();
                pos = null;
            }
            if (req != null)
            {
                req = null;
            }
            return html;
        }
    }
}
