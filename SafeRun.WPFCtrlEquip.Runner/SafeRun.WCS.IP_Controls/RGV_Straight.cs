using SafeRun.WCS.IP_Controls.ControlPage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace SafeRun.WCS.IP_Controls
{
    /// <summary>
    /// 小车直轨段
    /// <IP_Controls:RGV_Straight Canvas.Left="645" Canvas.Top="138" RGVRBarcodeMax ="5000" RGVRBarcodeMin="1" RGVRRegionHead="1" RGVRRegionTail="1000" TagReadText="Show:RGV1#RgvBarcode1+RgvTask1+RgvState1+RgvNo1,RGV2#RgvBarcode2+RgvTask2+RgvState2+RgvNo2" HorizontalAlignment="Center" VerticalAlignment="Top"/>
    /// 
    /// 
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SafeRun.WCS.IP_Controls.Themes"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SafeRun.WCS.IP_Controls.Themes;assembly=SafeRun.WCS.IP_Controls.Themes"
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
    ///     <MyNamespace:RGV_Straight/>
    ///
    /// </summary>
    public class RGV_Straight : HMIControlBase
    {
        
        static RGV_Straight()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RGV_Straight), new FrameworkPropertyMetadata(typeof(RGV_Straight)));
        }

        private RGV_Cars RGVCar = null;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // 在模板中查找名为 "RGVCar" 的子级元素
            RGVCar = GetTemplateChild("RGVCar") as RGV_Cars;
            if(RGVCar != null) { RGVCar.Visibility = Visibility.Hidden; }

        }


        #region 设置自定义控件属性值

        #region 条码最大最小值
        public static readonly DependencyProperty RGVRBarcodeMaxProperty = DependencyProperty.Register("RGVRBarcodeMax", typeof(int), typeof(RGV_Straight));

        /// <summary>
        /// RGV条码最大值
        /// </summary>
        [Category("HMIC")]
        public int RGVRBarcodeMax
        {
            get { return (int)GetValue(RGVRBarcodeMaxProperty); }
            set { SetValue(RGVRBarcodeMaxProperty, value); }
        }

        public static readonly DependencyProperty RGVRBarcodeMinProperty = DependencyProperty.Register("RGVRBarcodeMin", typeof(int), typeof(RGV_Straight));
        /// <summary>
        /// rgv条码最小值
        /// </summary>
        [Category("HMIC")]
        public int RGVRBarcodeMin
        {
            get { return (int)GetValue(RGVRBarcodeMinProperty); }
            set { SetValue(RGVRBarcodeMinProperty, value); }
        }
        #endregion

        #region 区域范围
        public static readonly DependencyProperty RGVRRegionHeadProperty = DependencyProperty.Register("RGVRRegionHead", typeof(int), typeof(RGV_Straight));

        /// <summary>
        /// rgv条码范围头
        /// </summary>
        [Category("HMIC")]
        public int RGVRRegionHead
        {
            get { return (int)GetValue(RGVRRegionHeadProperty); }
            set { SetValue(RGVRRegionHeadProperty, value); }
        }

        public static readonly DependencyProperty RGVRRegionTailProperty = DependencyProperty.Register("RGVRRegionTail", typeof(int), typeof(RGV_Straight));
        /// <summary>
        /// rgv条码范围尾
        /// </summary>
        [Category("HMIC")]
        public int RGVRRegionTail
        {
            get { return (int)GetValue(RGVRRegionTailProperty); }
            set { SetValue(RGVRRegionTailProperty, value); }
        }
        #endregion


        #region 小车编号 做轨道判断使用，不用设置
        private static readonly DependencyProperty RGVRNoProperty = DependencyProperty.Register("RGVRNo", typeof(string), typeof(RGV_Straight));

        /// <summary>
        /// 做轨道判断使用，不用设置
        /// </summary>
        [Category("HMIC")]
        private string RGVRNo
        {
            get { return (string)GetValue(RGVRNoProperty); }
            set { SetValue(RGVRNoProperty, value); }
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
                var tempContrName = contrName;
                if (contrName.Contains("RGV")) tempContrName = "RGVInfo";

                switch (tempContrName)
                {
                    case "RGVInfo":
                        //SetRgvInfo(contrVal);
                        SetRgvsInfo(contrVal);
                        break;
                    case "":
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

        #region 设置轨道上小车信息

        #region 多小车轨道显示
        private class rgvItem
        {
            public string RGVBarcode { get; set; }
            public string RGVTaskNo { get; set; }
            public string RGVState { get; set; }
            public string RGVNo { get; set; }
        }

        private List<rgvItem> rgvItems { get; set; } = new List<rgvItem>();

        /// <summary>
        /// 设置轨道上显示的小车信息
        /// </summary>
        /// <param name="contrVal"></param>
        private void SetRgvsInfo(string contrVal)
        {
            if (string.IsNullOrEmpty(contrVal)) return;
            //字符串格式: 当前条码号+任务号+状态+小车编号
            string[] rgvInfos = contrVal.Split('+');
            if (rgvInfos.Length < 4) return;
            var barcode = rgvInfos[0];//当前小车条码
            var taskNo = rgvInfos[1];//当前小车任务号
            var state = rgvInfos[2];//小车状态
            var rgvNo = rgvInfos[3];//小车编号

            if (string.IsNullOrEmpty(rgvNo)) return;

            //1.校验数据是否存在范围内,不存在就移除
            var rgvToRemove = new List<rgvItem>();
            foreach (var item in rgvItems)
            {
                if(item.RGVNo != rgvNo) continue;
                if (IsExistRegion(barcode)) continue;
                rgvToRemove.Add(item);
            }
            //1_2.移除不在范围内的数据
            foreach (var item in rgvToRemove)
            {
                if (RGVCar.DelRGVItem(item.RGVNo))
                {
                    rgvItems.Remove(item);
                }
                
            }
            //1_3.无需要显示的数据，则不显示小车
            if(rgvItems.Count <= 0 && RGVCar.Visibility != Visibility.Hidden)
            {
                RGVCar.Visibility = Visibility.Hidden;
            }

            
            //2.校验数据是否存在，存在就不添加 && 校验条码是否在范围内
            if (!IsExistRegion(barcode))
            {
                return;
            }

            //2_1.修改数据
            if (rgvItems.Exists(m => m.RGVNo == rgvNo) )
            {
                RGVCar.UpdRGVItem(rgvNo, state);
                return;
            }

            //2_2.添加数据
            if (RGVCar.AddRGVItem(rgvNo, state))
            {
                rgvItems.Add(new rgvItem() { RGVBarcode = barcode, RGVTaskNo = taskNo, RGVState = state, RGVNo = rgvNo });
            }

            //2_3.设置显示小车
            if (rgvItems.Count > 0 && RGVCar.Visibility != Visibility.Visible)
            {
                RGVCar.Visibility = Visibility.Visible;
            }

        }

        public override void Control_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.Control_MouseRightButtonDown(sender, e);//调用父类的实现方法

            if (rgvItems.Count <= 0) return;

            var rgvInfoList = new List<(string RGVNo, string TaskNo, string Barcode, string Status)>();

            foreach (var item in rgvItems)
            {
                rgvInfoList.Add((item.RGVNo, item.RGVTaskNo, item.RGVBarcode, item.RGVState));
            }

            _popupWindow = new PopupWindow(rgvInfoList);
            //_popupWindow.Owner = this; // 设置当前窗口为所有者窗口
            _popupWindow.WindowStartupLocation = WindowStartupLocation.Manual;

            //// 根据鼠标位置动态显示弹窗
            var winPos = GetWinPosition(_popupWindow);
            _popupWindow.Left = winPos.popupLeft;
            _popupWindow.Top = winPos.popupTop;

            _popupWindow.Show();
        }


        /// <summary>
        /// 设置轨道上显示的小车信息
        /// </summary>
        /// <param name="contrVal"></param>
        private void SetRgvInfo(string contrVal)
        {
            if (string.IsNullOrEmpty(contrVal)) return;
            //字符串格式: 当前条码号+任务号+状态+小车编号
            string[] rgvInfos = contrVal.Split('+');
            if (rgvInfos.Length < 4) return;
            var barcode = rgvInfos[0];//当前小车条码
            var taskNo = rgvInfos[1];//当前小车任务号
            var state = rgvInfos[2];//小车状态
            var rgvNo = rgvInfos[3];//小车编号

            if (!IsExistRegion(barcode))
            {
                if (RGVRNo == rgvNo)
                {
                    InitCotrInfo();
                }
                return;
            }

            RGVRNo = rgvNo;
            //RGVCar.RGVBarcode = barcode;
            //RGVCar.RGVTaskNo = taskNo;
            ////RGVCar.RGVState = state;
            //RGVCar.RGVNo = rgvNo;
            ////设置显示小车图片
            //if(RGVCar.Visibility != Visibility.Visible)
            //{
            //    RGVCar.Visibility = Visibility.Visible;
            //}
            
        }

        /// <summary>
        /// 是否存在区域范围内
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private bool IsExistRegion(string barcode)
        {
            if (string.IsNullOrEmpty(barcode)) return false;
            int barcodeMax = RGVRBarcodeMax;
            int barcodeMin = RGVRBarcodeMin;
            int regionHead = RGVRRegionHead;
            int regionTail = RGVRRegionTail;
            int bar = int.Parse(barcode);
            var result = false;
            if (regionTail < regionHead)
            {
                if ((regionHead <= bar && bar <= barcodeMax) || (barcodeMin <= bar && bar <= regionTail)) result = true;
            }
            else
            {
                if (regionHead <= bar && bar <= regionTail) result = true;
            }

            return result;
        }

        private void InitCotrInfo()
        {
            //RGVRNo = "";
            //RGVCar.RGVBarcode = "";
            //RGVCar.RGVTaskNo = "";
            ////RGVCar.RGVState = "";
            //RGVCar.RGVNo = "";
            ////设置不显示小车图片
            //if (RGVCar.Visibility != Visibility.Hidden)
            //{
            //    RGVCar.Visibility = Visibility.Hidden;
            //}
        }
        #endregion

        #endregion



    }
}
