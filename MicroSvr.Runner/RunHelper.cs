using System.Drawing;
using System.IO;

namespace MicroSvr.WPFRunner;

/// <summary>
/// 程序执行Helper，
/// </summary>
class RunHelper
{
    #region 单例模式
    //private static readonly object lockobject = new object();
    public static RunHelper Instance { get { return RunnerInstanceHelper.Create<RunHelper>(); } }
    /// <summary>
    /// 构造函数，加载程序文件夹内的所有文件
    /// </summary>
    private RunHelper()
    {
        try
        {
            
            StartLogFileHelper.Instance.StartMsg("RunHelper");
            new PrivatePath().AppendPrivatePaths();
            StartLogFileHelper.Instance.StartMsg($"AppPath：{Process.GetCurrentProcess().MainModule.FileName}");
            StartLogFileHelper.Instance.StartMsg("Initialize Runner");
            this.AppIcon = getfavicon();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
    #endregion

    #region RunApp
    /// <summary>
    /// 启动参数
    /// </summary>
    public string[] ApplicationArgs { get; set; }
    /// <summary>
    /// 启动参数 静默启动
    /// </summary>
    public bool IsSilent
    {
        get
        {
            var args = this.ApplicationArgs;
            if (args == null)
            {
                return false;
            }
            foreach (var arg in args)
            {
                if (string.IsNullOrWhiteSpace(arg))
                {
                    continue;
                }
                if (arg.Trim().ToLower() == "/silent")
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 获取启动参数
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private string getMainArg(string name)
    {
        var ss = this.ApplicationArgs;
        if (ss == null)
        {
            return string.Empty;
        }
        foreach (var s in ss)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                continue;
            }
            var p = s.Trim();
            var idx = p.ToLower().IndexOf(name.ToLower(), StringComparison.Ordinal);
            if (idx < 0)
            {
                continue;
            }
            p = p.Substring(idx + name.Length);
            p = p.Trim();
            if (!p.StartsWith("="))
            {
                continue;
            }
            p = p.Substring(1);
            return p.Trim();
        }
        return string.Empty;
    }
    /// <summary>
    /// 系统路径
    /// </summary>
    private DirectoryInfo getAppPath()
    {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly == null)
        {
            assembly = this.GetType().Assembly;
        }
        var app = new FileInfo(new Uri(assembly.Location).LocalPath);
        return app.Directory;
    }

    /// <summary>
    /// 是否启动过进程
    /// </summary>
    /// <param name="mutexId"></param>
    /// <returns></returns>
    private bool isMutexId(string mutexId)
    {
        if (string.IsNullOrWhiteSpace(mutexId))
        {
            mutexId = getAppPath().FullName.Replace(":", "").Replace("\\", "|");
        }
        StartLogFileHelper.Instance.StartMsg("初始化系统运行界面……"+ mutexId);
        if (!string.IsNullOrWhiteSpace(mutexId))
        {
            StartLogFileHelper.Instance.StartMsg("进程互斥变量=" + mutexId);
            if (new AppMutexHelper().IsExist(mutexId))
            {
                StartLogFileHelper.Instance.StartMsg($"系统已运行，不允许重复运行！");
                var path = getAppPath();
                HandleFileHelper.Instance.ShowMainForm(path);
                return true;
                //if (!RunHelper.Instance.IsSilent)
                //{
                //    MessageBox.Show($"{path.FullName}\r\n\r\n系统不允许重复执行！", $"系统已运行");
                //}
                //return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 启动进程
    /// </summary>
    /// <param name="mutexId"></param>
    public void RunApp(string mutexId)
    {
        try
        {
            FormStateHelper.Instance.SetFormState(null);
            FormStateHelper.Instance.SetFormState(StartFrm.Instance);
            if (isMutexId(mutexId))
            {
                AppKillerHelper.Instance.KillSelf();
                return;
            }
            if (new AdminRunHelper().IsRunAsAdmin())
            {
                StartLogFileHelper.Instance.StartMsg("正在以管理员身份运行……");
            }
            else
            {
                StartLogFileHelper.Instance.StartMsg("非管理员身份运行……");
                //StartLogFileHelper.Instance.StartMsg("应用重启……");
                //AppKillerHelper.Instance.KillSelf();
                //new AdminRun().RunAsAdmin(MicroSvr.Runner.RunHelper.Instance.ApplicationArgs);
                //return;
            }
            StartLogFileHelper.Instance.StartMsg("重复文件检测……");
            RepeatFileHelper.Instance.Check();
            StartLogFileHelper.Instance.StartMsg("服务启动……");

            //MessageBox.Show("服务启动");

            if (!start_server())
            {
                return;
            }
            StartLogFileHelper.Instance.StartMsg("运行主界面……");
            runApp();
            ShowMainForm();
        }
        catch (Exception ex)
        {
            StartLogFileHelper.Instance.StartException(ex);
        }
    }

    public void ShowMainForm()
    {
        if (this.MainFrm != null)
        {
            StartLogFileHelper.Instance.StartMsg("Application.Run");
            Application.Current.MainWindow = this.MainFrm;

            this.MainFrm.Closed += MainFrm_FormClosed;

            MainFrm.Show();
        }
    }

    /// <summary>
    /// 启动服务
    /// </summary>
    /// <returns></returns>
    internal bool start_server()
    {
        try
        {
            var result = startserver();
            StartLogFileHelper.Instance.StartMsg($"服务启动完成={result}");
        }
        catch (Exception ex)
        {
            StartLogFileHelper.Instance.StartException(ex);
            return false;
        }
        return true;
    }
    private int startserver()
    {
        var result = 0;
        StartLogFileHelper.Instance.StartMsg($"开始启动服务=MicroSvr.RSF.Server.RsfServerHelper,MicroSvr.RSF.Server");
        var type = Type.GetType("MicroSvr.RSF.Server.RsfServerHelper,MicroSvr.RSF.Server", false, true);
        if (type == null)
        {
            return result;
        }
        var instance = type.GetProperty("Instance").GetValue(null);
        var method = instance.GetType().GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        result = (int)method.Invoke(instance, new object[] { new Action<string>(StartLogFileHelper.Instance.StartMsg) });
        return result;
    }

    /// <summary>
    /// 启动进程
    /// </summary>
    private void runApp()
    {

        if (this.MainFrm != null)
        {
            StartLogFileHelper.Instance.StartMsg("主界面正在运行");
            return;
        }
        StartLogFileHelper.Instance.StartMsg("获取主界面……");
        this.MainFrm = AppRunHelper.Instance.GetAppForm();
        
    }


    private void MainFrm_FormClosed(object sender, EventArgs e)
    {
        AppKillerHelper.Instance.KillSelf();
    }
    #endregion

    #region MainFrm
    public Window MainFrm { get; private set; }
    #endregion

    #region Ico
    public event Action<ImageSource> IconChanged;
    public ImageSource AppIcon { get { return this.__appIcon; } set { setAppIcon(value); } }
    private ImageSource __appIcon = null;
    private void setAppIcon(ImageSource icon)
    {
        if (icon == null)
        {
            return;
        }
        this.__appIcon = icon;
        try { IconChanged?.Invoke(icon); } catch { }
    }

    private ImageSource getfavicon()
    {

        var appFile = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);
        if (!appFile.Exists)
        {
            return null;
        }
        var filepath = Path.Combine(appFile.Directory.FullName, "favicon.ico");
        var faviconFile = new FileInfo(filepath);
        if (faviconFile.Exists)
        {
            try
            {
                Uri iconUri = new Uri(filepath, UriKind.RelativeOrAbsolute);
                return  BitmapFrame.Create(iconUri);
            }
            catch { };
        }
        return ConvertIconToImageSource(Icon.ExtractAssociatedIcon(appFile.FullName));
    }

    private ImageSource ConvertIconToImageSource(Icon icon)
    {
        ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
            icon.Handle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());

        return imageSource;
    }

    #endregion

}

