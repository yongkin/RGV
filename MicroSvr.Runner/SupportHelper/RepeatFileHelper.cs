namespace MicroSvr.WPFRunner;
class RepeatFileHelper
{
    #region 单例模式
    //private static readonly object lockobject = new object();
    public static RepeatFileHelper Instance { get { return RunnerInstanceHelper.Create<RepeatFileHelper>(); } }
    /// <summary>
    /// 构造函数
    /// </summary>
    private RepeatFileHelper()
    {
    }
    #endregion

    /// <summary>
    /// 重复文件验证
    /// </summary>
    public void Check()
    {
        repeatFileException(getFiles(PrivatePathHelper.Instance.PrivatePaths));
    }

    class MyComparerNew : IEqualityComparer<FileInfo>
    {
        public bool Equals(FileInfo x, FileInfo y)
        {
            if (x.Directory.Name.ToLower() == "locales")
            {
                return false;
            }
            return x.FullName.Equals(y.FullName, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(FileInfo obj)
        {
            if (obj == null)
                return 0;
            else
                return obj.FullName.GetHashCode();
        }
    }

    private void repeatFileException(FileInfo[] files)
    {
        files = files.Distinct(new MyComparerNew()).ToArray();
        var gfiles = files.GroupBy((e) => { return e.Name.ToLower(); });
        var errsb = new StringBuilder();
        foreach (var gfile in gfiles)
        {
            if (gfile.Count() > 1)
            {
                errsb.Append(gfile.FirstOrDefault().Name);
                errsb.Append(">>>>>>>>>>>>>>>>>").AppendLine();
                foreach (var f in gfile)
                {
                    errsb.Append(f.FullName).AppendLine();
                }
                errsb.Append(gfile.FirstOrDefault().Name);
                errsb.Append("<<<<<<<<<<<<<<<<").AppendLine();
            }
        }
        if (errsb.Length > 0)
        {
            throw new Exception("重复类库文件\r\n\r\n" + errsb.ToString());
        }
    }
    private FileInfo[] getFiles(DirectoryInfo[] dirs)
    {
        var result = new List<FileInfo>();
        foreach (var dir in dirs)
        {
            result.AddRange(getFiles(dir));
        }
        return result.ToArray();

    }
    private FileInfo[] getFiles(DirectoryInfo dir)
    {
        var result = new List<FileInfo>();
        if (!dir.Exists)
        {
            return result.ToArray();
        }
        var pdir = dir;
        while (true)
        {
            if (isPrivateDir(pdir) == 0)
            {
                return result.ToArray();
            }
            pdir = pdir.Parent;
            if (pdir == null)
            {
                break;
            }
        }

        foreach (var file in dir.GetFiles())
        {
            if (isPrivateFile(file) == 1)
            {
                result.Add(file);
            }
        }
        return result.ToArray();
    }

    private int isPrivateDir(DirectoryInfo dir)
    {
        var result = 1;
        if (dir.Name.ToLower().StartsWith("microsoft"))
        {
            return 0;
        }
        if (dir.Name.ToLower().EndsWith("x64"))
        {
            return 0;
        }
        if (dir.Name.ToLower().EndsWith("x86"))
        {
            return 0;
        }
        return result;
    }
    private int isPrivateFile(FileInfo fi)
    {
        var result = 0;
        if (fi.Name.ToLower().EndsWith(".dll"))
        {
            return 1;
        }
        if (fi.Name.ToLower().EndsWith(".exe"))
        {
            return 1;
        }
        return result;
    }

}
