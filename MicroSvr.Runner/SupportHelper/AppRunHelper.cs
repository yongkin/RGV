
namespace MicroSvr.WPFRunner;
class AppRunHelper
{
    #region 单例模式
    //private static readonly object lockobject = new object();
    public static AppRunHelper Instance { get { return RunnerInstanceHelper.Create<AppRunHelper>(); } }
    /// <summary>
    /// 构造函数
    /// </summary>
    private AppRunHelper()
    {
    }
    #endregion

    public Window GetAppForm()
    {
        try
        {
            appException();
            return getAppForm();
        }
        catch (Exception ex)
        {
        }
        return null;
    }

    #region getAppForm
    private Window getAppForm()
    {
        var frm = AppFormHelper.Instance.GetAppForm();
        HandleFileHelper.Instance.Save(frm);
        FormStateHelper.Instance.SetFormState(frm);
        return frm;
    }
    #endregion

    #region ThreadException

    /// <summary>
    /// 未捕获异常处理
    /// </summary>
    private void appException()
    {
        Application app = Application.Current;
        try { app.DispatcherUnhandledException += Application_ThreadException; } catch { }
        //try { app.ThreadException += Application_ThreadException; } catch { }
        //try { Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException); } catch { }
        //try { AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; } catch { }
    }

    private void showExceptionFrm(Exception ex)
    {
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        StartLogFileHelper.Instance.StartException($"系统未处理异常", ex);
        showExceptionFrm(ex);
    }

    private void Application_ThreadException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var ex = e.Exception as Exception;
        StartLogFileHelper.Instance.StartException($"进程未处理异常", ex);
        showExceptionFrm(ex);
    }
    #endregion
}
