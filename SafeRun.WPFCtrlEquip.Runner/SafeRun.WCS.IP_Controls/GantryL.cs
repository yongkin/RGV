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
    /// 龙门-纵向
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
    ///     <MyNamespace:Gantry_L/>
    ///
    /// </summary>
    public class GantryL : HMIControlBase
    {
        static GantryL()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GantryL), new FrameworkPropertyMetadata(typeof(GantryL)));
        }

        private Grid Gantry1 = null;
        private Grid Gantry2 = null;
        private Grid GantryContr = null;
        private double ContrWidth = 520;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // 在模板中查找名为 "PART_MyButton" 的子级元素
            Gantry1 = GetTemplateChild("Gantry1") as Grid;
            Gantry2 = GetTemplateChild("Gantry2") as Grid;
            GantryContr = GetTemplateChild("GantryContr") as Grid;

            if (GantryContr != null)
            {
                ContrWidth = GantryContr.Width;
            }

            if (Gantry1 != null)
            {
                ContrWidth = ContrWidth - Gantry1.Width;
            }

        }


        #region 设置自定义控件属性值

        #region 共有多少列
        public static readonly DependencyProperty GantryColNumProperty = DependencyProperty.Register("GantryColNum", typeof(int), typeof(GantryL));

        [Category("HMIC")]
        public int GantryColNum
        {
            get { return (int)GetValue(GantryColNumProperty); }
            set { SetValue(GantryColNumProperty, value); }
        }


        #endregion


        #region 龙门显示1
        public static readonly DependencyProperty IsShowGantry1Property = DependencyProperty.Register("IsShowGantry1", typeof(string), typeof(GantryL));

        /// <summary>
        /// 值: Visible 显示 Hidden 隐藏
        /// </summary>
        [Category("HMIC")]
        public string IsShowGantry1
        {
            get { return (string)GetValue(IsShowGantry1Property); }
            set { SetValue(IsShowGantry1Property, value); }
        }


        #endregion

        #region 龙门显示2
        public static readonly DependencyProperty IsShowGantry2Property = DependencyProperty.Register("IsShowGantry2", typeof(string), typeof(GantryL));

        /// <summary>
        /// 值: Visible 显示 Hidden 隐藏
        /// </summary>
        [Category("HMIC")]
        public string IsShowGantry2
        {
            get { return (string)GetValue(IsShowGantry2Property); }
            set { SetValue(IsShowGantry2Property, value); }
        }

        #region 龙门1任务显示
        public static readonly DependencyProperty Generic1TaskNoProperty = DependencyProperty.Register("Generic1TaskNo", typeof(string), typeof(GantryL));

        [Category("HMIC")]
        public string Generic1TaskNo
        {
            get { return (string)GetValue(Generic1TaskNoProperty); }
            set { SetValue(Generic1TaskNoProperty, value); }
        }


        #endregion

        #region 龙门2任务显示
        public static readonly DependencyProperty Generic2TaskNoProperty = DependencyProperty.Register("Generic2TaskNo", typeof(string), typeof(GantryL));

        [Category("HMIC")]
        public string Generic2TaskNo
        {
            get { return (string)GetValue(Generic2TaskNoProperty); }
            set { SetValue(Generic2TaskNoProperty, value); }
        }


        #endregion


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
                    case "Generic1TaskNo":
                        Generic1TaskNo = contrVal;
                        break;
                    case "Generic2TaskNo":
                        Generic2TaskNo = contrVal;
                        break;
                    case "G1CurrentCol":
                        SetGantryContrPosition(Gantry1, contrVal);
                        break;
                    case "G2CurrentCol":
                        SetGantryContrPosition(Gantry2, contrVal);
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

        private void SetGantryContrPosition(Grid grid, string currentCol)
        {
            if (grid == null) return;
            double currentColNum = double.Parse(currentCol);
            double colRatio = ((currentColNum - 1) / (GantryColNum - 1) * 1.000);
            double gantryPosition = ContrWidth * colRatio;
            grid.Margin = new Thickness(gantryPosition, 0, 0, 0);
        }

        #endregion
    }
}
