using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Controls
{
    /// <summary>
    /// ListPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ListPanel : BoxedControl
    {
        public ListPanel()
        {
            InitializeComponent();
            SelectedItems = new ObservableCollection<object>();
        }

        #region Properties

        #region Core

        static public DependencyProperty CoreProperty = DependencyProperty.Register(
            "Core", typeof(ListBoardCore), typeof(ListPanel),
            new PropertyMetadata(null, OnPropertyChanged_Core));

        public ListBoardCore Core
        {
            get { return (ListBoardCore)GetValue(CoreProperty); }
            set { SetValue(CoreProperty, value); }
        }

        private static void OnPropertyChanged_Core(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListPanel) ((ListPanel)d).OnCoreChanged(e);
        }

        protected virtual void OnCoreChanged(DependencyPropertyChangedEventArgs e)
        {
            SelectedItems.Clear();
            if (Core == null) return;
            while (SP_Buttons.Children.Count <= Core.Items.Count())
            {
                Border border = new Border();
                CustomButton button = new CustomButton();
                border.BorderBrush = Brushes.White;
                border.BorderThickness = new Thickness(0);
                border.HorizontalAlignment = HorizontalAlignment.Stretch;
                border.Child = button;
                button.Height = 24;
                button.MinWidth = 100;
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.Click += OnButtonClick;
                SP_Buttons.Children.Add(border);
            }
            for (int i = 0; i < Core.Items.Count(); i++)
            {
                Border border = SP_Buttons.Children[i] as Border;
                CustomButton button = border?.Child as CustomButton;
                if (button == null) continue;
                border.Visibility = Visibility.Visible;
                button.DataContext = Core.Items[i];
                if (button.DataContext is Player)
                    button.Content = ((Player)(button.DataContext)).Name;
                else if (button.DataContext is Skill)
                    button.Content = ((Skill)(button.DataContext)).GetInfo()?.Name;
                else if (button.DataContext is Card)
                    button.Content = App.GetCardInfo((Card)(button.DataContext))?.Name;
                else
                    button.Content = button.DataContext?.ToString();
            }
            for (int i = Core.Items.Count(); i < SP_Buttons.Children.Count; i++)
            {
                FrameworkElement fe = SP_Buttons.Children[i] as FrameworkElement;
                if (fe == null) continue;
                fe.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region SelectedItems

        static public DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems", typeof(IList<object>), typeof(ListPanel),
            new PropertyMetadata(null, OnPropertyChanged_SelectedItems));

        public IList<object> SelectedItems
        {
            get { return (IList<object>)GetValue(SelectedItemsProperty); }
            private set { SetValue(SelectedItemsProperty, value); }
        }

        private static void OnPropertyChanged_SelectedItems(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListPanel) ((ListPanel)d).OnSelectedItemsChanged(e);
        }

        protected virtual void OnSelectedItemsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyCollectionChanged)
            {
                INotifyCollectionChanged oldvalue = (INotifyCollectionChanged)(e.OldValue);
                oldvalue.CollectionChanged -= OnSelectionChanged;
            }
            if (e.NewValue is INotifyCollectionChanged)
            {
                INotifyCollectionChanged newvalue = (INotifyCollectionChanged)(e.NewValue);
                newvalue.CollectionChanged += OnSelectionChanged;
            }
        }

        #endregion

        #endregion

        #region Event Handler

        public event RoutedEventHandler SelectedDone;

        private void OnSelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            for (int i = 0; i < SP_Buttons.Children.Count; i++)
            {
                Border border = SP_Buttons.Children[i] as Border;
                CustomButton button = border?.Child as CustomButton;
                object data = button?.DataContext;
                if (data == null) continue;
                border.BorderThickness = new Thickness(SelectedItems.Contains(data) ? 2 : 0);
            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is CustomButton)) return;
            CustomButton button = (CustomButton)sender;
            if (button.DataContext == null) return;
            if (SelectedItems.Contains(button.DataContext))
                SelectedItems.Remove(button.DataContext);
            else
                SelectedItems.Add(button.DataContext);
            if (Core != null && SelectedItems.Count >= Core.SelectMax)
                SelectedDone?.Invoke(this, e);
        }

        #endregion

    }
}
