using MicroSvr.Comm;
using SafeRun.WPFCtrlEquip.WPFRunner.ControlViews;
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

namespace SafeRun.WPFCtrlEquip.WPFRunner
{
   
    /// <summary>
    /// DashboardDetailForm.xaml 的交互逻辑
    /// </summary>
    public partial class DashboardDetailForm : UserControl
    {
        DashboardDetailFormModel frmData = new DashboardDetailFormModel();

        public DashboardDetailForm()
        {
            this.DataContext = frmData;
            InitializeComponent();
        }

        public DashboardDetailForm(_BlockInfo blockInfo,string showContrName)
        {
            InitializeComponent();
            //初始化节点数据
            frmData.InitFrmData(blockInfo.biz);
            this.DataContext = frmData;
            //加载现场布局页面
            var pageControl = GetPageControlByName(showContrName);
            if(pageControl != null)
            {
                ShowControl(pageControl);
            }
        }

        /// <summary>
        /// 通过名称获取控件
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        private UserControl GetUserControlByName(string pageName)
        {
            // 根据pageName获取对应的UserControl
            var controlType = Type.GetType($"SafeRun.WPFCtrlEquip.WPFRunner.ControlViews.{pageName}");
            if (controlType != null)
            {
                return Activator.CreateInstance(controlType) as UserControl;
            }
            else
            {
                // 如果找不到对应的UserControl，返回null或者抛出一个异常
                return null;
            }
        }


        private Page GetPageControlByName(string pageName)
        {

            // 根据pageName获取对应的UserControl
            var controlType = Type.GetType($"SafeRun.WPFCtrlEquip.WPFRunner.ControlViews.{pageName}");
            if (controlType != null)
            {
                return Activator.CreateInstance(controlType) as Page;
            }
            else
            {
                // 如果找不到对应的UserControl，返回null或者抛出一个异常
                return null;
            }
        }

        /// <summary>
        /// 显示控件
        /// </summary>
        /// <param name="userControl"></param>
        private void ShowControl(UserControl userControl)
        {
            if (controlPanel.Content is IDisposable disposableControl)
            {
                disposableControl.Dispose();
            }
            controlPanel.Content = (userControl);
        }

        private void ShowControl(Page pageControl)
        {
            if (controlPanel.Content is IDisposable disposableControl)
            {
                disposableControl.Dispose();
            }
            Frame myFrame = new Frame();
            myFrame.Content = pageControl;
            controlPanel.Content = (myFrame);
        }

    }
}
