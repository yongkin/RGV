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
using System.Windows.Shapes;
using System.Drawing;

namespace MicroSvr.WPFRunner
{
    /// <summary>
    /// StartFrm.xaml 的交互逻辑
    /// </summary>
    public partial class StartFrm : Window
    {
        public static StartFrm Instance { get { return RunnerInstanceHelper.Create<StartFrm>(); } }

        public StartFrm()
        {
            InitializeComponent();

        }

        private void StartFrm1_Loaded(object sender, RoutedEventArgs e)
        {
            this.Icon = RunHelper.Instance.AppIcon;
            RunHelper.Instance.IconChanged += (icon) => { this.Icon = icon; };
            if (RunHelper.Instance.MainFrm != null)
            {
                this.Title = $"启动日志[{RunHelper.Instance.MainFrm.Title}]";
            }
            if (new AdminRunHelper().IsRunAsAdmin())
            {
                this.Title = $"{this.Title} (管理员)";
            }
            this.TBDir.Text = Process.GetCurrentProcess().MainModule.FileName;
        }

        private void showMainFrm()
        {
            if ((!Application.Current.Dispatcher.CheckAccess()))
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => { showMainFrm(); }));
                return;
            }
            RunHelper.Instance.MainFrm.Show();
        }

        internal void ShowMainForm(IntPtr hwnd)
        {
            showMainFrm();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var file = new System.IO.FileInfo(TBDir.Text);
            if (!file.Exists)
            {
                return;
            }
            var p = System.Diagnostics.Process.Start("explorer.exe", file.Directory.FullName);
        }

        internal void ShowMsg(int level, string msg)
        {
            try
            {
                showMsgInForm(DateTime.Now, 1, string.Empty, msg);
            }
            catch (Exception ex)
            {

            }
        }

        private void showMsgInForm(DateTime dtime, int level, string topic, string msg)
        {
            if (((!Application.Current.Dispatcher.CheckAccess())))
            {
                Application.Current.Dispatcher.BeginInvoke(new Action<DateTime, int, string, string>(showMsgInForm), dtime, level, topic, msg);
                return;
            }
            var log = new StringBuilder();
            string text = tbMsg.Text; // 获取TextBox的文本内容  
            string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None); // 将文本内容分割成行  
            int numberOfLines = lines.Length; // 计算行的数量
            if (numberOfLines > 500)
            {
                var lines2 = this.tbMsg.Text.Split('\n').TakeLast(20);
                log.Append(string.Join('\n', lines2));
            }
            else
            {
                log.Append(this.tbMsg.Text);
            }
            log.Append(dtime.ToString("▶ yyyy-MM-dd HH:mm:ss.fff "));
            if (!string.IsNullOrWhiteSpace(topic))
            {
                log.Append(topic);
            }
            if (level < 5)
            {
                log.Append(" ● ");
            }
            else
            {
                log.Append(" ◆ ");
            }
            log.Append(msg).AppendLine();
            this.tbMsg.Text = log.ToString();
            
        }

    }
}
