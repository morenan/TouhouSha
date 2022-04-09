using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// SkillButton.xaml 的交互逻辑
    /// </summary>
    public partial class SkillButton : ToggleButton
    {
        public SkillButton()
        {
            InitializeComponent();
        }

        #region Properties

        #region Skill

        static public DependencyProperty SkillProperty = DependencyProperty.Register(
            "Skill", typeof(Skill), typeof(SkillButton),
            new PropertyMetadata(null, OnPropertyChanged_Skill));

        public Skill Skill
        {
            get { return (Skill)GetValue(SkillProperty); }
            set { SetValue(SkillProperty, value); }
        }

        private static void OnPropertyChanged_Skill(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SkillButton) ((SkillButton)d).OnSkillChanged(e);
        }

        protected virtual void OnSkillChanged(DependencyPropertyChangedEventArgs e)
        {
            SkillInfo = Skill?.GetInfo() ?? null;
        }

        #endregion

        #region SkillInfo

        static public DependencyProperty SkillInfoProperty = DependencyProperty.Register(
            "SkillInfo", typeof(SkillInfo), typeof(SkillButton),
            new PropertyMetadata(null, OnPropertyChanged_SkillInfo));

        public SkillInfo SkillInfo
        {
            get { return (SkillInfo)GetValue(SkillInfoProperty); }
            set { SetValue(SkillInfoProperty, value); }
        }

        private static void OnPropertyChanged_SkillInfo(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SkillButton) ((SkillButton)d).OnSkillInfoChanged(e);
        }

        protected virtual void OnSkillInfoChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #endregion
    }
}
