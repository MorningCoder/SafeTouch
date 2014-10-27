using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;

namespace PhoneSafe
{
    /// <summary>
    /// 联系人类
    /// </summary>
    class Contacter
    {
        /// <summary>
        /// 联系人名字
        /// </summary>
        public string ContactorName { get; set; }
        /// <summary>
        /// 联系人号码
        /// </summary>
        public string ContactorNum { get; set; }
        /// <summary>
        /// 联系人归属地
        /// </summary>
        public string ContactorLocation { get; set; }
        /// <summary>
        /// 静态联系人列表
        /// </summary>
        public static List<Contacter> ContacterList = new List<Contacter>();

        /// <summary>
        /// 获取联系人列表
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetContacterList(string username, NetWork net)
        {
            //执行页面请求
            string result = await net.GetContacterList(username);
            //处理字符串
            result = result.Trim(new char[] { '[', ']' });
            ContacterList.Clear();
            if (result == "")
                return "短信列表为空！";
            if (result[0] != '{')
                return "获取失败！请检查网络设置";
            //判断是否为错误信息，false为错误信息，true为正常数据
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
                        ContacterList.Add(new Contacter()
                        {
                            ContactorName = (string)jd["contacter_name"],
                            ContactorNum = (string)jd["contacter_num"],
                            ContactorLocation = (string)jd["contacter_location"]
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
            //如果是错误信息
            else
            {
                JsonData jd = JsonMapper.ToObject(result);
                //判断是否存在message字段
                if ((string)jd["message"] == string.Empty)
                {
                    ContacterList.Add(new Contacter()
                    {
                        ContactorName = (string)jd["contacter_name"],
                        ContactorNum = (string)jd["contacter_num"],
                        ContactorLocation = (string)jd["contacter_location"]
                    });
                    return string.Empty;
                }
                else
                    return "获取失败！" + (string)jd["message"];
            }
        }

        public override string ToString()
        {
            return string.Format("联系人姓名：{0}\n联系人电话：{1}\n联系人归属地：{2}",this.ContactorName,this.ContactorNum,this.ContactorLocation);
        }
    }
}
