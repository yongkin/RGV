using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SafeRun.WPFCtrlEquip.WPFRunner.CustomizeControls
{
    /// <summary>
    /// DateTimeControls.xaml 的交互逻辑
    /// </summary>
    public partial class DateTimeControls : UserControl
    {
        
        public DateTimeControls()
        {
            InitializeComponent();

            hourTextBox.PreviewTextInput += ValidateNumericInput;
            minuteTextBox.PreviewTextInput += ValidateNumericInput;
            hourTextBox.LostFocus += ValidateHourInput;
            minuteTextBox.LostFocus += ValidateMinuteInput;

            //SelectedDateTime = DateTime.Now;
        }


        // 限制只能输入数字
        private void ValidateNumericInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        // 验证小时输入
        private void ValidateHourInput(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(hourTextBox.Text, out int hour))
            {
                if (hour < 0 || hour > 23)
                {
                    MessageBox.Show("小时必须在 0 到 23 之间！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                    hourTextBox.Text = $"{0:D2}";
                }
            }
            else
            {
                hourTextBox.Text = $"{0:D2}";
            }
        }

        // 验证分钟输入
        private void ValidateMinuteInput(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(minuteTextBox.Text, out int minute))
            {
                if (minute < 0 || minute > 59)
                {
                    MessageBox.Show("分钟必须在 0 到 59 之间！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                    minuteTextBox.Text = $"{0:D2}";
                }
            }
            else
            {
                minuteTextBox.Text = $"{0:D2}";
            }
        }

        // 公开一个属性来获取或设置选中的日期时间
        //public DateTime? SelectedDateTime
        //{
        //    get
        //    {
        //        if (datePicker.SelectedDate.HasValue &&
        //            int.TryParse(hourTextBox.Text, out int hour) &&
        //            int.TryParse(minuteTextBox.Text, out int minute))
        //        {
        //            return datePicker.SelectedDate.Value.AddHours(hour - 1).AddMinutes(minute - 1);
        //        }
        //        return null;
        //    }
        //    set
        //    {
        //        if (value.HasValue)
        //        {
        //            datePicker.SelectedDate = value.Value.Date;
        //            hourTextBox.Text = $"{value.Value.Hour:D2}";
        //            minuteTextBox.Text = $"{value.Value.Minute:D2}";
        //        }
        //        else
        //        {
        //            datePicker.SelectedDate = null;
        //            hourTextBox.Text = $"{0:D2}";
        //            minuteTextBox.Text = $"{0:D2}";
        //        }
        //    }
        //}
    }
}
