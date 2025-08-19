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
    ///     <MyNamespace:RGV_Item/>
    ///
    /// </summary>
    public class RGV_Item : HMIControlBase
    {
        static RGV_Item()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RGV_Item), new FrameworkPropertyMetadata(typeof(RGV_Item)));
        }

        private Ellipse _rgvItemModeLamp;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _rgvItemModeLamp = GetTemplateChild("RGVItemModeLamp") as Ellipse;
        }

        #region 设置自定义控件属性值

        #region 状态
        public static readonly DependencyProperty RGVItemStateProperty = DependencyProperty.Register("RGVItemState", typeof(string), typeof(RGV_Item));

        [Category("HMIC")]
        public string RGVItemState
        {
            get { return (string)GetValue(RGVItemStateProperty); }
            set { SetValue(RGVItemStateProperty, value); }
        }
        #endregion

        #region 小车编号
        public static readonly DependencyProperty RGVItemNoProperty = DependencyProperty.Register("RGVItemNo", typeof(string), typeof(RGV_Item));

        [Category("HMIC")]
        public string RGVItemNo
        {
            get { return (string)GetValue(RGVItemNoProperty); }
            set { SetValue(RGVItemNoProperty, value); }
        }
        #endregion
        #region 小车是否显示
        public static readonly DependencyProperty IsVisibleRgvItemProperty = DependencyProperty.Register("IsVisibleRgvItem", typeof(string), typeof(RGV_Item));

        [Category("HMIC")]
        public string IsVisibleRgvItem
        {
            get { return (string)GetValue(IsVisibleRgvItemProperty); }
            set { SetValue(IsVisibleRgvItemProperty, value); }
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

                    case "RGVState":
                        

                        break;
                    case "RGVNo":
                        RGVItemNo = contrVal;
                        break;
                    

                }
                //base.SetShowControls(contrName, contrVal);

            }
            catch (Exception ex)
            {
                //throw;
            }

            return null;
        }

        #endregion

        public void SetStatus(string val)
        {
            switch (val)
            {
                case "1":
                    SetModeLampAnimationColor(ColorLibrary.AutoRunColor);//自动
                    break;
                case "2":
                    SetModeLampAnimationColor(ColorLibrary.ManualRunColor);//手动
                    break;
                case "3":
                    SetModeLampAnimationColor(ColorLibrary.AlarmColor);
                    break;//报警
            }
        }

        /// <summary>
        /// 模式灯切换
        /// </summary>
        /// <param name="color"></param>
        private void SetModeLampAnimationColor(Color color)
        {
            //var rect = (Ellipse)this.Template.FindName("RGVItemModeLamp", this);//保持灯绑定控件
            //var rect = (Ellipse)this.FindName("RGVItemModeLamp");//保持灯绑定控件
            if (_rgvItemModeLamp != null)
            {
                _rgvItemModeLamp.Fill = new SolidColorBrush(color);
                //_storyboardModeLamp = SetKeepAnimation(_storyboardModeLamp, color, new SolidColorBrush(color), rect);
                //_storyboardModeLamp.Stop();
                //_storyboardModeLamp.Begin();
            }

        }
    }
}
