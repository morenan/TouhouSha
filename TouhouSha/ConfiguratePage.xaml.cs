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
using TouhouSha.Core;

namespace TouhouSha
{
    /// <summary>
    /// ConfiguratePage.xaml 的交互逻辑
    /// </summary>
    public partial class ConfiguratePage : UserControl
    {
        public ConfiguratePage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        #region Number 

        private MainWindow mw;
        private bool isinited;

        #endregion

        #region Event Handler

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.mw = App.FindAncestor<MainWindow>(this);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsVisibleProperty && IsVisible && !isinited)
            {
                foreach (IPackage package in App.PackageList)
                {
                    CheckBox checkbox = new CheckBox();
                    checkbox.DataContext = package;
                    checkbox.Content = package.PackageName;
                    checkbox.Margin = new Thickness(2, 2, 32, 2);
                    checkbox.IsChecked = Config.GameConfig.UsedPackages.Contains(package);
                    if (package is Koishi.Package) checkbox.IsEnabled = false;
                    if (package is Reimu.Package) checkbox.IsEnabled = false;
                    checkbox.Checked += OnPackageCheckBoxChecked;
                    checkbox.Unchecked += OnPackageCheckBoxUnchecked;
                    WP_Packages.Children.Add(checkbox);
                }
                isinited = true;
            }
        }

        private void OnPackageCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox)) return;
            CheckBox checkbox = (CheckBox)sender;
            if (!(checkbox.DataContext is IPackage)) return;
            Config.GameConfig.UsedPackages.Add((IPackage)(checkbox.DataContext));
        }

        private void OnPackageCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox)) return;
            CheckBox checkbox = (CheckBox)sender;
            if (!(checkbox.DataContext is IPackage)) return;
            Config.GameConfig.UsedPackages.Remove((IPackage)(checkbox.DataContext));

        }

        private void BN_Return_0_Click(object sender, RoutedEventArgs e)
        {
            mw?.CloseConfigPage();
        }

        private void BN_Return_1_Click(object sender, RoutedEventArgs e)
        {
            mw?.CloseConfigPage();
        }

        #endregion

    }
}
