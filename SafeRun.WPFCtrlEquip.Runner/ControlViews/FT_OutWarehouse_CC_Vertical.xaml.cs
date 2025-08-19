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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SafeRun.WPFCtrlEquip.WPFRunner.ControlViews
{
    /// <summary>
    /// FT_InWarehouse_CC.xaml 的交互逻辑
    /// </summary>
    public partial class FT_OutWarehouse_CC_Vertical : BasePageClass
    {
        public FT_OutWarehouse_CC_Vertical()
        {
            InitializeComponent();
        }

        #region 实现按中心鼠标滚轮缩放
        /*配套页面代码:
        <Grid Name = "contentGrid" MouseWheel="ContentGrid_MouseWheel" MouseLeftButtonDown="ContentGrid_MouseLeftButtonDown">
        <Grid.RenderTransform>
            <ScaleTransform x:Name="scaleTransform" ScaleX="1" ScaleY="1"/>
        </Grid.RenderTransform>
        </Grid>
        */

        //鼠标点击中心点
        Point clickPosition;

        private void ContentGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 每次滚动的缩放因子
            double zoomFactor = 0.1;

            // 获取当前的缩放值
            double currentScaleX = scaleTransform.ScaleX;
            double currentScaleY = scaleTransform.ScaleY;

            // 根据鼠标滚轮的方向调整缩放值
            if (e.Delta > 0)
            {
                // 放大
                scaleTransform.ScaleX = currentScaleX + zoomFactor;
                scaleTransform.ScaleY = currentScaleY + zoomFactor;
            }
            else
            {
                // 缩小，但不允许缩小到小于0.1倍
                //if (currentScaleX > zoomFactor && currentScaleY > zoomFactor)
                if (currentScaleX > 1 && currentScaleY > 1)
                {
                    scaleTransform.ScaleX = currentScaleX - zoomFactor;
                    scaleTransform.ScaleY = currentScaleY - zoomFactor;
                }
            }

            // 获取鼠标当前位置
            //Point mousePosition = e.GetPosition(contentGrid);
            Point mousePosition = clickPosition;

            // 计算缩放中心点
            contentGrid.RenderTransformOrigin = new Point(mousePosition.X / contentGrid.ActualWidth, mousePosition.Y / contentGrid.ActualHeight);
        }
        //获取鼠标点击位置
        private void ContentGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 获取鼠标点击位置
            //Point clickPosition = e.GetPosition(contentGrid);
            clickPosition = e.GetPosition(contentGrid);

            // 设置RenderTransformOrigin为鼠标点击位置
            contentGrid.RenderTransformOrigin = new Point(clickPosition.X / contentGrid.ActualWidth, clickPosition.Y / contentGrid.ActualHeight);
        }
        #endregion
    }
}
