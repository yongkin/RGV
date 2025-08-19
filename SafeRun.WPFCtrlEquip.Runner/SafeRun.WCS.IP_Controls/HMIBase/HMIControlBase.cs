
using MahApps.Metro.Controls;
using SafeRun.WCS.IP_Controls.ControlPage;
using System.Windows;
using System.Windows.Shapes;

namespace SafeRun.WCS.IP_Controls
{
    public class HMIControlBase : Control, ITagReader
    {
        public static PopupWindow _popupWindow; // 当前弹出的窗口
        public HMIControlBase()
            : base()
        {
            DefaultStyleKey = typeof(HMIControlBase);
            this.MouseRightButtonDown += Control_MouseRightButtonDown;
        }

        // 创建报警开启动画

        #region 设置自定义控件属性值

        #region 控件背景色
        public static readonly DependencyProperty ContrBackgroundProperty =
            DependencyProperty.Register("ContrBackground", typeof(string), typeof(HMIControlBase));
        [Category("HMIC")]
        public string ContrBackground
        {
            get { return ((string)base.GetValue(ContrBackgroundProperty)); }
            set { base.SetValue(ContrBackgroundProperty, value); }
        }

        #endregion

        #region 显示控件信息(鼠标移动到控件上)
        public static readonly DependencyProperty ShowTitleProperty =
            DependencyProperty.Register("ShowTitle", typeof(string), typeof(HMIControlBase));
        [Category("HMIC")]
        public string ShowTitle
        {
            get { return ((string)base.GetValue(ShowTitleProperty)); }
            set { base.SetValue(ShowTitleProperty, value); }
        }

        public static readonly DependencyProperty ShowContentProperty =
            DependencyProperty.Register("ShowContent", typeof(string), typeof(HMIControlBase));
        [Category("HMIC")]
        public string ShowContent
        {
            get { return ((string)base.GetValue(ShowContentProperty)); }
            set { base.SetValue(ShowContentProperty, value); }
        }

        #endregion

        #region 绑定WMS上的编号
        public static readonly DependencyProperty SystemNoProperty =
            DependencyProperty.Register("SystemNo", typeof(string), typeof(HMIControlBase));
        [Category("HMIC")]
        public string SystemNo
        {
            get { return ((string)base.GetValue(SystemNoProperty)); }
            set { base.SetValue(SystemNoProperty, value); }
        }

        
        #endregion

        #region 委托事件
        public Func<Dictionary<string, string>, List<(string key, string val)>> UpdAction { get; set; }
        public Func<Dictionary<string, string>, List<(string key, string val)>> DeleteAction { get; set; }
        public Func<Dictionary<string, string>, List<(string key, string val)>> CompleteAction { get; set; }
        public Func<Dictionary<string, string>, List<(string key, string val)>> QueryAction { get; set; }

        #endregion

        #endregion


        #region ITagReader接口实现
        public static readonly DependencyProperty TagReadTextProperty =
            DependencyProperty.Register("TagReadText", typeof(string), typeof(HMIControlBase));
        [Category("HMIC")]
        public string TagReadText
        {
            get { return ((string)base.GetValue(TagReadTextProperty)); }
            set { base.SetValue(TagReadTextProperty, value); }
        }

        private List<ShowTagEnt> _ShowTags;
        public List<ShowTagEnt> ShowTags { get => _ShowTags; set => _ShowTags = value; }

        private List<TagLinkEnt> children = new List<TagLinkEnt>();
        public List<TagLinkEnt> Children
        {
            get { return children; }
            set { children = value; }
        }
        public string Node
        {
            get { return this.Name; }
        }


        public virtual bool SetTagReader(string key, string content)
        {

            switch (key)
            {
                case TagActions.RUN:
                    //Run:aaa,bbb
                    foreach (var node in content.Split(','))
                    {
                        Children.Add(new TagLinkEnt() { Node = node });
                    }
                    break;

                case TagActions.SHOW:
                    this.ShowTags = new List<ShowTagEnt>();
                    //格式:Show:aaa#123+222,bbb#222
                    foreach (var node in content.Split(','))
                    {
                        var items = node.Split("#");
                        if (items.Length < 2) continue;

                        var tagNodes = new List<string>();
                        foreach (var nodeItem in items[1].Split('+'))
                        {
                            tagNodes.Add(nodeItem);
                        }

                        ShowTags.Add(new ShowTagEnt() { ContrName = items[0], NodeNames = tagNodes });
                    }
                    break;
            }
            InitAlarmAnimation();
            return true;
        }

