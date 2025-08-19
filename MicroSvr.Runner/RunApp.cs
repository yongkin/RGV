

namespace MicroSvr.WPFRunner;
public class RunApp
{
    /// <summary>
    /// 主程序启动
    /// </summary>
    /// <param name="args"></param>
    public static void AppMain(string[] args)
    {
        System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        if (args == null)
        {
            args = new string[0];
        }
        AppResourceAssembly.Resolve();
        new RunAppHelper().AppMain(args);
    }
}
class RunAppHelper
{
    /// <summary>
    /// 主程序启动
    /// </summary>
    /// <param name="args"></param>
    public void AppMain(string[] args)
    {
        var datetime = DateTime.Now;
        var msg = new StringBuilder();
        msg.Append($"{datetime.ToString("yyyy-MM-dd HH:mm:ss")} >> 应用程序启动").AppendLine();
        if (args != null)
        {
            for (var i = 0; i < args.Length; i++)
            {
                msg.Append($"                   arg[{i}]={args[i]}").AppendLine();
            }
            Console.WriteLine(msg.ToString());
        }
        try
        {
            runApp(args);
        }
        catch (Exception ex)
        {
            msg = new StringBuilder();
            msg.Append($"{datetime.ToString("yyyy-MM-dd HH:mm:ss")} >> 应用程序异常").AppendLine();
            msg.Append(ex.ToString()).AppendLine();
            Console.WriteLine(msg.ToString());
        }
        msg = new StringBuilder();
        msg.Append($"{datetime.ToString("yyyy-MM-dd HH:mm:ss")} >> 应用程序退出").AppendLine();
        Console.WriteLine(msg.ToString());
        Console.ReadLine();
    }



    #region MicroSvr.Runner.RunHelper.Instance.RunApp

    private void runApp(string[] args)
    {
        MicroSvr.WPFRunner.RunHelper.Instance.ApplicationArgs = args;
        MicroSvr.WPFRunner.RunHelper.Instance.RunApp(string.Empty);
    }
    #endregion

}
