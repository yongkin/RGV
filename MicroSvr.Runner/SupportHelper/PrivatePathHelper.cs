using System.IO;

namespace MicroSvr.WPFRunner;
/// <summary>
/// 程序应用程序文件夹
/// </summary>
class PrivatePathHelper
{
    #region 单例模式
    //private static readonly object lockobject = new object();
    public static PrivatePathHelper Instance { get { return RunnerInstanceHelper.Create<PrivatePathHelper>(); } }
    /// <summary>
    /// 构造函数
    /// </summary>
    private PrivatePathHelper()
    {
        try
        {
            this.PrivatePaths = getPrivatePaths();
        }
        catch { }
    }
    #endregion

    public DirectoryInfo[] PrivatePaths { get; private set; }

    private FileInfo __process_file;
    /// <summary>
    /// 系统路径
    /// </summary>
    public FileInfo ProcessFile
    {
        get
        {
            if (__process_file != null)
            {
                return __process_file;
            }
            var fileName = Process.GetCurrentProcess().MainModule.FileName;
            return __process_file = new FileInfo(fileName);
        }
    }

    /// <summary>
    /// 系统路径
    /// </summary>
    private DirectoryInfo getSystemPath()
    {
        var dir = this.ProcessFile.Directory;
        if (dir.Name.ToLower() == "bin")
        {
            dir = dir.Parent;
        }
        if (dir.Name.ToLower().EndsWith(".app.bin"))
        {
            dir = dir.Parent;
        }
        return dir;
    }
    /// <summary>
    /// 获取应用程序文件夹
    /// </summary>
    /// <returns></returns>
    private DirectoryInfo[] getPrivatePaths(DirectoryInfo baseDir)
    {
        //sendMsg($"getPrivatePaths:{baseDir.FullName}");
        StartLogFileHelper.Instance.StartMsg("getPrivatePaths:"+ baseDir.FullName);

        var result = new List<DirectoryInfo>();
        var fi = new FileInfo(Path.Combine(baseDir.FullName, ".runnerignore"));
        if (fi.Exists)
        {
            return result.ToArray();
        }
        if (checkPuginDir(baseDir))
        {
            return addPrivatePath(baseDir);
        }
        //result.Add(baseDir);
        foreach (var dir in baseDir.GetDirectories())
        {
            //var tDir =;
            result.AddRange(getPrivatePaths(dir));
        }
        return result.ToArray();
    }
    /// <summary>
    /// 获取应用程序文件夹
    /// </summary>
    /// <returns></returns>
    private DirectoryInfo[] getPrivatePaths()
    {
        var result = new List<DirectoryInfo>();
        var processPath = getSystemPath();
        result.Add(processPath);
        //加载当前文件夹内的路径（runner）
        result.AddRange(getPrivatePaths(processPath));
        //加载公用文件
        var runtimePath = new DirectoryInfo(Path.Combine(processPath.Parent.FullName, "CommonLib"));
        if (runtimePath.Exists)
        {
            var winPath = new DirectoryInfo(Path.Combine(runtimePath.FullName, "SysLib"));
            if (winPath.Exists)
            {
                result.Add(winPath);
                result.AddRange(getPrivatePaths(winPath));
            }
            winPath = new DirectoryInfo(Path.Combine(runtimePath.FullName, "CustomLib"));
            if (winPath.Exists)
            {
                result.Add(winPath);
                result.AddRange(getPrivatePaths(winPath));
            }
            if (this.ProcessFile.Name.ToLower().Contains(".x86."))
            {
                winPath = new DirectoryInfo(Path.Combine(runtimePath.FullName, "CustomLibX86"));
            }
            else
            {
                winPath = new DirectoryInfo(Path.Combine(runtimePath.FullName, "CustomLibX64"));
            }
            if (winPath != null && winPath.Exists)
            {
                result.Add(winPath);
                result.AddRange(getPrivatePaths(winPath));
            }
        }
        return result.ToArray();
    }
    /// <summary>
    /// 应用程序文件夹判定
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    private bool checkPuginDir(DirectoryInfo dir)
    {
        var fi = new FileInfo(Path.Combine(dir.FullName, this.GetType().Namespace));
        return fi.Exists;
    }

    /// <summary>
    /// 获取所有应用程序文件夹
    /// </summary>
    /// <param name="baseDir"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    private DirectoryInfo[] addPrivatePath(DirectoryInfo dir)
    {
        var result = new List<DirectoryInfo>();
        var fi = new FileInfo(Path.Combine(dir.FullName, ".runnerignore"));
        if (fi.Exists)
        {
            return result.ToArray();
        }
        result.Add(dir);
        foreach (var sdir in dir.GetDirectories())
        {
            result.AddRange(addPrivatePath(sdir));
        }
        return result.ToArray();
    }
}
