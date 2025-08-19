namespace MicroSvr.WPFRunner;
class LocalSubFrmHelper
{
    #region 单例模式
    //private static readonly object lockobject = new object();
    public static LocalSubFrmHelper Instance { get { return RunnerInstanceHelper.Create<LocalSubFrmHelper>(); } }
    /// <summary>
    /// 构造函数
    /// </summary>
    private LocalSubFrmHelper()
    {
    }
    #endregion

    private FileInfo[] getRunnerFiles()
    {
        var result = new List<FileInfo>();
        var thisFile = new FileInfo(new Uri(this.GetType().Assembly.Location).LocalPath);
        var thisfilename = thisFile.Name.ToLower();
        var startName = thisfilename.Substring(0, thisfilename.IndexOf(".", StringComparison.Ordinal) + 1);//MicroSvr.
        var endName = thisfilename.Substring(thisfilename.LastIndexOf(".", StringComparison.Ordinal));//.dll
        var local = new FileInfo(new Uri(this.GetType().Assembly.Location).LocalPath).Directory;
        local = new DirectoryInfo(Path.Combine(local.FullName, this.GetType().Namespace));
        if (!local.Exists)
        {
            return result.ToArray();
        }
        var runner = new FileInfo(Path.Combine(local.FullName, this.GetType().Namespace));
        if (!runner.Exists)
        {
            return result.ToArray();
        }
        foreach (var fi in local.GetFiles())
        {
            var fileName = fi.Name.ToLower();
            if (fileName == thisFile.Name.ToLower())
            {
                continue;
            }
            if (fileName.StartsWith(startName)
                && fileName.EndsWith(endName))
            {
                result.Add(fi);
            }
        }
        return result.ToArray();
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

    private Window getForm(FileInfo file)
    {
        var type = getFormType(file);
        if (type == null)
        {
            StartLogFileHelper.Instance.StartMsg("界面类为空！");
            return null;
        }
        StartLogFileHelper.Instance.StartMsg($"界面类[{type.AssemblyQualifiedName}]");
        var obj = System.Activator.CreateInstance(type, null);
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

    public Window GetAppForm()
    {
        StartLogFileHelper.Instance.StartMsg($"获取子路径中类库的界面……");
        var files = getRunnerFiles();
        foreach (var file in files)
        {
            StartLogFileHelper.Instance.StartMsg($"{file.Name}>>MainFrm");
            var frm = getForm(file);
            if (frm != null)
            {
                StartLogFileHelper.Instance.StartMsg($"获取子路径中类库成功");
                return frm;
            }
        }
        StartLogFileHelper.Instance.StartMsg($"获取子路径中类库失败");
        return null;
    }
}
