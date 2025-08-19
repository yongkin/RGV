using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SafeRun.WCS.IP_Controls.ControlPage
{
    /// <summary>
    /// PopupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PopupWindow : Window
    {
        /// <summary>
        /// 启动时间
        /// </summary>
        private DateTime _startTime;
        /// <summary>
        /// 限制时间(min)
        /// </summary>
        private int _limitTime = 1;

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string,string> _showParams = new Dictionary<string,string>();
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, string> _operNodes = new Dictionary<string, string>();

        private Func<Dictionary<string, string>,List<(string key,string val)>>? _updAction;
        private Func<Dictionary<string, string>,List<(string key,string val)>>? _delAction;
        private Func<Dictionary<string, string>,List<(string key,string val)>>? _completeAction;
        private Func<Dictionary<string, string>, List<(string key, string val)>>? _queryAction;

        /// <summary>
        /// 右击显示窗体
        /// </summary>
        /// <param name="showParams">显示参数</param>
        /// <param name="operNodes">可操作参数</param>
        /// <param name="updAction">更新委托事件</param>
        /// <param name="delAction">清除委托事件</param>
        /// <param name="completeAction">完成委托事件</param>
        public PopupWindow(Dictionary<string, string> showParams, Dictionary<string, string> operNodes, Func<Dictionary<string, string>, List<(string key, string val)>>? updAction, Func<Dictionary<string, string>, List<(string key, string val)>>? delAction, Func<Dictionary<string, string>, List<(string key, string val)>>? completeAction, Func<Dictionary<string, string>, List<(string key, string val)>>? queryAction)
        {
            InitializeComponent();

            _showParams = new Dictionary<string, string>(showParams);//传入显示参数，去继承入参

            _operNodes = new Dictionary<string, string>(operNodes);//去继承入参

            _updAction = updAction;
            _delAction = delAction;
            _completeAction = completeAction;
            _queryAction = queryAction;

            _startTime = DateTime.Now;
            grid_General.Visibility = Visibility.Visible;
            grid_RGVInfo.Visibility = Visibility.Hidden;
        }


        public List<(string RGVNo, string TaskNo, string Barcode, string Status)> RGVInfoList { get; set; }

        public PopupWindow(List<(string RGVNo, string TaskNo, string Barcode, string Status)> rgvInfoList)
        {
            InitializeComponent();
            this.btn_cleanPlcTask.Visibility = Visibility.Hidden;
            this.btn_Complete.Visibility = Visibility.Hidden;
            this.btn_updPlcTask.Visibility = Visibility.Hidden;
            this.btn_del.Visibility = Visibility.Hidden;


            this.Title = string.Empty;
            DataTable table = new DataTable();
            table.Columns.Add("RGVNo", typeof(string));
            table.Columns.Add("TaskNo", typeof(string));
            table.Columns.Add("Barcode", typeof(string));
            table.Columns.Add("Status", typeof(string));

            foreach(var item in rgvInfoList)
            {
                table.Rows.Add(item.RGVNo,item.TaskNo,item.Barcode,item.Status);
            }

            dataGrid_RGVInfo.ItemsSource = table.DefaultView;

            grid_General.Visibility = Visibility.Hidden;
            grid_RGVInfo.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(grid_General.Visibility == Visibility.Visible)
            {
                //1.显示页面上的基础信息
                var contrType = _showParams["ContrType"] ?? "";
                var contrTypeName = _showParams["ContrTypeName"] ?? contrType;
                InitPage(contrType, contrTypeName, _showParams["ShowTitle"] ?? "", _showParams["TaskNo"] ?? "");
                //2.处理可编辑项
            }

            InitOpenBtn(_operNodes);

        }

        /// <summary>
        /// 初始化页面显示数据
        /// </summary>
        /// <param name="controlType"></param>
        /// <param name="showTitle"></param>
        /// <param name="taskNo"></param>
        private void InitPage(string controlType,string controlTypeName,string showTitle,string taskNo)
        {
            //1.处理Title显示
            this.Title = $"{controlTypeName}_{showTitle}";
            this.lab_showTitle.Content = $"{controlTypeName}_{showTitle}";
            this.txtB_TaskNo.Text = taskNo;
            //2.查询获取该任务的起点和终点
            var queryParam = new Dictionary<string, string>();
            queryParam.Add("TaskNo", taskNo);
            var taskInfo = _queryAction?.Invoke(queryParam);
            if (taskInfo == null || taskInfo.Count <= 0)
            {
                PageClose("查询任务失败,请确认!!!");
                return;
            }

            foreach (var param in taskInfo)
            {
                switch (param.key)
                {
                    case "TaskNo":
                        txtB_TaskNo.Text = $"{param.val}";
                        break;
                    case "SLocNo":
                        txtB_startingPoint.Text = $"{param.val}";
                        break;
                    case "DLocNo":
                        txtB_end.Text = $"{param.val}";
                        break;
                    case "Error":
                        PageClose(param.val);
                        return;
                }
            }
        }
        
        private void InitOpenBtn(Dictionary<string, string> operNodes)
        {

            btn_updPlcTask.Visibility = Visibility.Hidden;
            btn_cleanPlcTask.Visibility = Visibility.Hidden;
            btn_del.Visibility = Visibility.Hidden;
            btn_Complete.Visibility = Visibility.Hidden;

            foreach (var item in operNodes)
            {
                switch (item.Key)
                {
                    case "updPlcTask":
                        btn_updPlcTask.Visibility = Visibility.Visible;
                        btn_updPlcTask.Tag = item.Value;
                        break;
                    case "cleanPlcTask":
                        btn_cleanPlcTask.Visibility = Visibility.Visible;
                        btn_cleanPlcTask.Tag = item.Value;
                        break;
                    case "delete":
                        btn_del.Visibility = Visibility.Visible;
                        btn_del.Tag = item.Value;
                        break;
                    case "complete":
                        btn_Complete.Visibility = Visibility.Visible;
                        btn_Complete.Tag = item.Value;
                        break;
                }
            }
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            var plcName = btn_cleanPlcTask.Tag.ToString();
            var taskNo = txtB_TaskNo.Text;
            if (string.IsNullOrEmpty(plcName) || string.IsNullOrEmpty(taskNo))
            {
                return;
            }

            if(MessageBox.Show($"请确认更新站台:{_showParams["ShowTitle"]},节点:{plcName}的任务号为:{taskNo}","更新确认!",MessageBoxButton.YesNo) == MessageBoxResult.No) { return; }

            var updParam = new Dictionary<string, string>();
            updParam.Add(plcName, taskNo);
            var result = _updAction?.Invoke(updParam);
            HandleResultShow(result);
            TimedCloseForm(10000);
            if (GetHandleResult("UpdResult", result) == "true")
            {
                this.txtB_TaskNo.Text = taskNo;
            }
            //TimedCloseForm();
        }

        private void cleanBtn_Click(object sender, RoutedEventArgs e)
        {
            var plcName = btn_cleanPlcTask.Tag.ToString();
            if (string.IsNullOrEmpty(plcName))
            {
                return;
            }
            if (MessageBox.Show($"请确认清除站台:{_showParams["ShowTitle"]},节点:{plcName}的任务号", "清除确认!", MessageBoxButton.YesNo) == MessageBoxResult.No) { return; }
            var updParam = new Dictionary<string, string>();
            updParam.Add(plcName, "");
            var result = _updAction?.Invoke(updParam);
            HandleResultShow(result);
            TimedCloseForm(10000);
            if (GetHandleResult("UpdResult", result) == "true")
            {
                this.txtB_TaskNo.Text = "";
            }
        }

        private void delBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsBeyondLimitTime())
            {
                return;
            }
            var delParam = new Dictionary<string, string>();
            delParam.Add("TaskNo", _showParams["TaskNo"]);
            var result = _delAction?.Invoke(delParam);
            HandleResultShow(result);
        }

        private void btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            if(IsBeyondLimitTime())
            {
                return;
            }

            var taskNoContrVal = this.txtB_TaskNo.Text;
            var taskNo = _showParams["TaskNo"]??"";
            if (string.IsNullOrEmpty(taskNoContrVal) || string.IsNullOrEmpty(taskNo) || taskNoContrVal != taskNo)
            {
                HandleResultShow(new List<(string key, string val)>() {("CompleteCheckWarning", "") });
                return;
            }

            var completeParam = new Dictionary<string, string>();
            completeParam.Add("TaskNo", taskNo);
            completeParam.Add("ContrType", _showParams["ContrType"] ?? "");
            var result = _completeAction?.Invoke(completeParam);
            HandleResultShow(result);
        }

        private void HandleResultShow(List<(string key,string val)> result)
        {
            foreach (var item in result) 
            {
                switch (item.key)
                {
                    case "Error":
                        //MessageBox.Show($"{item.val}", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                        lab_tipInfo.Text = $"警告:{item.val}";
                        break;
                    case "Result":
                        lab_tipInfo.Text = $"信息:{item.val}";
                        //MessageBox.Show($"{item.val}", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case "UpdResult":
                        lab_tipInfo.Text = item.val == "true" ? $"信息:更新节点成功!!!" : $"信息:更新节点失败!!!";
                        //MessageBox.Show($"{item.val}", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case "CompleteCheckWarning":
                        lab_tipInfo.Text = "警告:当前操作信息已经过期,请关闭后重试!!!";
                        break;
                }
            }
        }

        private string GetHandleResult(string key, List<(string key, string val)> result)
        {
            var item = result.FirstOrDefault(x => x.key == key);
            if(item.key == null)
            {
                return "";
            }
            else {  return item.val; }
        }

        /// <summary>
        /// 是否超出限定时间
        /// </summary>
        /// <returns></returns>
        private bool IsBeyondLimitTime()
        {
            var timeSpan = DateTime.Now - _startTime;
            if (timeSpan.Minutes <= _limitTime)
            {
                return false;
            }
            else
            {
                PageClose("超出限定操作时间,请重新操作!!!");
                TimedCloseForm();
                return true;
            }
        }

        private void PageClose(string msg)
        {
            HandleResultShow(new List<(string key, string val)>() { ("Error", msg) });

            
        }

        private void TimedCloseForm(int time = 5000)
        {
            
            Task.Delay(time).ContinueWith(t =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.Close();
                });
            });
            //this.Close();
        }

    }
}
