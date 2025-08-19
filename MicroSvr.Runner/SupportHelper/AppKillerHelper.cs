namespace MicroSvr.WPFRunner;
class AppKillerHelper
{
    #region 单例模式
    //private static readonly object lockobject = new object();
    public static AppKillerHelper Instance { get { return RunnerInstanceHelper.Create<AppKillerHelper>(); } }
    /// <summary>
    /// 构造函数
    /// </summary>
    private AppKillerHelper()
    {
    }
    #endregion

    /// <summary>
    /// 关闭自身
    /// </summary>

    public void KillSelf()
    {
        try
        {
            //Application.ExitThread();
            Application.Current.Shutdown();
        }
        catch (Exception ex)
        {

        }
        try
        {
            var p = Process.GetCurrentProcess();
            if (p != null)
            {
                p.Kill();
            } 
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 延时关闭
    /// </summary>
    public void AfterKillSelf()
    {
        Task.Run(() =>
        {
            Thread.Sleep(5 * 1000);
            KillSelf();
        });
    }
}
