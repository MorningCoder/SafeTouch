using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;

namespace PhoneSafe
{
    //系统通知类
    class Notice
    {
        //发送者
        public string Sender { get;set; }
        //通知内容
        public string Content { get; set; }
        //静态通知列表
        public static List<Notice> NoticeList = new List<Notice>();


        public static async Task<string> GetNotice(NetWork net, string username)
        {
            //执行页面请求
            string result = await net.GetNoticeList(username);
            //处理字符串
            result = result.Trim(new char[] { '[', ']' });
            NoticeList.Clear();
            if (result == "")
                return "";
            if (result[0] != '{')
                return "获取失败！请检查网络设置";
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
                        NoticeList.Add(new Notice()
                        {
                            Sender = (string)jd["sender"],
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
            else
            {
                JsonData jd = JsonMapper.ToObject(result);
                //判断是否存在message字段
                if ((string)jd["message"] == string.Empty)
                {
                    NoticeList.Add(new Notice()
                    {
                        Sender = (string)jd["sender"],
                        Content = (string)jd["content"]
                    });
                    return string.Empty;
                }
                else
                    return (string)jd["message"];
            }
        }
    }
}
