namespace MicroSvr.WPFRunner;
class AppFormHelper
{
    #region 单例模式
    //private static readonly object lockobject = new object();
    public static AppFormHelper Instance { get { return RunnerInstanceHelper.Create<AppFormHelper>(); } }
    /// <summary>
    /// 构造函数
    /// </summary>
    private AppFormHelper()
    {
    }
    #endregion

    /// <summary>
    /// 获取主界面
    /// </summary>
    /// <returns></returns>
    public Window GetAppForm()
    {
        try
        {
            return getAppForm();
        }
        catch (Exception ex)
        {
        }
        return null;
    }

    /// <summary>
    /// 获取主界面
    /// </summary>
    /// <returns></returns>
    private Window getAppForm()
    {
        Window result = null;
        try
        {
            //获取*.Runner.dll 的MainFrm
            result = RunnerFrmHelper.Instance.GetAppForm();
            if (result != null)
            {
                return result;
            }
        }
        catch (Exception ex)
        {
            StartLogFileHelper.Instance.StartException(ex);
        }
        try
        {
            //获取*.Test.dll 的MainFrm
            result = TestFrmHelper.Instance.GetAppForm();
            if (result != null)
            {
                return result;
            }
        }
        catch (Exception ex)
        {
            StartLogFileHelper.Instance.StartException(ex);
        }
        try
        {
            //获取根目录Comm.*.dll 的MainFrm
            result = LocalFrmHelper.Instance.GetAppForm();
            if (result != null)
            {
                return result;
            }
        }
        catch (Exception ex)
        {
            StartLogFileHelper.Instance.StartException(ex);
        }
        try
        {
            //获取子目录MicroSvr.*.dll 的MainFrm
            result = LocalSubFrmHelper.Instance.GetAppForm();
            if (result != null)
            {
                return result;
            }
        }
        catch (Exception ex)
        {
            StartLogFileHelper.Instance.StartException(ex);
        }
        return StartFrm.Instance;
    }

}
