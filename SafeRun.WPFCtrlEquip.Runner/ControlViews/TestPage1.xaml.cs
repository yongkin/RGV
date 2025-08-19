

using System.Windows.Shapes;

namespace SafeRun.WPFCtrlEquip.WPFRunner.ControlViews
{
    /// <summary>
    /// TestPage1.xaml 的交互逻辑
    /// </summary>
    public partial class TestPage1 : BasePageClass
    {
        public TestPage1()
        {
            InitializeComponent();
        }

        private Storyboard _alarmOnStoryboard = null;
        private Storyboard _alarmOffStoryboard = null;

        public void SetAlarmOnAnimation()
        {
            VisualStateManager.GoToState(this, "AlarmOn", true);
            if (_alarmOnStoryboard == null) InitAlarmAnimation();
            _alarmOnStoryboard.Begin();
        }

        public void SetAlarmOffAnimation()
        {
            VisualStateManager.GoToState(this, "AlarmOff", true);
            if (_alarmOffStoryboard == null) InitAlarmAnimation();
            _alarmOffStoryboard.Begin();
        }

        private void InitAlarmAnimation()
        {
            var rect = (Rectangle)this.Template.FindName("rect", this);
            _alarmOffStoryboard = CreateAlarmAnimation(ColorLibrary.AlarmDefaultColor, rect);
            _alarmOnStoryboard = CreateAlarmAnimation(ColorLibrary.AlarmColorB, rect);
        }

        private Storyboard CreateAlarmAnimation(SolidColorBrush colorBrushs, Rectangle rect)
        {
            Storyboard storyboard = new Storyboard();

            // 创建颜色变化的动画
            ObjectAnimationUsingKeyFrames colorAnimation = new ObjectAnimationUsingKeyFrames
            {
                Duration = TimeSpan.FromSeconds(0.5)
            };
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
                Value = colorBrushs // Alarm Color
            });
            //var rect = (Rectangle)this.Template.FindName("rect", this);
            Storyboard.SetTarget(colorAnimation, rect);
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(Shape.FillProperty));

            storyboard.Children.Add(colorAnimation);
            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            return storyboard;
            //storyboard.Begin();
        }

        private void StackerShelves_1_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
