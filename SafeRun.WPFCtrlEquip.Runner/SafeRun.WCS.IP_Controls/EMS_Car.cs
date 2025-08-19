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
    /// EMS小车控件
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
    ///     <MyNamespace:EMS_Car/>
    ///
    /// </summary>
    public class EMS_Car : HMIControlBase
    {
        static EMS_Car()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EMS_Car), new FrameworkPropertyMetadata(typeof(EMS_Car)));
        }

        #region 设置自定义控件属性值


        #region 当前条码号
        public static readonly DependencyProperty EMSBarcodeProperty = DependencyProperty.Register("EMSBarcode", typeof(string), typeof(EMS_Car));

        [Category("HMIC")]
        public string EMSBarcode
        {
            get { return (string)GetValue(EMSBarcodeProperty); }
            set { SetValue(EMSBarcodeProperty, value); }
        }
        #endregion

        #region 任务号
        public static readonly DependencyProperty EMSTaskNoProperty = DependencyProperty.Register("EMSTaskNo", typeof(string), typeof(EMS_Car));

        [Category("HMIC")]
        public string EMSTaskNo
        {
            get { return (string)GetValue(EMSTaskNoProperty); }
            set { SetValue(EMSTaskNoProperty, value); }
        }
        #endregion

        #region 状态
        public static readonly DependencyProperty EMSStateProperty = DependencyProperty.Register("EMSState", typeof(string), typeof(EMS_Car));

        [Category("HMIC")]
        public string EMSState
        {
            get { return (string)GetValue(EMSStateProperty); }
            set { SetValue(EMSStateProperty, value); }
        }
        #endregion

        #region 小车编号
        public static readonly DependencyProperty EMSNoProperty = DependencyProperty.Register("EMSNo", typeof(string), typeof(EMS_Car));

        [Category("HMIC")]
        public string EMSNo
        {
            get { return (string)GetValue(EMSNoProperty); }
            set { SetValue(EMSNoProperty, value); }
        }
        #endregion
        #region 小车是否显示
        public static readonly DependencyProperty IsVisibleEMSProperty = DependencyProperty.Register("IsVisibleEMS", typeof(string), typeof(EMS_Car));

        [Category("HMIC")]
        public string IsVisibleEMS
        {
            get { return (string)GetValue(IsVisibleEMSProperty); }
            set { SetValue(IsVisibleEMSProperty, value); }
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

                switch (contrName)
                {
                    case "EMSTaskNo":
                        EMSTaskNo = contrVal;
                        break;
                    case "EMSBarcode":
                        EMSBarcode = contrVal;
                        break;
                    //case "EMSState":
                    //ContrBackground = contrVal == "1" ? ColorLibrary.GetColorStr(ColorLibrary.AutoModeColor) : ColorLibrary.GetColorStr(ColorLibrary.MoveModeColor);
                    //    break;
                    case "EMSNo":
                        EMSNo = contrVal;
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
    }
}
