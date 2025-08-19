using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SafeRun.WCS.IP_Controls
{
    public interface ITagLink
    {
        string Node { get;  }
    }

    public class TagLinkEnt
    {
        public string? Node { get; set; }
    }

    public class ShowTagEnt
    {
        /// <summary>
        /// 控件名称
        /// </summary>
        public string? ContrName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IList<string>? NodeNames { get; set; }
    }


    public interface ITagReader : ITagLink
    {
        /// <summary>
        /// 存放读取配置内容
        /// </summary>
        public string TagReadText { get; set; }

        /// <summary>
        /// 存放读取的所有节点
        /// </summary>
        public List<TagLinkEnt> Children { get; set; }

        /// <summary>
        /// 存放与控件绑定的节点信息
        /// </summary>
        public List<ShowTagEnt> ShowTags { get; set; }

        /// <summary>
        /// 设置读取标签内容处理
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetTagReader(string key, string content);

        /// <summary>
        /// 设置读取标签控件显示内容处理
        /// </summary>
        /// <param name="contrName"></param>
        /// <param name="contrVal"></param>
        /// <returns></returns>
        public Action SetShowControls(string contrName,string contrVal);

        /// <summary>
        /// 数据拆解,用于解析传入的节点数据进行拆分
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="contrName"></param>
        /// <param name="contrVal"></param>
        /// <returns></returns>
        public string DataDisassembly(string typeName, string contrName, string contrVal);

        /// <summary>
        /// 鼠标右击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Control_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e);

        #region 委托方法
        public Func<Dictionary<string, string>, List<(string key, string val)>> UpdAction { get; set; }
        public Func<Dictionary<string, string>, List<(string key, string val)>> DeleteAction { get; set; }
        public Func<Dictionary<string, string>, List<(string key, string val)>> CompleteAction { get; set; }
        public Func<Dictionary<string, string>,List<(string key,string val)>> QueryAction { get; set; }
        #endregion

        /// <summary>
        /// 控件WMS上编号 hyx 2025-01-22 added
        /// </summary>
        public string SystemNo {  get; set; }

    }

    public class TagActions
    {
        public const string RUN = "Run";//运行
        public const string SHOW = "Show";//显示
        public const string ALARM = "报警";
        public const string SP = "理论值";
        public const string PV = "实际值";
        public const string BYPASS = "旁通";
        public const string RAWNAME = "料名";
        public const string CAPTION = "标题";
        public const string TEXT = "文本";
        public const string ON = "开";
        public const string OFF = "关";
        public const string ONOFF = "开/关";
        public const string START = "启动";
        public const string STOP = "停止";
        public const string DEVICENAME = "设备名";
        public const string ONALARM = "开不到位";
        public const string OFFALARM = "关不到位";
        public const string LEFTALARM = "左不到位";
        public const string RIGHTALARM = "右不到位";
        public const string WARN = "提醒";
        public const string PRESS = "按下按钮";
        public const string HIGHLEVEL = "高料位";
        public const string LOWLEVEL = "低料位";
        public const string SPEED = "速度";
        public const string AMPS = "电流";
        public const string LEFT = "左";
        public const string RIGHT = "右";
        public const string MID = "中";
        public const string STATE = "状态变化";
        public const string STATE1 = "状态1";
        public const string STATE2 = "状态2";
        public const string STATE3 = "状态3";
        public const string STATE4 = "状态4";
        public const string STATE5 = "状态5";
        public const string STATE6 = "状态6";
        public const string STATE7 = "状态7";
        public const string STATE8 = "状态8";
        public const string VISIBLE = "可见性";
        public const string ENABLE = "使能";
        public const string DISABLE = "失效";
    }


   
}
