using System.Windows.Interop;

namespace MicroSvr.WPFRunner;
class HandleFileHelper
{
    #region 单例模式
    //private static readonly object lockobject = new object();
    public static HandleFileHelper Instance { get { return RunnerInstanceHelper.Create<HandleFileHelper>(); } }
    /// <summary>
    /// 构造函数
    /// </summary>
    private HandleFileHelper()
    {
    }
    #endregion

    private const string CONST_MAIN_FORM_HANDLE = "MainFrm=";
    private const string CONST_STSRT_FORM_HANDLE = "StartFrm=";

    /// <summary>
    /// 界面日志保存
    /// </summary>
    /// <param name="frm"></param>
    public void Save(Window frm)
    {
        var file = getRunningFile();
        if (file.Exists)
        {
            file.Delete();
        }
        appendText($"ProcessId={Process.GetCurrentProcess().Id}");
        IntPtr windowHandle = new WindowInteropHelper(frm).Handle;
        appendText($"{CONST_MAIN_FORM_HANDLE}{windowHandle}");
        //使用StartFrm进行windows消息处理  getStartFormHandle
        IntPtr StartFrmHandle = new WindowInteropHelper(StartFrm.Instance).Handle;
        appendText($"{CONST_STSRT_FORM_HANDLE}{StartFrmHandle}");
        StartLogFileHelper.Instance.StartMsg($"ProcessId[{Process.GetCurrentProcess().Id}].MainFrm[{windowHandle}]");
    }

    private void appendText(string msg)
    {
        try
        {
            msg = $"{msg}\r\n";
            System.Console.WriteLine(msg);
            System.Diagnostics.Trace.WriteLine(msg);
            var file = getRunningFile();
            File.AppendAllText(file.FullName, msg);
        }
        catch { }
    }

    private string getFileName()
    {
        var fileName = this.GetType().Namespace;
        fileName = fileName.Substring(0, fileName.IndexOf(".", StringComparison.Ordinal) + 1) + "handle";
        return fileName;
    }

    private FileInfo getRunningFile()
    {
        var appPath = new FileInfo(new Uri(this.GetType().Assembly.Location).LocalPath);
        var file = new FileInfo(new Uri(this.GetType().Assembly.Location).LocalPath);
        var dir = file.Directory;
        if (dir.Name.ToLower() == "bin")
        {
            dir = dir.Parent;
        }
        file = new FileInfo(Path.Combine(dir.FullName, getFileName()));
        return file;
    }
    public void ShowMainForm(DirectoryInfo path)
    {
        try
        {
            showMainForm(path);
        }
        catch (Exception ex)
        { }
    }
    private IntPtr getStartFormHandle(DirectoryInfo path)
    {
        var file = new FileInfo(Path.Combine(path.FullName, getFileName()));
        if (file == null || !file.Exists)
        {
            return IntPtr.Zero;
        }
        var ss = File.ReadAllLines(file.FullName);
        if (ss == null || ss.Length == 0)
        {
            return IntPtr.Zero;
        }
        var handle = IntPtr.Zero;
        foreach (var s in ss)
        {
            var str = s.ToLower().Trim();
            if (!str.StartsWith(CONST_STSRT_FORM_HANDLE.ToLower()))
            {
                continue;
            }
            str = str.Substring(CONST_STSRT_FORM_HANDLE.Length);
            if (int.TryParse(str, out int h))
            {
                handle = new IntPtr(h);
            }
            break;
        }
        return handle;
    }

    private IntPtr GetMainFormHandle(DirectoryInfo path)
    {
        var file = new FileInfo(Path.Combine(path.FullName, getFileName()));
        if (file == null || !file.Exists)
        {
            return IntPtr.Zero;
        }
        var ss = File.ReadAllLines(file.FullName);
        if (ss == null || ss.Length == 0)
        {
            return IntPtr.Zero;
        }
        var handle = IntPtr.Zero;
        foreach (var s in ss)
        {
            var str = s.ToLower().Trim();
            if (!str.StartsWith(CONST_MAIN_FORM_HANDLE.ToLower()))
            {
                continue;
            }
            str = str.Substring(CONST_MAIN_FORM_HANDLE.Length);
            if (int.TryParse(str, out int h))
            {
                handle = new IntPtr(h);
            }
            break;
        }
        return handle;
    }

    private void showMainForm(DirectoryInfo path)
    {
        var handle = getStartFormHandle(path);
        if (handle == IntPtr.Zero)
        {
            return;
        }
        StartFrm.Instance.ShowMainForm(handle);
    }
}
