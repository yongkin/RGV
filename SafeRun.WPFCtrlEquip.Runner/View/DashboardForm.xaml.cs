using SafeRun.WPFCtrlEquip.WPFRunner.ControlViews;
using System.Windows.Controls;
using System.Windows.Input;


namespace SafeRun.WPFCtrlEquip.WPFRunner
{


    /// <summary>
    /// DashboardForm.xaml 的交互逻辑
    /// </summary>
    public partial class DashboardForm : UserControl
    {

        DashboardFormModel vm = new DashboardFormModel();
        public DashboardForm()
        {
            this.DataContext = vm;
            InitializeComponent();

            ControlInit();

            SetErrorDateTimeControls(vm.ErrorStartDate, vm.ErrorEndDate);

        }

        private void ControlInit()
        {
            //隐藏按钮全局显示
            btn_AllPanel.Visibility = Visibility.Hidden;

            // 绑定数据到 ComboBox
            //comB_TaskStatus.ItemsSource = items;
            comB_TaskStatus.ItemsSource = vm.TaskStatusComB;
            comB_TaskStatus.DisplayMemberPath = "Value"; // 显示名称
            comB_TaskStatus.SelectedValuePath = "Key";  // 返回代码
            comB_TaskStatus.SelectedValue = "";//默认值

            //任务时间
            //dateT_TaskStartTime.SelectedDateTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00"));
            //dateT_TaskEndTime.SelectedDateTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 23:59"));

            //comB_EquipmentCode.ItemsSource = vm.PlcCodeComB;
            //comB_EquipmentCode.DisplayMemberPath = "Value"; // 显示名称
            //comB_EquipmentCode.SelectedValuePath = "Key";  // 返回代码
            //comB_EquipmentCode.SelectedValue = "";//默认值
        }

        /// <summary>
        /// 双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItem_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            /*
            // 处理双击事件
            var listViewItem = sender as ListViewItem;
            if (listViewItem != null && listViewItem.Content != null)
            {
                // 在此处添加你的逻辑代码
                var selItem = listViewItem.Content as _BlockInfo;

                // 创建辅助窗口
                // 加载布局页面: selItem.biz.Block.Name = MDC_EQUIP_BLOCK.BLOCK_NAME
                var auxiliaryWindow = new Window
                {
                    //Title = "Auxiliary Page",
                    //Content = new ControlViews.Page1() // 设置窗口内容为辅助页面
                    Title = "",
                    Content = new DashboardDetailForm(selItem, selItem.biz.Block.Name),
                    WindowState = WindowState.Maximized,
                    //WindowStyle = WindowStyle.SingleBorderWindow,
                    //ResizeMode = ResizeMode.NoResize,

                };

                // 显示辅助窗口
                auxiliaryWindow.ShowDialog();

            }
            */
        }

        private void TabControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMonitorPage();


        }


        #region 监控页面Tab

        private void LoadMonitorPage()
        {
            fra_monitorPage.Content = new FT_FullLayout_CC();
        }

        private void ReloadMonitorPage()
        {
            //fra_monitorPage.Content = null;
            //fra_monitorPage.Content = new FT_FullLayout_CC();
            if (fra_monitorPage.Content is Page oldPage)
            {
                oldPage.NavigationService?.RemoveBackEntry();//释放资源
            }
            fra_monitorPage.Content = null;
            fra_monitorPage.Content = new FT_FullLayout_CC();
        }
        private void btn_ReloadMonitor_Click(object sender, RoutedEventArgs e)
        {
            ReloadMonitorPage();
        }

        #endregion

        #region 任务Tab
        private void btn_TaskQuery_Click(object sender, RoutedEventArgs e)
        {
            var dicQueryCirteria = new Dictionary<string, string>();

            dicQueryCirteria.Add("TaskNo", txt_TaskNo.Text);
            dicQueryCirteria.Add("TaskStatus", comB_TaskStatus.SelectedValue.ToString());
            dicQueryCirteria.Add("SLocNo", txt_SLocNo.Text);
            dicQueryCirteria.Add("DLocNo", txt_DLocNo.Text);

            vm.TaskQuery(dicQueryCirteria);
        }
        #endregion
        #region 设备异常Tab
        private void btn_EquipmentError_Click(object sender, RoutedEventArgs e)
        {
            GetErrorDateTimeControls();
            vm.UpdatePagedItems();
        }

        private object GetDateTimeControls(string tagVal)
        {
            object result = null;
            foreach (var item in stackPanel.Children)
            {
                if (!(item is StackPanel)) continue;
                var stackItem = (item as StackPanel);
                foreach (var itemChildren in stackItem.Children)
                {
                    //var itemChildrenType = (HandyControl.Controls.DatePicker)itemChildren;

                    var tag = ((System.Windows.FrameworkElement)itemChildren).Tag;

                    if (tag != null && tag.ToString() == tagVal) 
                    {
                        result = itemChildren;
                    }
                    
                    
                }
            }

            return result;
        }

        private void SetErrorDateTimeControls(DateTime startTime, DateTime endTime)
        {

            var item = GetDateTimeControls("ErrorStartDate");
            //((System.Windows.Controls.DatePicker)item).Text = startTime;
            ((System.Windows.Controls.DatePicker)item).SelectedDate = startTime;
            var itemEnd = GetDateTimeControls("ErrorEndDate");
            ((System.Windows.Controls.DatePicker)itemEnd).SelectedDate = endTime;
            //((HandyControl.Controls.DatePicker)GetDateTimeControls("ErrorEndDate")).Text = endTime;
            //if(dateControl == null) return;
            //switch (dateControl.Tag.ToString())
            //{
            //    case "ErrorStartDate":
            //        dateControl.Text = startTime;
            //        break;
            //    case "EndDate":
            //        dateControl.Text = endTime;
            //        break;
            //}

        }

        private void GetErrorDateTimeControls()
        {
            var item = GetDateTimeControls("ErrorStartDate");
            vm.ErrorStartDate = ((System.Windows.Controls.DatePicker)item).SelectedDate??DateTime.Now;
            var itemEnd = GetDateTimeControls("ErrorEndDate");
            vm.ErrorEndDate = ((System.Windows.Controls.DatePicker)itemEnd).SelectedDate??DateTime.Now;
            

        }
        #endregion

        
    }


}
