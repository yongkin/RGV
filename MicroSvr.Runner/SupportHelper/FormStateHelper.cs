namespace MicroSvr.WPFRunner;

class FormStateHelper
{
    #region 单例模式
    //private static readonly object lockobject = new object();
    public static FormStateHelper Instance { get { return RunnerInstanceHelper.Create<FormStateHelper>(); } }
    /// <summary>
    /// 构造函数
    /// </summary>
    private FormStateHelper()
    {
        try
        {
            setCurrentFormState();
        }
        catch { }
    }
    #endregion

    public void SetFormState(Window frm)
    {
        if (frm == null)
        {
            return;
        }
        setFormState(frm);
    }

    private void setCurrentFormState()
    {
        setFormState(StartFrm.Instance);
    }

    private void setFormState(Window frm)
    {
        if (frm == null)
        {
            return;
        }
        frm.Icon = RunHelper.Instance.AppIcon;
        frm.WindowState = WindowState.Maximized;
    }

}
