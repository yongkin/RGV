using System.Windows;

namespace MicroSvr.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // 创建并显示你想要的主窗口
           MicroSvr.WPFRunner.RunApp.AppMain(e.Args);
        }
    }

}
