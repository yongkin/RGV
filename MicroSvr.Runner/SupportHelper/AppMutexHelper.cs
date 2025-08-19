using System.Runtime.InteropServices;

namespace MicroSvr.WPFRunner;
class AppMutexHelper
{
    /// <summary>
    /// 进程只启动一次
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public bool IsExist(string guid)
    {
        try
        {
            guid = guid.Replace("\\", "|");
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
            {
                return new MutexWinCE().IsExist(guid);
            }
            else
            {
                return new MutexWin32().IsExist(guid);
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}

#region 实现
#region WinCE
class MutexWinCE
{
    [DllImport("coredll.dll", EntryPoint = "CreateMutex", SetLastError = true)]
    public static extern IntPtr CreateMutex(
            IntPtr lpMutexAttributes,
            bool InitialOwner,
            string MutexName);

    [DllImport("coredll.dll", EntryPoint = "ReleaseMutex", SetLastError = true)]
    public static extern bool ReleaseMutex(IntPtr hMutex);

    private const int ERROR_ALREADY_EXISTS = 0183;

    /// <summary>
    /// 判断程序是否已经运行
    /// </summary>
    /// <returns>
    /// true: 程序已运行，则什么都不做
    /// false: 程序未运行，则启动程序
    /// </returns>
    internal bool IsExist(string guid)
    {
        if (Environment.OSVersion.Platform == PlatformID.WinCE)
        {
            IntPtr hMutex = CreateMutex(IntPtr.Zero, true, guid);
            if (hMutex == IntPtr.Zero)
                throw new ApplicationException("Failure creating mutex:"
                + Marshal.GetLastWin32Error().ToString("X"));

            if (Marshal.GetLastWin32Error() == ERROR_ALREADY_EXISTS)
            {
                ReleaseMutex(hMutex);
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }
}
#endregion

#region Win32
class MutexWin32
{
    [DllImport("kernel32.dll", EntryPoint = "CreateMutex", SetLastError = true)]
    public static extern IntPtr CreateMutex(
            IntPtr lpMutexAttributes,
            bool InitialOwner,
            string MutexName);

    [DllImport("kernel32.dll", EntryPoint = "ReleaseMutex", SetLastError = true)]
    public static extern bool ReleaseMutex(IntPtr hMutex);

    private const int ERROR_ALREADY_EXISTS = 0183;

    /// <summary>
    /// 判断程序是否已经运行
    /// </summary>
    /// <returns>
    /// true: 程序已运行，则什么都不做
    /// false: 程序未运行，则启动程序
    /// </returns>
    internal bool IsExist(string guid)
    {
        IntPtr hMutex = CreateMutex(IntPtr.Zero, true, guid);
        if (hMutex == IntPtr.Zero)
            throw new ApplicationException("Failure creating mutex:"
            + Marshal.GetLastWin32Error().ToString("X"));

        if (Marshal.GetLastWin32Error() == ERROR_ALREADY_EXISTS)
        {
            ReleaseMutex(hMutex);
            return true;
        }
        return false;
    }
}
#endregion
#endregion

