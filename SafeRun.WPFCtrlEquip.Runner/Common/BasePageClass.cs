
using System.Windows;


namespace SafeRun.WPFCtrlEquip.WPFRunner
{
    public class BasePageClass : Page
    {
        public BasePageClass()
        {
            //如果你的 UserControl 在 XAML 里调用了 静态方法 或者 访问了静态资源，但这些方法/资源在 WPF 设计器模式下不可用，就会导致 XAML 加载失败。
            //加这段代码，解决this.BindingToServer();静态调用的问题
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return; // 设计模式下不执行
            }
            //InitializeComponent();
            this.Loaded += Page_Loaded;
        }

        private bool IsLoaded { get; set; } = false;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                //这个要在视图树加载后才能生效
                this.BindingToServer();
                IsLoaded = true;
            }
        }
    }

    public class BaseUserControlClass : UserControl
    {
        public BaseUserControlClass()
        {
            //InitializeComponent();
            this.Loaded += Page_Loaded;
        }

        private bool IsLoaded { get; set; } = false;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                //这个要在视图树加载后才能生效
                this.BindingToServer();
                IsLoaded = true;
            }
        }
    }
}