        public virtual Action SetShowControls(string contrName, string contrVal)
        {

            switch (contrName)
            {
                case "Alarm":
                    if (contrVal == "1")
                    {
                        //VisualStateManager.GoToState(this, "AlarmOn", true);
                        SetAlarmOnAnimation();
                    }
                    else
                    {
                        //VisualStateManager.GoToState(this, "AlarmOff", true);
                        SetAlarmOffAnimation();
                    }

                    break;

                case "AutoMode":
                    ContrBackground = contrVal == "1" ? ColorLibrary.GetColorStr(ColorLibrary.AutoModeColor) : ColorLibrary.GetColorStr(ColorLibrary.MoveModeColor);
                    break;

            }

            return null;
        }

        /// <summary>
        /// 解析数据，满足控件使用要求
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="contrName"></param>
        /// <param name="contrVal"></param>
        /// <returns></returns>
        public virtual string DataDisassembly(string typeName, string contrName, string contrVal)
        {
            return contrVal;
        }
        #endregion

        #region 报警动画处理

        private Storyboard _alarmOnStoryboard = null;
        private Storyboard _alarmOffStoryboard = null;

        public void SetAlarmOnAnimation()
        {
            VisualStateManager.GoToState(this, "AlarmOn", true);
            if (_alarmOnStoryboard == null) InitAlarmAnimation();
            _alarmOnStoryboard.Begin();
        }

        public void SetAlarmOffAnimation()
        {
            VisualStateManager.GoToState(this, "AlarmOff", true);
            if (_alarmOffStoryboard == null) InitAlarmAnimation();
            _alarmOffStoryboard.Begin();
        }

        private void InitAlarmAnimation()
        {
            var rect = (Rectangle)this.Template.FindName("rect", this);
            _alarmOffStoryboard = CreateAlarmAnimation(ColorLibrary.AlarmDefaultColor, rect);
            _alarmOnStoryboard = CreateAlarmAnimation(ColorLibrary.AlarmColorB, rect);
        }

        private Storyboard CreateAlarmAnimation(SolidColorBrush colorBrushs, Rectangle rect)
        {
            Storyboard storyboard = new Storyboard();

            // 创建颜色变化的动画
            ObjectAnimationUsingKeyFrames colorAnimation = new ObjectAnimationUsingKeyFrames
            {
                Duration = TimeSpan.FromSeconds(0.5)
            };
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
                Value = colorBrushs // Alarm Color
            });
            //var rect = (Rectangle)this.Template.FindName("rect", this);
            Storyboard.SetTarget(colorAnimation, rect);
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(Shape.FillProperty));

            storyboard.Children.Add(colorAnimation);
            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            return storyboard;
            //storyboard.Begin();
        }


        #endregion

        #region 鼠标右击事件

        public virtual void Control_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // 如果已有弹窗，先关闭
            if (_popupWindow != null)
            {
                _popupWindow.Close();
                _popupWindow = null;
            }

        }

        public virtual void RightClickFrom(Dictionary<string, string> showParam, Dictionary<string, string> operNodes,
            Func<Dictionary<string, string>, List<(string key, string val)>> updAction,
            Func<Dictionary<string, string>, List<(string key, string val)>> deleteAction,
            Func<Dictionary<string, string>, List<(string key, string val)>> completeAction,
            Func<Dictionary<string, string>, List<(string key, string val)>> queryAction
            )
        {
            _popupWindow = new PopupWindow(
                showParam,
                operNodes,
                UpdAction, DeleteAction, CompleteAction, QueryAction);

            
            // 获取当前屏幕的宽高（支持多显示器）
            var screen = System.Windows.Forms.Screen.PrimaryScreen;
            var screenWidth = screen.WorkingArea.Width;
            var screenHeight = screen.WorkingArea.Height;

            //如果你希望窗口尺寸自适应内容，但仍然居中，可以先 Show()，再设置位置：
            _popupWindow.Show();

            _popupWindow.Left = (SystemParameters.PrimaryScreenWidth - _popupWindow.ActualWidth) / 2;
            _popupWindow.Top = (SystemParameters.PrimaryScreenHeight - _popupWindow.ActualHeight) / 2;
            

            /*
            //根据鼠标位置动态显示弹窗
            _popupWindow.Show();
            //_popupWindow.Owner = this; // 设置当前窗口为所有者窗口
            _popupWindow.WindowStartupLocation = WindowStartupLocation.Manual;

            //// 根据鼠标位置动态显示弹窗
            var winPos = GetWinPosition(_popupWindow);
            //_popupWindow.Left = winPos.popupLeft - 320;
            _popupWindow.Left = winPos.popupLeft - _popupWindow.ActualWidth + 110;
            //_popupWindow.Top = winPos.popupTop - 280;
            _popupWindow.Top = winPos.popupTop - _popupWindow.ActualHeight + 30;*/
        }

        public (double popupLeft, double popupTop) GetWinPosition(Window popupWindow)
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
    }
}
