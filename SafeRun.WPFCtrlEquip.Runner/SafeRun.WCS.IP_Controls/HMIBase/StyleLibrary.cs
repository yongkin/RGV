using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SafeRun.WCS.IP_Controls
{
    public static class ColorLibrary
    {
        public static readonly SolidColorBrush DefaultBackgroundColor = new SolidColorBrush(Colors.WhiteSmoke);
        public static readonly SolidColorBrush RedColorB = new SolidColorBrush(Colors.Red);
        public static readonly Color RedColor = Colors.Red;

        public static readonly SolidColorBrush Blue = new SolidColorBrush(Colors.Blue);
        // Add more colors as needed
        //报警颜色
        public static readonly SolidColorBrush AlarmColorB = new SolidColorBrush(Colors.Red);
        public static readonly Color AlarmColor = (Colors.Red);
        public static readonly SolidColorBrush AlarmDefaultColor = new SolidColorBrush(Colors.White);

        //边框颜色
        public static readonly SolidColorBrush BorderColorB = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4472C4"));//wpf 中是32位颜色 ARGB
        
        public static readonly Color BorderColor = ((Color)ColorConverter.ConvertFromString("#FF4472C4"));//wpf 中是32位颜色 ARGB

        /// <summary>
        /// 深灰色，地轨色
        /// </summary>
        public static readonly SolidColorBrush GroundTrackB = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF585555"));

        /// <summary>
        /// 堆垛机-橙
        /// </summary>
        public static readonly SolidColorBrush CrnOrangeB = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF48F1F"));


        //设备模式颜色
        public static readonly SolidColorBrush AutoModeColor = new SolidColorBrush(Colors.White);
        public static readonly SolidColorBrush MoveModeColor = new SolidColorBrush(Colors.LightGray);
        public static readonly SolidColorBrush AutoRunColorB = new SolidColorBrush(Colors.Lime);
        public static readonly Color AutoRunColor = Colors.Lime;

        public static readonly SolidColorBrush ManualRunColorB = new SolidColorBrush(Colors.Orange);//手动运行-橙色
        public static readonly Color ManualRunColor = Colors.Orange;

        //传感器激活颜色
        public static readonly SolidColorBrush SensorActiveColor = new SolidColorBrush(Colors.Lime);

        //线体有载颜色
        public static readonly SolidColorBrush CVLoadedColor = new SolidColorBrush(Colors.Lime);

        public static string GetColorStr(SolidColorBrush color)
        {
            return color.ToString();
        }
    }
}
