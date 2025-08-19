using MicroSvr.Comm;
using MicroSvr.XRun;
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

namespace SafeRun.WPFCtrlEquip.WPFRunner
{
    /// <summary>
    /// MainFrm.xaml 的交互逻辑
    /// </summary>
    public partial class MainFrm : Window
    {
        public MainFrm()
        {
            InitializeComponent();

            showMainFrm();
        }

        private void showMainFrm()
        {
            MessageBox.Show("监控平台服务启动");

            //startserver();

            //initializeDockPanel();
            //this.Text = $"{XApp.Current.AppName} @ {XApp.Current.Localhost}:{XApp.Current.AppPort}";
            //initializeRunnerFrm();

            ShowUserControl(new DashboardForm());
        }

        private int startserver()
        {
            var result = 0;
            var type = "SafeRun.CtrlEquip.SpiService.MDCStartHelper,SafeRun.CtrlEquip.SpiService".ToType();

            if (type == null)
            {
                return result;
            }
            result = (int)RunHelper.Invoke(InvokeModel.Default, type, "Instance.Start", null, null, null, null);
            return result;
        }

        private void ShowUserControl(UserControl userControl)
        {
            if (mainContent.Content is IDisposable disposableControl)
            {
                disposableControl.Dispose();
            }
            mainContent.Content = (userControl);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowUserControl(new DashboardForm());
        }
    }
}
