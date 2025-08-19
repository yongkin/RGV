using SafeRun.WPFCtrlEquip.WPFRunner.Common;
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
using static SafeRun.WPFCtrlEquip.WPFRunner.Common.CommonMethods;

namespace SafeRun.WCS.IP_Controls
{
    /// <summary>
    /// 传感器
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
    ///     <MyNamespace:Sensor/>
    ///
    /// </summary>
    public class Sensor : HMIControlBase
    {
        static Sensor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Sensor), new FrameworkPropertyMetadata(typeof(Sensor)));
        }

        #region 设置自定义控件属性值

        //#region 任务号
        //public static readonly DependencyProperty TaskNoProperty = DependencyProperty.Register("CVTaskNo", typeof(string), typeof(LineBody));

        //[Category("HMIC")]
        //public string TaskNo
        //{
        //    get { return (string)GetValue(TaskNoProperty); }
        //    set { SetValue(TaskNoProperty, value); }
        //}
        //#endregion

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
                contrVal = DataDisassembly("FT_FullLayout_CC", contrName, contrVal);
                switch (contrName)
                {
                    case "IsActive":
                        if(contrVal == "1")
                        {
                            ContrBackground = ColorLibrary.GetColorStr(ColorLibrary.SensorActiveColor);
                        }
                        else
                        {
                            ContrBackground = ColorLibrary.GetColorStr(ColorLibrary.DefaultBackgroundColor);
                        }

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


        #region 自定义方法
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
        private string FitterFT_FullLayout_CC(string contrName, string contrVal)
        {
            var result = contrVal;

            switch (contrName)
            {
                case "IsActive":
                    result = CommonMethods.UshortChangeInt(ushort.Parse(contrVal), ValidPositions.Pos8).ToString();
                    break;
                //case "Alarm":
                //    result = CommonMethods.UshortChangeInt(ushort.Parse(contrVal), ValidPositions.Pos2).ToString();
                //    break;
                //case "AutoMode":
                //    result = CommonMethods.UshortChangeInt(ushort.Parse(contrVal), ValidPositions.Pos10).ToString(); ;
                //    break;
            }

            return result;
        }



        #endregion



        #endregion

        #endregion
    }
}
