namespace MicroSvr.WPFRunner;
class StartLogFileHelper
{
    #region 单例模式
    //private static readonly object lockobject = new object();
    public static StartLogFileHelper Instance { get { return RunnerInstanceHelper.Create<StartLogFileHelper>(); } }
    /// <summary>
    /// 构造函数
    /// </summary>
    private StartLogFileHelper()
    {
        try
        {
            var runningFile = getRunningFile();
            if (runningFile.Exists)
            {
                runningFile.Delete();
            }
        }
        catch
        {

        }
    }

    #endregion

    /// <summary>
    /// 启动消息
    /// </summary>
    /// <param name="msg"></param>
    public void StartMsg(string msg)
    {
        sysStarFormLogMessage(1, msg);
    }
    public void StartException(Exception ex)
    {
        this.StartException(string.Empty, ex);
    }

    private static readonly object lockException = new object();
    private int isException = 0;
    /// <summary>
    /// 启动异常，异常只触发一次
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="ex"></param>
    public void StartException(string msg, Exception ex)
    {
        if (isException > 0)
        {
            return;
        }
        lock (lockException)
        {
            if (isException > 0)
            {
                return;
            }
            isException++;
            try
            {
                var log = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    log.Append(msg).Append(">>>").AppendLine();

                }
                log.Append(ex.ToString());
                sysStarFormLogMessage(5, log.ToString());
                AppKillerHelper.Instance.AfterKillSelf();
                StartFrm.Instance.ShowDialog();
            }
            catch { }
        }
    }

    private void sysStarFormLogMessage(int level, string msg)
    {
        try
        {
            appendText(msg);
            StartFrm.Instance.ShowMsg(level, msg);
        }
        catch { }
    }

    private void appendText(string msg)
    {
        try
        {
            msg = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} >> {msg}\r\n";
            Console.WriteLine(msg);
            System.Diagnostics.Trace.WriteLine(msg);
            var file = this.getRunningFile();
            File.AppendAllText(file.FullName, msg);
        }
        catch { }
    }
    private FileInfo getRunningFile()
    {
        var file = new FileInfo(new Uri(this.GetType().Assembly.Location).LocalPath);
        var dir = file.Directory;
        if (dir.Name.ToLower() == "bin")
        {
            dir = dir.Parent;
        }
        var fileName = this.GetType().Namespace;
        fileName = fileName.Substring(0, fileName.IndexOf(".", StringComparison.Ordinal) + 1) + "Running";
        file = new FileInfo(Path.Combine(dir.FullName, fileName));
        return file;
    }
}
