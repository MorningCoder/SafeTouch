using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using LitJson;
using Microsoft.Maps.MapControl.WPF;

namespace PhoneSafe
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        //登录对话框对象
        BaseMetroDialog dialog;
        //提供network全局对象
        NetWork network;
        //记录本次登录用户名
        string name;
        //记录用户密码
        string psw;
        //记录客户端是否在线
        bool isOnline;

        public MainWindow()
        {
            InitializeComponent();
            //初始化Network
            network = NetWork.Create();
        }

        //异步读取联系人列表
        private async Task GetContacterList()
        {
            Task<string> task = Contacter.GetContacterList(this.name, network);
            string result = await task;
            if (result != string.Empty)
                this.Dispatcher.Invoke(() => {this.error.Text = result;});
            this.Dispatcher.Invoke(() => { this.contacter.ItemsSource = Contacter.ContacterList; });
        }

        //用于异步读取短信列表并存储于MessageList
        private async Task GetMessageList()
        {
            Task<string> get_task = Message.GetMessage(network, this.name);
            string result = await get_task;
            if (result != string.Empty)
                this.Dispatcher.Invoke(() => { this.error.Text = result; });
            this.Dispatcher.Invoke(() => { this.message.ItemsSource = Message.MessageList; });

        }

        //用于异步读取手机基本信息
        private async Task GetPhoneInfo()
        {
            //读取手机基本信息
            string info = await network.GetPhoneInfo(this.name);
            //解析json
            JsonData jd = JsonMapper.ToObject(info);
            if ((string)jd["result"] != string.Empty)
            {
                await this.ShowMessageAsync("", "获取手机基本信息失败！" + (string)jd["message"]);
                return;
            }
            else
            {
                //赋值
                this.Dispatcher.Invoke(() => { this.tab.Header = (string)jd["phone_name"]; });
                this.Dispatcher.Invoke(() => { this.power.Text = "电量" + (string)jd["power"]; });
                //解析
                double latitude = double.Parse((string)jd["latitude"]);
                double longtitude = double.Parse((string)jd["longtitude"]);
                this.Dispatcher.Invoke(() => { this.map.Center = new Location(latitude, longtitude); });

            }
        }

        //获取系统通知
        private async Task GetNotice()
        {
           Task<string> task = Notice.GetNotice(network,this.name);
            string result = await task;
            if (result != string.Empty)
                this.Dispatcher.Invoke(() => { this.error.Text = result ;});
            this.Dispatcher.Invoke(() => { this.notice.ItemsSource = Notice.NoticeList; });

        }

        private async Task GetPictureList()
        {
            Task<string> task = Picture.GetPictureList(network, this.name);
            string result = await task;
            if (result != string.Empty)
                this.Dispatcher.Invoke(() => { this.error.Text = result; });
            this.Dispatcher.Invoke(() => { this.picture.ItemsSource = Picture.PictureList; });
        }

        //在窗口加载事件中处理登录并读取手机基本信息
        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //LoginDialogData data = await this.ShowLoginAsync("login", "message");
            dialog = (BaseMetroDialog)this.Resources["InitialDialog"];
            await this.ShowMetroDialogAsync(dialog);
            
        }

        //定时更新线程函数
        private async void update()
        {
            while(this.name != string.Empty)
            {
                await GetMessageList();
                await GetContacterList();
                await GetPhoneInfo();
                await GetNotice();
                await GetPictureList();
                Thread.Sleep(30 * 1000);
            }
        }

        //处理登录事件
        private async void login_click(object sender, RoutedEventArgs e)
        {
            //获取对话框中的值
            TabControl c = dialog.DialogTop as TabControl;
            Canvas v = c.SelectedContent as Canvas;
            IEnumerable<PasswordBox> w = v.FindChildren<PasswordBox>();
            IEnumerable<TextBlock> t = v.FindChildren<TextBlock>();
            IEnumerable<TextBox> b = v.FindChildren<TextBox>();
            this.name = b.First<TextBox>().Text;
            this.psw = w.First<PasswordBox>().Password;
            //检测输入框
            if (this.name == string.Empty || this.psw == string.Empty)
            {
                t.First<TextBlock>().Text = "用户名或密码为空！请重新输入";
                return;
            }
            //开始登录
            Task<string> login_task = network.Login(name, psw);
            t.First<TextBlock>().Text = "正在登录...";
            string res = await login_task;
            //处理json
            try
            {
                JsonData jd = JsonMapper.ToObject(res);
                if ((string)jd["result"] == "0")
                {
                    isOnline = true;
                    await this.HideMetroDialogAsync(dialog);
                    Thread UpdateThread = new Thread(new ThreadStart(update));
                    UpdateThread.Start();
                    return;
                }
                else if((string)jd["result"] == "1")
                {
                    isOnline = false;
                    this.tab.Header += "（离线）";
                    await this.HideMetroDialogAsync(dialog);
                    Thread UpdateThread = new Thread(new ThreadStart(update));
                    UpdateThread.Start();
                    return;
                }
                else
                {
                    t.First<TextBlock>().Text = "登录失败！" + (string)jd["message"];
                }
            }
            catch (Exception ee)
            {
                t.First<TextBlock>().Text = "登录失败！" + "请检查网络设置";
            }
        }

        //处理注册事件
        private async void register_click(object sender, RoutedEventArgs e)
        {
            //获取对话框中的值
            TabControl c = dialog.DialogTop as TabControl;
            Canvas p = c.SelectedContent as Canvas;
            IEnumerable<PasswordBox> w = p.FindChildren<PasswordBox>();
            IEnumerable<TextBlock> t = p.FindChildren<TextBlock>();
            IEnumerable<TextBox> b = p.FindChildren<TextBox>();
            this.name = b.First<TextBox>().Text;
            this.psw = w.First<PasswordBox>().Password;
            string confirm = b.ElementAt<TextBox>(1).Text;
            //检测输入框
            if (this.name == string.Empty || this.psw == string.Empty)
            {
                t.First<TextBlock>().Text = "用户名或密码为空！请重新输入";
                return;
            }
            //开始注册
            Task<string> register_task = network.Register(name, psw,confirm);
            t.First<TextBlock>().Text = "正在注册...";
            string res = await register_task;
            //处理json
            try
            {
                JsonData jd = JsonMapper.ToObject(res);
                if ((string)jd["result"] == "0")
                {
                    t.First<TextBlock>().Text = "注册成功！";
                    await this.HideMetroDialogAsync(dialog);
                    return;
                }
                else
                {
                    t.First<TextBlock>().Text = "注册失败！" + (string)jd["message"];
                }
            }
            catch (Exception ee)
            {
                t.First<TextBlock>().Text = "注册失败！" + "请检查网络设置";
            }
        }

        //退出按钮的事件处理
        private void exit_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        //锁屏处理
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(isOnline)
            {
                this.erroring.IsActive = true;
                string result = await network.UploadInstructor(this.name, "2");
                JsonData jd = JsonMapper.ToObject(result);
                if ((string)jd["result"] != "0" || (string)jd["result"] != "*")
                    this.error.Text = "指令重复，发送失败！";
                this.erroring.IsActive = false;
                return;
            }
            else
            {
                await this.ShowMessageAsync("", "手机尚未在线！");
                return;
            }
        }

        private async void SendMsg(object sender, RoutedEventArgs e)
        {
            if(!isOnline)
            {
                await this.ShowMessageAsync("", "手机尚未在线！");
                return;
            }
            this.SendMsgFly.IsOpen = true;
        }

        //编辑新短信发送
        private async void Send(object sender, RoutedEventArgs e)
        {
            string num = this.num.Text;
            string con = this.con.Text;
            this.ring.IsActive = true;
            string result = await network.UploadSendingMsg(this.name, num, con);
            this.ring.IsActive = false;
            JsonData jd = JsonMapper.ToObject(result);
            await this.ShowMessageAsync("", (string)jd["message"]);
            if ((string)jd["result"] == "0")
            {
                this.num.Text = "";
                this.con.Text = "";
            }
        }

        private void message_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Message temp = message.SelectedItem as Message;
            this.ShowMessageFly.IsOpen = true;
            Grid g = ShowMessageFly.Content as Grid;
            IEnumerable<TextBlock> c = g.FindChildren<TextBlock>();
            c.ElementAt<TextBlock>(1).Text = temp.SenderNum;
            c.ElementAt<TextBlock>(3).Text = temp.Content;
        }

        private void picture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Picture p = picture.SelectedItem as Picture;
            
        }

        //处理下载图片事件
        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Picture p = picture.SelectedItem as Picture;
            if(p == null)
            {
                await this.ShowMessageAsync("", "未选中图片");
                return;
            }
            this.error.Text = "正在下载...";
            this.erroring.IsActive = true;
            await network.DownloadPicture(this.name, p.PictureName);
            this.error.Text ="下载完成";
            this.erroring.IsActive = false;
        }

        //清除数据
        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if(!isOnline)
            {
                await this.ShowMessageAsync("", "手机尚未在线！");
                return;
            }
            this.erroring.IsActive = true;
            string result = await network.UploadInstructor(this.name, "3");
            JsonData jd = JsonMapper.ToObject(result);
            if ((string)jd["result"] != "0" || (string)jd["result"] != "*")
                this.error.Text = "指令重复，发送失败！";
            this.erroring.IsActive = false;
            return;
        }

        //获取验证码
        private async void GetConfirmCode_Click(object sender, RoutedEventArgs e)
        {
            //获取对话框中的值
            TabControl c = dialog.DialogTop as TabControl;
            Canvas p = c.SelectedContent as Canvas;
            IEnumerable<TextBlock> t = p.FindChildren<TextBlock>();
            IEnumerable<TextBox> b = p.FindChildren<TextBox>();
            //检测输入框
            if (b.First<TextBox>().Text == "")
            {
                t.First<TextBlock>().Text = "用户名为空！请重新输入";
                return;
            }
            //开始验证
            Task<string> confirm_task = network.Confirm(b.First<TextBox>().Text);
            t.First<TextBlock>().Text = "正在获取验证码...";
            string result = await confirm_task;
            JsonData jd = JsonMapper.ToObject(result);
            t.First<TextBlock>().Text = (string)jd["message"];
        }
    }
}
