
//堆垛机

using SafeRun.WCS.IP_Controls.ControlPage;
using SafeRun.WPFCtrlEquip.WPFRunner.Common;
using System.Collections.Generic;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using static SafeRun.WPFCtrlEquip.WPFRunner.Common.CommonMethods;
using System.Linq;
using System.Threading.Tasks;

namespace SafeRun.WCS.IP_Controls
{

    /// <summary>
    /// 堆垛机
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SafeRun.WCS.IP_Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SafeRun.WCS.IP_Controls;assembly=SafeRun.WCS.IP_Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:Stacker/>
    ///
    /// </summary>

    public class Stacker : HMIControlBase
    {
        private double ControlWidth = 500;
        private Grid StackerCrn = null;
        static Stacker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Stacker), new FrameworkPropertyMetadata(typeof(Stacker)));
        }

        #region 设置自定义控件属性值

        #region 任务号
        public static readonly DependencyProperty StackerTaskNoProperty = DependencyProperty.Register("StackerTaskNo", typeof(string), typeof(Stacker));

        /// <summary>
        /// 任务号
        /// </summary>
        [Category("HMIC")]
        public string StackerTaskNo
        {
            get { return (string)GetValue(StackerTaskNoProperty); }
            set { SetValue(StackerTaskNoProperty, value); }
        }

        #endregion

        #region 堆垛机列数
        public static readonly DependencyProperty CrnColNumProperty = DependencyProperty.Register("CrnColNum", typeof(int), typeof(Stacker));

        /// <summary>
        /// 堆垛机列数
        /// </summary>
        [Category("HMIC")]
        public int CrnColNum
        {
            get { return (int)GetValue(CrnColNumProperty); }
            set { SetValue(CrnColNumProperty, value); }
        }

        #endregion

        #region 堆垛机正逆方向(默认小到大为正，左到右)
        public static readonly DependencyProperty CrnFIProperty = DependencyProperty.Register("CrnFI", typeof(bool), typeof(Stacker));

        /// <summary>
        /// 堆垛机正逆方向(默认小到大为正，左到右)
        /// True:正,False:逆
        /// </summary>
        [Category("HMIC")]
        public bool CrnFI
        {
            get { return (bool)GetValue(CrnFIProperty); }
            set { SetValue(CrnFIProperty, value); }
        }

        #endregion

        #region 是否使用文本
        public static readonly DependencyProperty UseTxtProperty = DependencyProperty.Register("UseTxt", typeof(string), typeof(Stacker));

        /// <summary>
        /// 任务号
        /// </summary>
        [Category("HMIC")]
        public string UseTxt
        {
            get { return (string)GetValue(UseTxtProperty); }
            set { SetValue(UseTxtProperty, value); }
        }

        #endregion

        #region 空库位数
        public static readonly DependencyProperty CrnEmptyNumProperty = DependencyProperty.Register("CrnEmptyNum", typeof(string), typeof(Stacker));

        /// <summary>
        /// 空库位数
        /// </summary>
        [Category("HMIC")]
        public string CrnEmptyNum
        {
            get { return (string)GetValue(CrnEmptyNumProperty); }
            //set { SetValue(CrnEmptyNumProperty, value); }
            set
            {
                if (Dispatcher.CheckAccess())
                {
                    // 当前线程是 UI 线程，直接设置值
                    SetValue(CrnEmptyNumProperty, value);
                }
                else
                {
                    // 当前线程不是 UI 线程，使用 Dispatcher 切换到 UI 线程
                    Dispatcher.Invoke(() => SetValue(CrnEmptyNumProperty, value));
                }
            }
        }

        #endregion

        #region 有货位数
        public static readonly DependencyProperty CrnMaterNumProperty = DependencyProperty.Register("CrnMaterNum", typeof(string), typeof(Stacker));

        /// <summary>
        /// 有货位数
        /// </summary>
        [Category("HMIC")]
        public string CrnMaterNum
        {
            get { return (string)GetValue(CrnMaterNumProperty); }
            //set { SetValue(CrnMaterNumProperty, value); }
            set
            {
                if (Dispatcher.CheckAccess())
                {
                    // 当前线程是 UI 线程，直接设置值
                    SetValue(CrnMaterNumProperty, value);
                }
                else
                {
                    // 当前线程不是 UI 线程，使用 Dispatcher 切换到 UI 线程
                    Dispatcher.Invoke(() => SetValue(CrnMaterNumProperty, value));
                }
            }
        }

        #endregion

        #region 空托库位数
        public static readonly DependencyProperty CrnPalletNumProperty = DependencyProperty.Register("CrnPalletNum", typeof(string), typeof(Stacker));

        /// <summary>
        /// 空托库位数
        /// </summary>
        [Category("HMIC")]
        public string CrnPalletNum
        {
            get { return (string)GetValue(CrnPalletNumProperty); }
            //set { SetValue(CrnPalletNumProperty, value); }
            set
            {
                if (Dispatcher.CheckAccess())
                {
                    // 当前线程是 UI 线程，直接设置值
                    SetValue(CrnPalletNumProperty, value);
                }
                else
                {
                    // 当前线程不是 UI 线程，使用 Dispatcher 切换到 UI 线程
                    Dispatcher.Invoke(() => SetValue(CrnPalletNumProperty, value));
                }
            }
        }

        #endregion

        #region 库位使用率
        public static readonly DependencyProperty CrnRatioProperty = DependencyProperty.Register("CrnRatio", typeof(string), typeof(Stacker));

        /// <summary>
        /// 库位使用率
        /// </summary>
        [Category("HMIC")]
        public string CrnRatio
        {
            get { return (string)GetValue(CrnRatioProperty); }
            //set { SetValue(CrnRatioProperty, value); }
            set
            {
                if (Dispatcher.CheckAccess())
                {
                    // 当前线程是 UI 线程，直接设置值
                    SetValue(CrnRatioProperty, value);
                }
                else
                {
                    // 当前线程不是 UI 线程，使用 Dispatcher 切换到 UI 线程
                    Dispatcher.Invoke(() => SetValue(CrnRatioProperty, value));
                }
            }
        }

        #endregion

        #region 显示堆垛机编号
        public static readonly DependencyProperty ShowCrnNoProperty = DependencyProperty.Register("ShowCrnNo", typeof(string), typeof(Stacker));

        /// <summary>
        /// 任务号
        /// </summary>
        [Category("HMIC")]
        public string ShowCrnNo
        {
            get { return (string)GetValue(ShowCrnNoProperty); }
            set { SetValue(ShowCrnNoProperty, value); }
        }

        #endregion

        #region 目标库位
        public static readonly DependencyProperty CrnTargetLocProperty = DependencyProperty.Register("CrnTargetLoc", typeof(string), typeof(Stacker));

        /// <summary>
        /// 任务号
        /// </summary>
        [Category("HMIC")]
        public string CrnTargetLoc
        {
            get { return (string)GetValue(CrnTargetLocProperty); }
            set { SetValue(CrnTargetLocProperty, value); }
        }

        #endregion

        #region 任务类型(出库、入库、移库)
        public static readonly DependencyProperty CrnTaskTypeProperty = DependencyProperty.Register("CrnTaskType", typeof(string), typeof(Stacker));

        /// <summary>
        /// 任务号
        /// </summary>
        [Category("HMIC")]
        public string CrnTaskType
        {
            get { return (string)GetValue(CrnTaskTypeProperty); }
            set { SetValue(CrnTaskTypeProperty, value); }
        }

        #endregion

        #endregion

        #region 继承父类读取标签相关


        /// <summary>
        /// 继承父类读取标签处理
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public override bool SetTagReader(string key, string content)
        {
            base.SetTagReader(key, content);
            UseTxt = "未启用";
            this.ShowCrnNo = ShowTitle.Replace("CRN", "");
            return true;
        }

        /// <summary>
        /// 继承父类显示控件处理
        /// </summary>
        /// <param name="contrName"></param>
        /// <param name="contrVal"></param>
        /// <returns></returns>
        public override Action SetShowControls(string contrName, string contrVal)
        {
            try
            {
                contrVal = DataDisassembly("FT_FullLayout_CC", contrName, contrVal);
                switch (contrName)
                {
                    case "StackerTaskNo":
                        var oldTask = StackerTaskNo;
                        StackerTaskNo = contrVal == "0" ? "" : contrVal;
                        HandleRefreshTask(oldTask, StackerTaskNo);
                        break;
                    //case "CrnTargetLoc": //通过接口获取了HandleRefreshTask
                    //    CrnTargetLoc = contrVal == "0" ? "" : contrVal;
                    //    break;
                    //case "CrnTaskType": //通过接口获取了HandleRefreshTask
                    //    CrnTaskType = contrVal == "0" ? "" : contrVal;
                    //    break;
                    case "CRNCurrentCol":
                        contrVal = (string.IsNullOrEmpty(contrVal) || contrVal.Trim() == "0") ? "1" : contrVal;
                        StackerRun(ControlWidth, CrnColNum, int.Parse(contrVal));
                        break;
                    case "AutoMode":
                        if (contrVal == "1")
                        {
                            //自动
                            SetModeLampAnimationColor(ColorLibrary.AutoRunColor);
                        }
                        else
                        {
                            //手动
                            SetModeLampAnimationColor(ColorLibrary.ManualRunColor);
                        }
                        break;
                    case "HeartBeat":
                        if (contrVal == "1")
                        {
                            //正常
                            SetHeartbeatAnimationColor(ColorLibrary.AutoRunColor);
                        }
                        else
                        {
                            //异常
                            SetHeartbeatAnimationColor(ColorLibrary.RedColor);
                        }
                        break;
                }
                base.SetShowControls(contrName, contrVal);

            }
            catch (Exception ex)
            {
                //throw;
            }

            return null;
        }
        #endregion

        #region 重写OnApplyTemplate方法，获取模板中的控件

        /// <summary>
        /// 重写OnApplyTemplate方法，获取模板中的控件
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // 在模板中查找名为 "StackerCrn" 的子级元素
            var stackerContr = GetTemplateChild("StackerContr") as Grid;
            if (stackerContr != null)
            {
                ControlWidth = stackerContr.Width;
            }

            var stackerCrn = GetTemplateChild("StackerCrn") as Grid;
            if (stackerCrn != null)
            {
                StackerCrn = stackerCrn;
                ControlWidth = ControlWidth - StackerCrn.Width;
            }

        }

        #endregion

        #region 自定义方法

        /// <summary>
        /// 堆垛机运行轨迹，通过百分比设置
        /// </summary>
        /// <param name="contrWidth"></param>
        /// <param name="colNum"></param>
        /// <param name="currentCol"></param>
        public void StackerRun(double contrWidth, int colNum, int currentCol)
        {
            double currentRatio = (currentCol - 1) / ((colNum - 1) * 1.000);
            double currentLeft = (contrWidth * currentRatio);

            if (!CrnFI)//反向处理
            {
                currentLeft = contrWidth - currentLeft;
            }

            var limitVal = 15;
            if(currentLeft <= limitVal)
            {
                currentLeft = limitVal;
            }
            else if (currentLeft >= (contrWidth - limitVal))
            {
                currentLeft = (contrWidth - limitVal);
            }

            StackerCrn.Margin = new Thickness(currentLeft, 0, 0, 0);

            //Application.Current.Dispatcher.Invoke(() => {
            //    StackerCrn.Margin = new Thickness(currentLeft, 0, 0, 0);
            //});
        }

        #region 库存信息接口调用更新
        public void AsyncRefreshStock(Func<Dictionary<string, string>, List<(string key, string val)>> func)
        {
            var systemNo = SystemNo;
            HandleRefreshStock(func, systemNo);
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(10000);

                        HandleRefreshStock(func, systemNo);
                    }
                    catch { }
                }
            });
        }

        private void HandleRefreshStock(Func<Dictionary<string, string>, List<(string key, string val)>> func, string systemNo)
        {
            var queryItem = new Dictionary<string, string>();
            queryItem.Add("CrnNo", systemNo);
            var result = func(queryItem);
            foreach (var item in result)
            {
                switch (item.key)
                {
                    case "EmptyNum":
                        CrnEmptyNum = item.val;
                        break;
                    case "MaterNum":
                        CrnMaterNum = item.val;
                        break;
                    case "PalletNum":
                        CrnPalletNum = item.val;
                        break;
                    case "Ratio":
                        CrnRatio = item.val;
                        break;
                }

            }
        }
        #endregion


        #region 堆垛机任务信息更新

        private void HandleRefreshTask(string oldTaskNo, string newTaskNo)
        {

            var taskNo = newTaskNo;

            if (string.IsNullOrEmpty(taskNo))
            {
                CrnTargetLoc = "";
                CrnTaskType = "";
                return;
            }

            if (oldTaskNo == newTaskNo) return;

            var queryItem = new Dictionary<string, string>();
            queryItem.Add("TaskNo", taskNo);
            var result = QueryAction(queryItem);
            //CrnTargetLoc = result.ToJson(); 测试看反馈值
            //return;
            var taskType = result.FirstOrDefault(m => m.key == "TaskType");//100040:生产入库,200070:销售出库,:移库
            var ioType = result.FirstOrDefault(m => m.key == "IOType");//IOType字段 I:入库，O：出库 T：移库
            var taskTypeName = result.FirstOrDefault(m => m.key == "TaskTypeName");
            var sLocNo = result.FirstOrDefault(m => m.key == "SLocNo");
            var dLocNo = result.FirstOrDefault(m => m.key == "DLocNo");

            if (ioType.IsNull() || result.Count <= 0)
            {
                CrnTargetLoc = "";
                CrnTaskType = "";
                return;
            }
            else if (ioType.val == "T" || ioType.val == "T")
            {
                CrnTargetLoc = dLocNo.val;
            }
            else if (ioType.val == "O")
            {
                CrnTargetLoc = sLocNo.val;
            }
            else if (!string.IsNullOrEmpty(ioType.val))
            {
                CrnTargetLoc = dLocNo.val;
            }
            else
            {
                CrnTargetLoc = "";
            }

            if (taskTypeName.IsNull())
            {
                CrnTaskType = "";
            }
            else
            {
                CrnTaskType = taskTypeName.val;
            }

        }
        #endregion



        #endregion

        #region 数据拆解
        public override string DataDisassembly(string typeName, string contrName, string contrVal)
        {
            var result = contrVal;
            if (typeName == "FT_FullLayout_CC")
            {
                result = FitterFT_FullLayout_CC(contrName, contrVal);
            }
            //base.DataDisassembly(typeName, contrName, contrVal);
            return result;
        }

        #region 适配不同的控件及取值
        /// <summary>
        /// 心跳灯异常次数
        /// </summary>
        private int HeartBeatFalseCount { get; set; } = 0;
        private string FitterFT_FullLayout_CC(string contrName, string contrVal)
        {
            var result = contrVal;

            switch (contrName)
            {
                case "StackerTaskNo":
                    result = contrVal;
                    break;
                case "Alarm":
                    result = CommonMethods.UshortChangeInt(ushort.Parse(contrVal), ValidPositions.Pos2).ToString();
                    break;
                case "AutoMode":
                    result = CommonMethods.UshortChangeInt(ushort.Parse(contrVal), ValidPositions.Pos10).ToString();
                    break;
                case "HeartBeat":
                    result = CommonMethods.UshortChangeInt(ushort.Parse(contrVal), ValidPositions.Pos7).ToString();
                    if (result == "0")
                    {
                        HeartBeatFalseCount++;
                        if (HeartBeatFalseCount <= 10)
                        {
                            result = "1";
                        }
                    }
                    else
                    {
                        HeartBeatFalseCount = 0;
                    }
                    break;

            }

            return result;
        }



        #endregion



        #endregion

        #region 右击事件
        public override void Control_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            base.Control_MouseRightButtonDown(sender, e);//调用父类的实现方法

            // 创建并显示新的弹窗
            var operNodes = new Dictionary<string, string>();
            //operNodes.Add("upd", "TaskNoContr");
            //operNodes.Add("delete", "TaskNoContr");
            operNodes.Add("complete", "TaskNoContr");
            var showParam = new Dictionary<string, string>();
            showParam.Add("ContrType", "CRN");
            showParam.Add("ContrTypeName", "堆垛机");
            showParam.Add("ShowTitle", this.ShowTitle);
            showParam.Add("TaskNo", this.StackerTaskNo);

            base.RightClickFrom(showParam, operNodes, UpdAction, DeleteAction, CompleteAction, QueryAction);
            //_popupWindow = new PopupWindow(
            //showParam,
            //operNodes,
            //UpdAction, DeleteAction, CompleteAction,QueryAction);
            ////_popupWindow.Owner = this; // 设置当前窗口为所有者窗口
            //_popupWindow.WindowStartupLocation = WindowStartupLocation.Manual;

            ////// 根据鼠标位置动态显示弹窗
            //var winPos = GetWinPosition(_popupWindow);
            //_popupWindow.Left = winPos.popupLeft - 320;
            //_popupWindow.Top = winPos.popupTop - 280;

            ////var position = e.GetPosition(this);
            ////_popupWindow.Left = position.X + winPos.popupLeft - 320; // 偏移量
            ////_popupWindow.Top = position.Y + winPos.popupTop - 280;

            //_popupWindow.Show();
        }

        private (double popupLeft, double popupTop) GetWinPosition(Window popupWindow)
        {

            // 获取鼠标位置（相对于屏幕坐标）
            System.Windows.Point mousePosition = System.Windows.Input.Mouse.GetPosition(Application.Current.MainWindow);
            mousePosition = Application.Current.MainWindow.PointToScreen(mousePosition);

            // 获取主屏幕的工作区
            var screenWidth = SystemParameters.WorkArea.Width;
            var screenHeight = SystemParameters.WorkArea.Height;


            // 计算弹窗位置，确保弹窗不会超出屏幕
            double popupLeft = mousePosition.X;
            double popupTop = mousePosition.Y;

            if (popupLeft + popupWindow.Width > screenWidth)
            {
                popupLeft = screenWidth - popupWindow.Width;
            }
            if (popupTop + popupWindow.Height > screenHeight)
            {
                popupTop = screenHeight - popupWindow.Height;
            }

            //// 如果弹窗超出屏幕右边缘，则调整到左侧
            //if (popupLeft + popupWindow.Width > workingArea.Right)
            //{
            //    popupLeft = workingArea.Right - popupWindow.Width;
            //}

            //// 如果弹窗超出屏幕下边缘，则调整到上方
            //if (popupTop + popupWindow.Height > workingArea.Bottom)
            //{
            //    popupTop = workingArea.Bottom - popupWindow.Height;
            //}

            //// 如果弹窗超出屏幕左边缘，则调整到右侧
            //if (popupLeft < workingArea.Left)
            //{
            //    popupLeft = workingArea.Left;
            //}

            //// 如果弹窗超出屏幕顶部，则调整到下方
            //if (popupTop < workingArea.Top)
            //{
            //    popupTop = workingArea.Top;
            //}

            return (popupLeft, popupTop);
        }

        #endregion

        #region 页面动画效果
        /// <summary>
        /// 心跳灯颜色动画
        /// </summary>
        private Storyboard _storyboardHeartbeat = null;
        /// <summary>
        /// 手自动灯颜色动画
        /// </summary>
        private Storyboard _storyboardModeLamp = null;


        private void SetHeartbeatAnimationColor(Color color)
        {
            var rect = (Ellipse)this.Template.FindName("HeartbeatLamp", this);//保持灯绑定控件
            if (rect != null)
            {
                _storyboardModeLamp = SetFlickerAnimation(_storyboardModeLamp, color, new SolidColorBrush(color), rect);

                _storyboardModeLamp.Stop();
                _storyboardModeLamp.Begin();
            }

        }

        /// <summary>
        /// 手自动灯切换
        /// </summary>
        /// <param name="color"></param>
        private void SetModeLampAnimationColor(Color color)
        {
            var rect = (Ellipse)this.Template.FindName("ModeLamp", this);//保持灯绑定控件
            if (rect != null)
            {
                rect.Fill = new SolidColorBrush(color);
                //_storyboardModeLamp = SetKeepAnimation(_storyboardModeLamp, color, new SolidColorBrush(color), rect);
                //_storyboardModeLamp.Stop();
                //_storyboardModeLamp.Begin();
            }

        }

        /// <summary>
        /// 设置保持动画
        /// </summary>
        /// <param name="storyboard"></param>
        /// <param name="fromColor"></param>
        /// <param name="colorBrush"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private Storyboard SetKeepAnimation(Storyboard storyboard, Color fromColor, SolidColorBrush colorBrush, Ellipse rect)
        {
            // 确保 Fill 是一个 SolidColorBrush
            //if (!(rect.Fill is SolidColorBrush fillBrush))
            //{
            //    //fillBrush = new SolidColorBrush(colorBrush);
            //    fillBrush = colorBrush;
            //    rect.Fill = fillBrush;
            //}

            // 创建故事板
            if (storyboard == null)
            {
                storyboard = new Storyboard();
            }

            // 创建颜色保持动画
            //var colorAnimation = new ObjectAnimationUsingKeyFrames();



            //// 创建颜色渐变动画
            var colorAnimation = new ObjectAnimationUsingKeyFrames
            {
                //Duration = TimeSpan.FromSeconds(10),
                //RepeatBehavior = RepeatBehavior.Forever
                FillBehavior = FillBehavior.HoldEnd
            };

            // 设置关键帧：颜色保持不变
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
                Value = fromColor
            });

            //// 第一帧：设置为开始颜色
            //colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame
            //{
            //    KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
            //    Value = fromColor
            //});

            //// 第二帧：设置为结束颜色
            //colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame
            //{
            //    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.7)),
            //    //Value = Colors.Transparent
            //    Value = fromColor
            //});

            // 设置动画目标和目标属性
            Storyboard.SetTarget(colorAnimation, rect);
            //Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(Shape.Fill).(SolidColorBrush.Color)"));
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(Shape.Fill).(SolidColorBrush.Color)"));

            // 将动画添加到故事板
            storyboard.Children.Clear();
            storyboard.Children.Add(colorAnimation);

            return storyboard;
        }

        /// <summary>
        /// 设置闪烁动画
        /// </summary>
        /// <param name="colorBrush"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private Storyboard SetFlickerAnimation(Storyboard storyboard, Color fromColor, SolidColorBrush colorBrush, Ellipse rect)
        {
            // 确保 Fill 是一个 SolidColorBrush
            //if (!(rect.Fill is SolidColorBrush fillBrush))
            //{
            //    //fillBrush = new SolidColorBrush(colorBrush);
            //    fillBrush = colorBrush;
            //    rect.Fill = fillBrush;
            //}

            // 创建故事板
            if (storyboard == null)
            {
                storyboard = new Storyboard();
            }

            // 创建颜色渐变动画
            var colorAnimation = new ColorAnimation
            {
                From = fromColor,              // 开始颜色
                To = Colors.Transparent,        // 结束颜色
                Duration = TimeSpan.FromSeconds(0.7), // 持续时间
                AutoReverse = true,             // 自动反转
                RepeatBehavior = RepeatBehavior.Forever // 永久重复
            };

            // 设置动画目标和目标属性
            Storyboard.SetTarget(colorAnimation, rect);
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(Shape.Fill).(SolidColorBrush.Color)"));

            // 将动画添加到故事板
            storyboard.Children.Clear();
            storyboard.Children.Add(colorAnimation);

            return storyboard;
        }

        #endregion
    }
}
