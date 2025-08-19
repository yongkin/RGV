

namespace MicroSvr.WPFRunner;
class AdminRunHelper
{
    public void RunAsAdmin(string[] args)
    {
        //创建启动对象 
        var startInfo = new System.Diagnostics.ProcessStartInfo();
        //设置运行文件 
        startInfo.FileName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        startInfo.WorkingDirectory = new FileInfo(startInfo.FileName).Directory.FullName;
        //设置启动参数 
        startInfo.Arguments = String.Join(" ", args);
        //设置启动动作,确保以管理员身份运行 
        startInfo.Verb = "runas";
        //如果不是管理员，则启动UAC 
        System.Diagnostics.Process.Start(startInfo);
        //退出 
        Application.Current.Shutdown();
    }

    public bool IsRunAsAdmin()
    {
        var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
        var principal = new System.Security.Principal.WindowsPrincipal(identity);
        //判断当前登录用户是否为管理员 
        var result = principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        return result;
    }
}
