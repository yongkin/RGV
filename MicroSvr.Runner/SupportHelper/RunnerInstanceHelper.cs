namespace MicroSvr.WPFRunner;
static class RunnerInstanceHelper
{
    #region getInstance
    private static ConcurrentDictionary<Type, object> xstore = new ConcurrentDictionary<Type, object>();
    private static object createInstance(Type type)
    {
        var constructorInfoArray = type.GetConstructors(System.Reflection.BindingFlags.Instance
            | System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Public);
        foreach (var constructorInfo in constructorInfoArray)
        {
            if (0 == constructorInfo.GetParameters().Length)
            {
                return constructorInfo.Invoke(null);
            }
        }
        return null;
    }
    /// <summary>
    /// 获取单例
    /// </summary>
    /// <param name="type"></param>
    /// <param name="action"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    private static object createInstance(Type type, Action action, Func<object> func)
    {
        object result = null;
        if (xstore.TryGetValue(type, out result))
        {
            return result;
        }
        lock (type)
        {
            if (xstore.TryGetValue(type, out result))
            {
                return result;
            }

            action?.Invoke();
            var f = func?.Invoke();
            if (f != null && type.IsAssignableFrom(f.GetType()))
            {
                result = f;
            }
            else
            {
                result = createInstance(type);
            }
            if (result == null)
            {
                return null;
            }
            xstore.AddOrUpdate(type, result, (k, a) => { return result; });
            return result;

        }
    }
    /// <summary>
    /// 获取单例
    /// </summary>
    /// <param name="type"></param>
    /// <param name="action"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    private static object getInstance(Type type, Action action, Func<object> func)
    {
        try
        {
            return createInstance(type, action, func);
        }
        catch
        {
            return null;
        }
    }
    /// <summary>
    /// 获取单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <param name="func"></param>
    /// <returns></returns>

    private static T getInstance<T>(Action action, Func<object> func)
    {
        try
        {
            var result = getInstance(typeof(T), action, func);
            if (result is T)
            {
                return (T)result;
            }
            return default(T);
        }
        catch
        {
            return default(T);
        }
    }
    #endregion

    public static T Create<T>()
    {
        return getInstance<T>(null, null);
    }

    public static T Create<T>(Action action)
    {
        return getInstance<T>(action, null);
    }

    public static T Create<T>(Func<object> func)
    {
        return getInstance<T>(null, func);
    }

    public static object Create(Type type)
    {
        return getInstance(type, null, null);
    }

    public static object Create(Type type, Action action)
    {
        return getInstance(type, action, null);
    }

    public static object Create(Type type, Func<object> func)
    {
        return getInstance(type, null, func);
    }
    public static void Remove(Type type)
    {
        try { xstore.TryRemove(type, out _); } catch { return; }
    }
    public static void Remove<T>()
    {
        Remove(typeof(T));
    }


}
