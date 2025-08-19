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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SafeRun.WPFCtrlEquip.WPFRunner.ControlViews
{
    /// <summary>
    /// FT_FullLayout_CC.xaml 的交互逻辑
    /// </summary>
    public partial class FT_FullLayout_CC : BasePageClass
    {
        //public FT_FullLayout_CC()
        //{
        //    InitializeComponent();
        //}

        private Window ParentThat = null;
        public FT_FullLayout_CC(Window that)
        {
            InitializeComponent();
            ParentThat = that;
        }
        public FT_FullLayout_CC()
        {
            InitializeComponent();
        }

        private void BasePageClass_Loaded(object sender, RoutedEventArgs e)
        {
            ////// 获取窗口句柄
            //var hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(ParentThat).Handle);
            //hwndSource.AddHook(WndProc); // 订阅消息处理
        }

        #region 实现按中心鼠标滚轮缩放

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_MOUSEWHEEL = 0x020A; // 鼠标滚轮消息

            if (msg == WM_MOUSEWHEEL)
            {
                int delta = (short)((wParam.ToInt64() >> 16) & 0xFFFF);
                var potint = Mouse.GetPosition(contentGrid);
                if (delta > 0)
                {
                    //鼠标滚轮向上滚动
                    HandleMouseWheel(scaleTransform, potint, 0);
                }
                else
                {
                    //鼠标滚轮向下滚动
                    HandleMouseWheel(scaleTransform, potint, 1);
                }

                handled = true; // 标记事件已处理
            }

            return IntPtr.Zero; // 返回默认处理
        }



        /*配套页面代码:
        <Grid Name = "contentGrid" MouseWheel="ContentGrid_MouseWheel" MouseLeftButtonDown="ContentGrid_MouseLeftButtonDown">
        <Grid.RenderTransform>
            <ScaleTransform x:Name="scaleTransform" ScaleX="1" ScaleY="1"/>
        </Grid.RenderTransform>
        </Grid>
        */

        private void ContentGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {


            var potint = Mouse.GetPosition(contentGrid);
            // 根据鼠标滚轮的方向调整缩放值
            if (e.Delta > 0)
            {
                // 放大
                //鼠标滚轮向上滚动
                HandleMouseWheel(scaleTransform, potint, 0);
            }
            else
            {
                // 缩小，但不允许缩小到小于0.1倍
                //鼠标滚轮向下滚动
                HandleMouseWheel(scaleTransform, potint, 1);

            }

        }

        private void HandleMouseWheel(ScaleTransform scaleTransform, Point mousePosition,int forwardOrBackward)
        {
            // 每次滚动的缩放因子
            double zoomFactor = 0.1;

            // 获取当前的缩放值
            double currentScaleX = scaleTransform.ScaleX;
            double currentScaleY = scaleTransform.ScaleY;

            // 根据鼠标滚轮的方向调整缩放值
            if (forwardOrBackward == 0)//鼠标滚轮向前滚动
            {
                // 放大
                scaleTransform.ScaleX = currentScaleX + zoomFactor;
                scaleTransform.ScaleY = currentScaleY + zoomFactor;
            }
            else if (forwardOrBackward == 1)//鼠标滚轮向后滚动
            {
                // 缩小，但不允许缩小到小于0.1倍
                if (currentScaleX > 0.5 && currentScaleY > 0.5)
                {
                    scaleTransform.ScaleX = currentScaleX - zoomFactor;
                    scaleTransform.ScaleY = currentScaleY - zoomFactor;
                }

            }

            // 计算缩放中心点
            contentGrid.RenderTransformOrigin = new Point(mousePosition.X / contentGrid.ActualWidth, mousePosition.Y / contentGrid.ActualHeight);
        }

        #endregion

        #region 鼠标左键+左Ctrl键拖动

        private bool _isDragging = false;          // 是否正在拖动
        private Point _lastMousePosition;          // 上一次鼠标位置
        private Point _originTranslate;            // 拖动起始点的偏移量

        // 鼠标左键按下开始拖动
        private void ContentGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //_isDragging = true;
            //_lastMousePosition = e.GetPosition(this); // 记录鼠标起始位置
            //_originTranslate = new Point(translateTransform.X, translateTransform.Y); // 记录起始偏移
            //this.Cursor = Cursors.Hand; // 更改鼠标样式
            // 检查是否按下了 Ctrl 键
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                _isDragging = true;
                _lastMousePosition = e.GetPosition(this); // 记录鼠标起始位置
                _originTranslate = new Point(translateTransform.X, translateTransform.Y); // 记录起始偏移
                this.Cursor = Cursors.Hand; // 更改鼠标样式
            }
        }

        // 鼠标左键松开结束拖动
        private void ContentGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            this.Cursor = Cursors.Arrow; // 恢复鼠标样式
        }

        // 鼠标移动时拖动
        private void ContentGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point currentMousePosition = e.GetPosition(this); // 获取当前鼠标位置
                Vector delta = currentMousePosition - _lastMousePosition; // 计算位移向量

                // 更新偏移
                translateTransform.X = _originTranslate.X + delta.X;
                translateTransform.Y = _originTranslate.Y + delta.Y;
            }
        }

        #endregion
    }
}
