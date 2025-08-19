using SafeRun.WCS.IP_Controls.ControlPage;
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
    /// 线体
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
    ///     <MyNamespace:LineBody/>
    ///
    /// </summary>
    public class LineBody : HMIControlBase
    {

        static LineBody()
        {
            //绑定资源字典(Generic.xaml)内的样式
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineBody), new FrameworkPropertyMetadata(typeof(LineBody)));
        }

        #region 设置自定义控件属性值

        #region 任务号
        public static readonly DependencyProperty CVTaskNoProperty = DependencyProperty.Register("CVTaskNo", typeof(string), typeof(LineBody));

        [Category("HMIC")]
        public string CVTaskNo
        {
            get { return (string)GetValue(CVTaskNoProperty); }
            set { SetValue(CVTaskNoProperty, value); }
        }
        #endregion

        #region 出入库标志
        //Visible 显示 Hidden 隐藏
        //IsVisInbound="Visible" IsVisOutbound="Hidden" 
        public static readonly DependencyProperty IsVisOutboundProperty = DependencyProperty.Register("IsVisOutbound", typeof(string), typeof(LineBody));

        /// <summary>
        /// Visible 显示 Hidden 隐藏
        /// </summary>
        [Category("HMIC")]
        public string IsVisOutbound
        {
            get { return (string)GetValue(IsVisOutboundProperty); }
            set { SetValue(IsVisOutboundProperty, value); }
        }

        public static readonly DependencyProperty IsVisInboundProperty = DependencyProperty.Register("IsVisInbound", typeof(string), typeof(LineBody));

        /// <summary>
        /// Visible 显示 Hidden 隐藏
        /// </summary>
        [Category("HMIC")]
        public string IsVisInbound
        {
            get { return (string)GetValue(IsVisInboundProperty); }
            set { SetValue(IsVisInboundProperty, value); }
        }

        #endregion

        #region 任务框背景色设置
        public static readonly DependencyProperty CVTestBColorProperty = DependencyProperty.Register("CVTestBColor", typeof(string), typeof(LineBody));

        [Category("HMIC")]
        public string CVTestBColor
        {
            get { return (string)GetValue(CVTestBColorProperty); }
            set { SetValue(CVTestBColorProperty, value); }
        }
        #endregion

        #region 显示线体编号
        public static readonly DependencyProperty CVNoProperty = DependencyProperty.Register("CVNo", typeof(string), typeof(LineBody));

        [Category("HMIC")]
        public string CVNo
        {
            get { return (string)GetValue(CVNoProperty); }
            set { SetValue(CVNoProperty, value); }
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
                contrVal = DataDisassembly("FT_FullLayout_CC", contrName, contrVal);
                switch (contrName)
                {
                    case "TaskNoContr":
                        CVTaskNo = contrVal == "0" ? "" : contrVal;
                        CVTestBColor = (!string.IsNullOrEmpty(contrVal) && contrVal != "0" ) ? ColorLibrary.GetColorStr(ColorLibrary.CVLoadedColor) : Colors.White.ToString();
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


        public override void Control_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.Control_MouseRightButtonDown(sender, e);//调用父类的实现方法

            // 创建并显示新的弹窗:key:窗体内的控件名称，value:控件的绑定参数名称
            var operNodes = new Dictionary<string, string>();
            var contrItem = ShowTags.FirstOrDefault(x => x.ContrName == "TaskNoContr");
            if (contrItem != null && contrItem.NodeNames.Count == 1)
            {
                var nodeName = contrItem.NodeNames.First();
                operNodes.Add("updPlcTask", nodeName);
                operNodes.Add("cleanPlcTask", nodeName);
            }
            //operNodes.Add("cleanPlcTask", "TaskNoContr");
            //operNodes.Add("delete", "TaskNoContr");
            operNodes.Add("complete", "TaskNoContr");//启用强制完成
            var showParam = new Dictionary<string, string>();
            showParam.Add("ContrType", "CV");
            showParam.Add("ContrTypeName", "线体");
            showParam.Add("ShowTitle", this.ShowTitle);
            showParam.Add("TaskNo", this.CVTaskNo);

            base.RightClickFrom(showParam, operNodes, UpdAction, DeleteAction, CompleteAction, QueryAction);
        }

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
                case "TaskNoContr":
                    result = contrVal;
                    break;
                case "Alarm":
                    result = (string.IsNullOrEmpty(contrVal) || contrVal != "0") ? "1" : "0";
                    break;
                case "AutoMode":
                    result = CommonMethods.UshortChangeInt(ushort.Parse(contrVal), ValidPositions.Pos8).ToString(); ;
                    break;
            }

            return result;
        }



        #endregion



        #endregion

        #endregion

    }
}
