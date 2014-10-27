using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using LitJson;

namespace PhoneSafe
{
    /// <summary>
    /// 表示一条短信
    /// </summary>
    class Message
    {
        /// <summary>
        /// 发送者号码
        /// </summary>
        public string SenderNum { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        public string Content {get;set;}
        /// <summary>
        /// 接收时间
        /// </summary>
        public string ReceiveTime {get;set;}
        /// <summary>
        /// 静态短信列表
        /// </summary>
        public static List<Message> MessageList = new List<Message>();

        /// <summary>
        /// 用于从服务器读取短信数据
        /// </summary>
        public static async Task<string> GetMessage(NetWork net, string username)
        {
            //执行页面请求
            string result = await net.GetMessageList(username);
            //处理字符串
            result = result.Trim(new char[] { '[', ']' });
            MessageList.Clear();
            if (result == "")
                return "用户短信列表为空！";
            if(result[0] != '{')
                return "获取失败！请检查网络设置";
            //false为错误信息或者只有一条短信，true为正常数据
            bool flag = false;
            int count = 0;
            foreach (char c in result)
            {
                if (c == '{')
                    count++;
                if (count >= 2)
                {
                    flag = true;
                    break;
                }
            }
            //如果为正常数据
            if (flag)
            {
                JsonData jd = null;
                //用于记录临时字符串
                string temp = string.Empty;
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i] == '}')
                    {
                        temp += result[i];
                        jd = JsonMapper.ToObject(temp);
                        MessageList.Add(new Message()
                        {
                            SenderNum = (string)jd["sender_number"],
                            ReceiveTime = (string)jd["receive_time"],
                            Content = (string)jd["content"]
                        });
                        //跳过逗号
                        i++;
                        temp = string.Empty;
                        continue;
                    }
                    temp += result[i];
                }
                return string.Empty;
            }
            //如果是错误信息或者只有一条短信
            else
            {
                JsonData jd = JsonMapper.ToObject(result);
                //判断是否存在message字段
                if((string)jd["message"] == string.Empty)
                {
                    MessageList.Add(new Message()
                    {
                        SenderNum = (string)jd["sender_number"],
                        ReceiveTime = (string)jd["receive_time"],
                        Content = (string)jd["content"]
                    });
                    return string.Empty;
                }
                else
                    return "获取失败！" + (string)jd["message"];
            }
        }

        public override string ToString()
        {
            return string.Format("发信人：{0}\n内容：{1}\n接收时间：{2}", this.SenderNum, this.Content, this.ReceiveTime);
        }
    }
}
