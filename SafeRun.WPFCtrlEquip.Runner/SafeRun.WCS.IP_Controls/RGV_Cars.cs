using SafeRun.CtrlComm;
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
    ///     <MyNamespace:RGV_Car/>
    ///
    /// </summary>
    public class RGV_Cars : HMIControlBase
    {
        private StackPanel _rgvItemPanel;
        static RGV_Cars()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RGV_Cars), new FrameworkPropertyMetadata(typeof(RGV_Cars)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _rgvItemPanel = GetTemplateChild("RGVItemPanel") as StackPanel;

        }

        #region 设置自定义控件属性值

        #region 状态
        //public static readonly DependencyProperty RGVStateProperty = DependencyProperty.Register("RGVsState", typeof(string), typeof(RGV_Car));

        //[Category("HMIC")]
        //public string RGVState
        //{
        //    get { return (string)GetValue(RGVStateProperty); }
        //    set { SetValue(RGVStateProperty, value); }
        //}
        #endregion

        #region 小车编号
        //public static readonly DependencyProperty RGVNoProperty = DependencyProperty.Register("RGVsNo", typeof(string), typeof(RGV_Car));

        //[Category("HMIC")]
        //public string RGVNo
        //{
        //    get { return (string)GetValue(RGVNoProperty); }
        //    set { SetValue(RGVNoProperty, value); }
        //}
        #endregion
        #region 小车是否显示
        public static readonly DependencyProperty IsVisibleRgvsProperty = DependencyProperty.Register("IsVisibleRgvs", typeof(string), typeof(RGV_Car));

        [Category("HMIC")]
        public string IsVisibleRgvs
        {
            get { return (string)GetValue(IsVisibleRgvsProperty); }
            set { SetValue(IsVisibleRgvsProperty, value); }
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

        public bool AddRGVItem(string rgvNo, string rgvStatus)
        {
            return SetRGVItemPanel("add", rgvNo, rgvStatus); ;
        }

        public bool UpdRGVItem(string rgvNo, string rgvStatus)
        {
            return SetRGVItemPanel("upd", rgvNo, rgvStatus); ;
        }

        public bool DelRGVItem(string rgvNo)
        {
            return SetRGVItemPanel("del", rgvNo, "0"); ;
        }


        private bool SetRGVItemPanel(string setType, string rgvNo, string rgvStatus)
        {
            if (_rgvItemPanel == null) return false;
            var result = false;
            switch (setType)
            {
                case "add":
                    if (_rgvItemPanel.Children.Count >= 3) break;
                    var rgvItem = new RGV_Item
                    {
                        Width = 20,
                        Height = 40,
                        Margin = new Thickness(2, 0, 3, 0)
                    };
                    rgvItem.RGVItemNo = rgvNo;
                    rgvItem.SetStatus(StatusConverter(rgvStatus));
                    _rgvItemPanel.Children.Add(rgvItem);
                    result = true;
                    break;
                case "upd":
                    var itemToUpdate = _rgvItemPanel.Children
               .OfType<RGV_Item>()
               .FirstOrDefault(item => item.RGVItemNo == rgvNo);
                    itemToUpdate.RGVItemNo = rgvNo;
                    itemToUpdate.SetStatus(StatusConverter(rgvStatus));
                    
                    result = true;
                    break;
                case "del":
                    var itemToRemove = _rgvItemPanel.Children
               .OfType<RGV_Item>()
               .FirstOrDefault(item => item.RGVItemNo == rgvNo);
                    if (itemToRemove != null)
                    {
                        _rgvItemPanel.Children.Remove(itemToRemove);
                    }
                    result = true;
                    break;
            }

            return result;

        }

        private string StatusConverter(string status)
        {
            //1,未启用;2,急停;3,故障;4,心跳异常;5,手动;6自动;7，未启用。
            switch (status)
            {
                case "6":
                    return "1"; //6自动
                case "5":
                    return "2";//手动
                case "4":
                    return "3";//故障
                case "3":
                    return "3";//故障
                case "2":
                    return "3";//故障
                case "1":
                    return "2";//手动
                default:
                    return "3";
            }
        }
    }
}
