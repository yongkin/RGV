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
    /// 参考值 宽度:56.92 高度:27 靠左排位值:11.4
    /// 例子:<IP_Controls:RGV_Curve HorizontalAlignment="Left" Margin="11.4,1040,0,0" VerticalAlignment="Top" Width="56.92" Height="27" RenderTransformOrigin="0.5,0.5">
    /// 
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
    ///     <MyNamespace:DirectionArrow/>
    ///
    /// </summary>
    public class RGV_Curve : HMIControlBase
    {
        static RGV_Curve()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RGV_Curve), new FrameworkPropertyMetadata(typeof(RGV_Curve)));
        }

        #region 设置自定义控件属性值

        #region 任务号
        //public static readonly DependencyProperty StackerTaskNoProperty = DependencyProperty.Register("StackerTaskNo", typeof(string), typeof(Stacker));

        //[Category("HMIC")]
        //public string StackerTaskNo
        //{
        //    get { return (string)GetValue(StackerTaskNoProperty); }
        //    set { SetValue(StackerTaskNoProperty, value); }
        //}
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
                    //case "StackerTaskNo":
                    //    StackerTaskNo = contrVal;
                    //    break;
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
    }
}
