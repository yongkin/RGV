

namespace MicroSvr.WPFRunner;
static class AppResourceAssembly
{
    private static ConcurrentDictionary<string, Assembly> xstore = new ConcurrentDictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);
    public static void Resolve()
    {
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
         {
             try
             {
                 return AssemblyResolve(null, args);
             }
             catch
             {
                 return null;
             }
         };
    }

    private static Assembly AssemblyResolve(MethodBase method, ResolveEventArgs args)
    {
        //获取加载的程序集的全名
        var assName = new AssemblyName(args.Name);
        if (xstore.TryGetValue(assName.Name, out Assembly result))
        {
            return result;
        }
        Assembly assm = null;
        if (method == null)
        {
            assm = typeof(AppResourceAssembly).Assembly;
        }
        else
        {
            assm = method.ReflectedType.Assembly;
        }
        //读取资源
        using (var stream = assm.GetManifestResourceStream($"{assm.GetName().Name}.Resources.Assembly.{assName.Name}.dll"))
        {
            if (stream == null)
            {
                xstore.TryAdd(assName.Name, null);
                return null;
            }
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);
            result = Assembly.Load(bytes);
            if (result != null)
            {
                xstore.AddOrUpdate(assName.Name, result, (k, a) => { return result; });
                return result;
            }
        }
        return null;
    }
}
