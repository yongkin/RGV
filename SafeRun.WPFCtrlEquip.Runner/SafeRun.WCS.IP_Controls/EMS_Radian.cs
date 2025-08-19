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
    /// EMS 曲线部分处理
    /// <IP_Controls:EMS_Radian Canvas.Left="645" Canvas.Top="138" EMSRBarcodeMax ="5000" EMSRBarcodeMin="1" EMSRRegionHead="1" EMSRRegionTail="1000" TagReadText="Show:EMS1#EMSBarcode1+EMSTask1+EMSState1+EMSNo1,EMS2#EMSBarcode2+EMSTask2+EMSState2+EMSNo2" HorizontalAlignment="Center" VerticalAlignment="Top"/>
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
    ///     <MyNamespace:EMS_Radian/>
    ///
    /// </summary>
    public class EMS_Radian : HMIControlBase
    {
        static EMS_Radian()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EMS_Radian), new FrameworkPropertyMetadata(typeof(EMS_Radian)));
        }

        private EMS_Car EMSCar = null;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // 在模板中查找名为 "EMSCar" 的子级元素
            EMSCar = GetTemplateChild("EMSCar") as EMS_Car;
            if (EMSCar != null) { EMSCar.Visibility = Visibility.Hidden; }

        }


        #region 设置自定义控件属性值

        #region 条码最大最小值
        public static readonly DependencyProperty EMSRBarcodeMaxProperty = DependencyProperty.Register("EMSRBarcodeMax", typeof(int), typeof(EMS_Radian));

        /// <summary>
        /// EMS条码最大值
        /// </summary>
        [Category("HMIC")]
        public int EMSRBarcodeMax
        {
            get { return (int)GetValue(EMSRBarcodeMaxProperty); }
            set { SetValue(EMSRBarcodeMaxProperty, value); }
        }

        public static readonly DependencyProperty EMSRBarcodeMinProperty = DependencyProperty.Register("EMSRBarcodeMin", typeof(int), typeof(EMS_Radian));
        /// <summary>
        /// EMS条码最小值
        /// </summary>
        [Category("HMIC")]
        public int EMSRBarcodeMin
        {
            get { return (int)GetValue(EMSRBarcodeMinProperty); }
            set { SetValue(EMSRBarcodeMinProperty, value); }
        }
        #endregion

        #region 区域范围
        public static readonly DependencyProperty EMSRRegionHeadProperty = DependencyProperty.Register("EMSRRegionHead", typeof(int), typeof(EMS_Radian));

        /// <summary>
        /// EMS条码范围头
        /// </summary>
        [Category("HMIC")]
        public int EMSRRegionHead
        {
            get { return (int)GetValue(EMSRRegionHeadProperty); }
            set { SetValue(EMSRRegionHeadProperty, value); }
        }

        public static readonly DependencyProperty EMSRRegionTailProperty = DependencyProperty.Register("EMSRRegionTail", typeof(int), typeof(EMS_Radian));
        /// <summary>
        /// EMS条码范围尾
        /// </summary>
        [Category("HMIC")]
        public int EMSRRegionTail
        {
            get { return (int)GetValue(EMSRRegionTailProperty); }
            set { SetValue(EMSRRegionTailProperty, value); }
        }
        #endregion

        #region 小车编号
        public static readonly DependencyProperty EMSRNoProperty = DependencyProperty.Register("EMSRNo", typeof(string), typeof(EMS_Radian));

        [Category("HMIC")]
        public string EMSRNo
        {
            get { return (string)GetValue(EMSRNoProperty); }
            set { SetValue(EMSRNoProperty, value); }
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
                if (contrName.Contains("EMS")) tempContrName = "EMSInfo";

                switch (tempContrName)
                {
                    case "EMSInfo":
                        SetEMSInfo(contrVal);
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

        #region 设置轨道上小车信息
        /// <summary>
        /// 设置轨道上显示的小车信息
        /// </summary>
        /// <param name="contrVal"></param>
        private void SetEMSInfo(string contrVal)
        {
            if (string.IsNullOrEmpty(contrVal)) return;
            //字符串格式: 当前条码号+任务号+状态+小车编号
            string[] EMSInfos = contrVal.Split('+');
            if (EMSInfos.Length < 4) return;
            var barcode = EMSInfos[0];//当前小车条码
            var taskNo = EMSInfos[1];//当前小车任务号
            var state = EMSInfos[2];//小车状态
            var EMSNo = EMSInfos[3];//小车编号

            if (int.Parse(barcode) == 2000)
            {

            }

            if (!IsExistRegion(barcode))
            {
                if (EMSRNo == EMSNo)
                {
                    InitCotrInfo();
                }
                return;
            }

            EMSRNo = EMSNo;
            EMSCar.EMSBarcode = barcode;
            EMSCar.EMSTaskNo = taskNo;
            //EMSCar.EMSState = state;
            EMSCar.EMSNo = EMSNo;
            //设置显示小车图片
            if (EMSCar.Visibility != Visibility.Visible)
            {
                EMSCar.Visibility = Visibility.Visible;
            }

        }

        /// <summary>
        /// 是否存在区域范围内
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private bool IsExistRegion(string barcode)
        {
            if (string.IsNullOrEmpty(barcode)) return false;
            int barcodeMax = EMSRBarcodeMax;
            int barcodeMin = EMSRBarcodeMin;
            int regionHead = EMSRRegionHead;
            int regionTail = EMSRRegionTail;
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
            EMSRNo = "";
            EMSCar.EMSBarcode = "";
            EMSCar.EMSTaskNo = "";
            //EMSCar.EMSState = "";
            EMSCar.EMSNo = "";
            //设置不显示小车图片
            if (EMSCar.Visibility != Visibility.Hidden)
            {
                EMSCar.Visibility = Visibility.Hidden;
            }
        }
        #endregion

        #endregion
    }
}
