using System.IO;

namespace MicroSvr.WPFRunner;
/// <summary>
/// 程序应用程序文件夹
/// </summary>
class PrivatePath
{

    public void sendMsg(string msg)
    {
        try
        {
            var typeName = GetType().Namespace + ".StartLogFileHelper," + GetType().Namespace;
            var type = Type.GetType(typeName, false, true);
            if (type == null)
            {
                return;
            }

            var p = type.GetProperty("Instance", BindingFlags.NonPublic | BindingFlags.Public
                | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (p == null)
            {
                return;
            }
            var instance = p.GetValue(null);
            if (instance == null)
            {
                return;
            }
            var method = type.GetMethod("StartMsg", new Type[] { typeof(string) });
            if (method == null)
            {
                return;
            }
            method.Invoke(instance, new object[] { msg });
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 设置应用程序文件夹
    /// </summary>
    /// <returns></returns>
    public void AppendPrivatePaths()
    {
        var paths = PrivatePathHelper.Instance.PrivatePaths;
        foreach (var dir in paths)
        {
            sendMsg($"PrivatePath:{dir.FullName}");
            //tryRun(() => appendPrivatePathsByDependencyContext(dir));
            appendPrivatePathsByDependencyContext(dir);
        }
    }

    /// <summary>
    /// 设置应用程序文件夹
    /// </summary>
    /// <param name="dir"></param>
    private void appendPrivatePathsByDependencyContext(DirectoryInfo dir)
    {

        foreach (var f in dir.GetFiles())
        {
            try
            {
                if (f.Extension.ToLower() == ".dll")
                {
                    var assm = Assembly.LoadFrom(f.FullName);
                    if (assm != null)
                    {
                        object value = Microsoft.Extensions.DependencyModel.DependencyContext.Load(assm);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            };
        }


    }
    public static void tryRun(Action action)
    {
        try
        {
            action();
        }
        catch (Exception Ex)
        {
            Console.WriteLine(Ex.Message);
        }
    }

    /// <summary>
    /// 设置应用程序文件夹
    /// </summary>
    /// <param name="dir"></param>
    private void appendPrivatePathsByAppDomain(DirectoryInfo dir)
    {
        //var assemblyContext = AssemblyLoadContext.GetAssemblyContext(typeof(Program).Assembly);
        //assemblyContext.AppendPrivatePath("YourPathHere");
        //AppDomain.CurrentDomain.AppendPrivatePath(dir.FullName);
    }
    /// <summary>
    /// 设置应用程序文件夹  反射
    /// 暂不使用
    /// </summary>
    /// <param name="path"></param>
    private void appendPrivatePathsByFusionContext(DirectoryInfo dir)
    {
        var path = dir.FullName;
        AppDomain.CurrentDomain.SetData("PRIVATE_BINPATH", path);
        AppDomain.CurrentDomain.SetData("BINPATH_PROBE_ONLY", path);
        var m = typeof(AppDomainSetup).GetMethod("UpdateContextProperty", BindingFlags.NonPublic | BindingFlags.Static);
        var funsion = typeof(AppDomain).GetMethod("GetFusionContext", BindingFlags.NonPublic | BindingFlags.Instance);
        m.Invoke(null, new object[] { funsion.Invoke(AppDomain.CurrentDomain, null), "PRIVATE_BINPATH", path });
    }
}
