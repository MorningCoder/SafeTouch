using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LitJson;

namespace PhoneSafe
{
    //记录截图信息
    class Picture
    {
        //图片名
        public string PictureName { get; set; }
        //拍照时间
        public string Time { get; set; }
        //静态短信列表
        public static List<Picture> PictureList = new List<Picture>();

        /// <summary>
        /// 读取图片列表
        /// </summary>
        /// <param name="net"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static async Task<string> GetPictureList(NetWork net, string username)
        {
            //执行页面请求
            string result = await net.GetPictureList(username);
            //处理字符串
            result = result.Trim(new char[] { '[', ']' });
            PictureList.Clear();
            if (result == "")
                return "图片列表为空！";
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
                        PictureList.Add(new Picture()
                        {
                            PictureName = (string)jd["picture_name"],
                            Time = (string)jd["time"]
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
                    PictureList.Add(new Picture()
                    {
                        PictureName = (string)jd["picture_name"],
                        Time = (string)jd["time"]
                    });
                    return string.Empty;
                }
                else
                    return "获取图片列表失败！" + (string)jd["message"];
            }
        }
    }
}
