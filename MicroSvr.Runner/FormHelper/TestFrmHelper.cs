

namespace MicroSvr.WPFRunner;
class TestFrmHelper
{
    #region 单例模式
    //private static readonly object lockobject = new object();
    public static TestFrmHelper Instance { get { return RunnerInstanceHelper.Create<TestFrmHelper>(); } }
    /// <summary>
    /// 构造函数
    /// </summary>
    private TestFrmHelper()
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
        var loacl = new FileInfo(new Uri(this.GetType().Assembly.Location).LocalPath).Directory;
        foreach (var fi in loacl.GetFiles())
        {
            var fileName = fi.Name.ToLower();
            if (fileName == thisFile.Name.ToLower())
            {
                continue;
            }
            if (fileName.StartsWith(startName)
                && fileName.EndsWith("test" + endName))
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
        StartLogFileHelper.Instance.StartMsg($"获取TEST类库的界面……");
        var files = getRunnerFiles();
        foreach (var file in files)
        {
            StartLogFileHelper.Instance.StartMsg($"{file.Name}>>MainFrm");
            var frm = getForm(file);
            if (frm != null)
            {
                StartLogFileHelper.Instance.StartMsg($"获取TEST类库成功");
                return frm;
            }
        }
        StartLogFileHelper.Instance.StartMsg($"获取TEST类库失败");
        return null;
    }
}
