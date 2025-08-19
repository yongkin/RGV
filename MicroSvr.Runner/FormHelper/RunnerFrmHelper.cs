namespace MicroSvr.WPFRunner;
class RunnerFrmHelper
{
    #region 单例模式
    //private static readonly object lockobject = new object();
    public static RunnerFrmHelper Instance { get { return RunnerInstanceHelper.Create<RunnerFrmHelper>(); } }
    /// <summary>
    /// 构造函数
    /// </summary>
    private RunnerFrmHelper()
    {
    }
    #endregion

    public Window GetAppForm()
    {
        try
        {
            return getRunnerMainFrm();
        }
        catch (Exception ex)
        {
            StartLogFileHelper.Instance.StartException($"界面初始化异常，请确认系统配置！", ex);
        }
        return null;
    }

 

    private string getRunnerdllName()
    {
        var nsName = this.GetType().Namespace;
        var result = nsName.Substring(0, nsName.IndexOf('.')) + ".*" + nsName.Substring(nsName.IndexOf('.'));
        return result;
    }
    private Window getRunnerMainFrm()
    {
        StartLogFileHelper.Instance.StartMsg("初始化系统运行界面……");
        var runnerFiles = getRunnerFiles();
        if (runnerFiles == null || runnerFiles.Length == 0)
        {
            StartLogFileHelper.Instance.StartMsg($"{getRunnerdllName()}文件获取失败");
            return null;
        }
        Window frm = null;
        try
        {
            frm = getRunnerForm(runnerFiles);
        }
        catch (Exception ex)
        {
            StartLogFileHelper.Instance.StartMsg($"界面构造异常，请确认系统配置！>>\r\n{ex.ToString()}");
            return null;
        }
        if (frm == null)
        {
            StartLogFileHelper.Instance.StartMsg($"未发现系统界面，请确认系统配置！");
            return null;
        }
        StartLogFileHelper.Instance.StartMsg($"AppPath：{Process.GetCurrentProcess().MainModule.FileName}");
        return frm;
    }

    private Type getFormType(FileInfo runnerFile)
    {
        try
        {
            var ass = Assembly.LoadFile(runnerFile.FullName);
            var ts = ass.GetTypes();
            foreach (var t in ts)
            {
                if (t.Name.ToLower() == "mainfrm"
                    && typeof(Window).IsAssignableFrom(t))
                {
                    return t;
                }
            }
        }
        catch (Exception ex)
        {
            StartLogFileHelper.Instance.StartException($"获取界面类异常", ex);
        }
        return null;
    }

    private FileInfo[] getRunnerFiles()
    {
        var result = new List<FileInfo>();
        var thisFile = new FileInfo(new Uri(this.GetType().Assembly.Location).LocalPath);
        var thisfilename = thisFile.Name.ToLower();
        var startName = thisfilename.Substring(0, thisfilename.IndexOf(".", StringComparison.Ordinal) + 1);//Comm.
        var endName = thisfilename.Substring(thisfilename.IndexOf(".", StringComparison.Ordinal));//.runner.dll
        var endNamex86 = endName.Substring(0, endName.LastIndexOf(".", StringComparison.Ordinal)) + ".x86" + endName.Substring(endName.LastIndexOf(".", StringComparison.Ordinal));//.runner.dll
        foreach (var dir in PrivatePathHelper.Instance.PrivatePaths)
        {
            var fs = dir.GetFiles();
            foreach (var fi in fs)
            {
                var fileName = fi.Name.ToLower();
                if (fileName == thisfilename)
                {
                    continue;
                }
                if (fileName.EndsWith(endName) || fileName.EndsWith(endNamex86)) ////fileName.StartsWith(startName) &&  && (!fileName.Equals(thisfilename))

                {
                    StartLogFileHelper.Instance.StartMsg($"Runner文件[{fi.FullName}]");
                    result.Add(fi);
                }
            }
        }
        return result.ToArray();
    }

    private Window getRunnerForm(FileInfo[] files)
    {
        var frms = getRunnerForms(files);
        if (frms == null || frms.Length == 0)
        {
            return null;
        }
        return frms.FirstOrDefault();

    }

    private Window[] getRunnerForms(FileInfo[] files)
    {
        var result = new List<Window>();
        foreach (var file in files)
        {
            var frm = getRunnerForm(file);
            if (frm == null)
            {
                continue;
            }
            result.Add(frm);
        }
        return result.ToArray();
    }

    private Window getRunnerForm(FileInfo runnerFile)
    {
        StartLogFileHelper.Instance.StartMsg($"{getRunnerdllName()}信息=" + runnerFile.FullName);
        var type = getFormType(runnerFile);
        if (type == null)
        {
            StartLogFileHelper.Instance.StartMsg("界面类为空！");
            return null;
        }
        //type = Type.GetType(type.AssemblyQualifiedName);
        StartLogFileHelper.Instance.StartMsg($"界面类[{type.AssemblyQualifiedName}]");
        var obj = System.Activator.CreateInstance(type);
        if (obj == null)
        {
            StartLogFileHelper.Instance.StartMsg($"界面实例创建失败！[{type.AssemblyQualifiedName}]");
            return null;
        }
        var frm = obj as Window;
        if (frm == null)
        {
            StartLogFileHelper.Instance.StartMsg($"实例无法转为界面！[{type.AssemblyQualifiedName}]");
            return null;
        }
        return frm;
    }

}
