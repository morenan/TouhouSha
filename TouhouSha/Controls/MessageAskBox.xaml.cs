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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TouhouSha.Controls
{
    /// <summary>
    /// MessageAskBox.xaml 的交互逻辑
    /// </summary>
    public partial class MessageAskBox : UserControl
    {
        public MessageAskBox()
        {
            InitializeComponent();
        }

        #region Properties

        #region Message

        static public readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(MessageAskBox),
            new PropertyMetadata(String.Empty));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        #endregion

        #region IsAsking

        static public readonly DependencyProperty IsAskingProperty = DependencyProperty.Register(
            "IsAsking", typeof(bool), typeof(MessageAskBox),
            new PropertyMetadata(false));

        public bool IsAsking
        {
            get { return (bool)GetValue(IsAskingProperty); }
            set { SetValue(IsAskingProperty, value); }
        }

        #endregion

        #region TimeValue

        static public readonly DependencyProperty TimeValueProperty = DependencyProperty.Register(
            "TimeValue", typeof(double), typeof(MessageAskBox),
            new PropertyMetadata(0.0d));

        public double TimeValue
        {
            get { return (double)GetValue(TimeValueProperty); }
            set { SetValue(TimeValueProperty, value); }
        }

        #endregion

        #region Timeout

        static public readonly DependencyProperty TimeoutProperty = DependencyProperty.Register(
            "Timeout", typeof(double), typeof(MessageAskBox),
            new PropertyMetadata(15.0d));

        public double Timeout
        {
            get { return (double)GetValue(TimeoutProperty); }
            set { SetValue(TimeoutProperty, value); }
        }

        #endregion

        #region TimeAnim

        static public readonly DependencyProperty TimeAnimProperty = DependencyProperty.Register(
            "TimeAnim", typeof(DoubleAnimation), typeof(MessageAskBox),
            new PropertyMetadata(null));

        public DoubleAnimation TimeAnim
        {
            get { return (DoubleAnimation)GetValue(TimeAnimProperty); }
            set { SetValue(TimeAnimProperty, value); }
        }

        #endregion

        #endregion

        #region Event Handler

        public event RoutedEventHandler Yes;

        public event RoutedEventHandler No;

        public event EventHandler Timeouted;

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsAskingProperty)
            {
                BN_Yes.Visibility = BN_No.Visibility = IsAsking ? Visibility.Visible : Visibility.Collapsed;
                BN_Yes.IsEnabled = BN_No.IsEnabled = IsAsking;
                if (IsAsking)
                {
                    DoubleAnimation timeanim = new DoubleAnimation();
                    timeanim.From = Timeout;
                    timeanim.To = 0;
                    timeanim.Duration = new Duration(TimeSpan.FromSeconds(Timeout));
                    timeanim.Completed += OnTimeout;
                    TimeAnim = timeanim;
                }
            }
            if (e.Property == TimeAnimProperty)
            {
                if (e.OldValue is DoubleAnimation)
                {
                    DoubleAnimation oldvalue = (DoubleAnimation)(e.OldValue);
                    oldvalue.Completed -= OnTimeout;
                }
                if (e.NewValue is DoubleAnimation)
                {
                    DoubleAnimation newvalue = (DoubleAnimation)(e.NewValue);
                    newvalue.Completed += OnTimeout;
                    BeginAnimation(TimeValueProperty, newvalue, HandoffBehavior.SnapshotAndReplace);
                }
            }
        }

        private void OnTimeout(object sender, EventArgs e)
        {
            TimeAnim = null;
            Timeouted?.Invoke(this, e);
        }

        private void BN_Yes_Click(object sender, RoutedEventArgs e)
        {
            Yes?.Invoke(this, e);
        }

        private void BN_No_Click(object sender, RoutedEventArgs e)
        {
            No?.Invoke(this, e);
        }

        #endregion

    }
}
