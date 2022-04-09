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
using TouhouSha.Core.UIs;

namespace TouhouSha.Controls
{
    /// <summary>
    /// CharactorTooltip.xaml 的交互逻辑
    /// </summary>
    public partial class CharactorTooltip : UserControl
    {
        static public void FillText(SkillInfo skillinfo, TextBlock textblock)
        {
            StringBuilder sb = new StringBuilder();
            textblock.TextWrapping = TextWrapping.WrapWithOverflow;
            textblock.TextTrimming = TextTrimming.CharacterEllipsis;
            textblock.Inlines.Clear();
            textblock.Inlines.Add(new Run(String.Format("【{0}】", skillinfo.Name)) { Foreground = Brushes.Yellow });
            foreach (char c in skillinfo.Description)
            {
                switch (c)
                {
                    case '♥':
                    case '♦':
                        if (sb.Length > 0)
                        {
                            textblock.Inlines.Add(new Run(sb.ToString()));
                            sb.Clear();
                        }
                        textblock.Inlines.Add(new Run(c.ToString()) { Foreground = Brushes.Red });
                        break;
                    case '♣':
                    case '♠':
                        if (sb.Length > 0)
                        {
                            textblock.Inlines.Add(new Run(sb.ToString()));
                            sb.Clear();
                        }
                        textblock.Inlines.Add(new Run(c.ToString()) { Foreground = Brushes.Gray });
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            if (sb.Length > 0)
            {
                textblock.Inlines.Add(new Run(sb.ToString()));
                sb.Clear();
            }
        }


        public CharactorTooltip()
        {
            InitializeComponent();
        }

        #region Properties

        #region Core

        static public readonly DependencyProperty CoreProperty = DependencyProperty.Register(
            "Core", typeof(Charactor), typeof(CharactorTooltip),
            new PropertyMetadata(null, OnPropertyChanged_Core));

        public Charactor Core
        {
            get { return (Charactor)GetValue(CoreProperty); }
            set { SetValue(CoreProperty, value); }
        }

        private static void OnPropertyChanged_Core(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CharactorTooltip) ((CharactorTooltip)d).OnCoreChanged(e);
        }

        protected virtual void OnCoreChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Core != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(App.GetCountryName(Core.Country));
                if (Core.OtherCountries.Count() > 0)
                {
                    sb.Append("(可选择为:");
                    for (int i = 0; i < Core.OtherCountries.Count(); i++)
                    {
                        if (i > 0) sb.Append("/");
                        sb.Append(App.GetCountryName(Core.OtherCountries[i]));
                    }
                    sb.Append(")");
                }
                Info = App.GetCharactorInfo(Core);
                TL_HP.Text = String.Format("HP:{0}/{1}", Core.HP, Core.MaxHP);
                TL_Country.Text = sb.ToString();
            }
        }

        #endregion

        #region Info

        static public readonly DependencyProperty InfoProperty = DependencyProperty.Register(
            "Info", typeof(CharactorInfoCore), typeof(CharactorTooltip),
            new PropertyMetadata(null, OnPropertyChanged_Info));

        public CharactorInfoCore Info
        {
            get { return (CharactorInfoCore)GetValue(InfoProperty); }
            set { SetValue(InfoProperty, value); }
        }

        private static void OnPropertyChanged_Info(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CharactorTooltip) ((CharactorTooltip)d).OnInfoChanged(e);
        }

        protected virtual void OnInfoChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Info != null)
            {
                TL_Name.Text = Info.Name;
                SP_Skills.Children.Clear();
                foreach (SkillInfo skillinfo in Info.Skills)
                {
                    TextBlock textblock = new TextBlock();
                    textblock.Margin = new Thickness(2);
                    FillText(skillinfo, textblock);
                    SP_Skills.Children.Add(textblock);
                    if (skillinfo.AttachedSkills.Count() > 0)
                    {
                        Border border = new Border();
                        StackPanel stackpanel = new StackPanel();
                        border.Background = Brushes.Black;
                        border.BorderBrush = Brushes.White;
                        border.BorderThickness = new Thickness(1);
                        border.HorizontalAlignment = HorizontalAlignment.Stretch;
                        border.Margin = new Thickness(16,2,2,2);
                        border.Child = stackpanel;
                        SP_Skills.Children.Add(border);
                        foreach (SkillInfo subskill in skillinfo.AttachedSkills)
                        {
                            TextBlock subtext = new TextBlock();
                            subtext.Margin = new Thickness(2);
                            FillText(subskill, subtext);
                            stackpanel.Children.Add(subtext);
                        }
                    }
                }
                if (!String.IsNullOrEmpty(Info.Image?.Author))
                    TL_Image_Author.Text = String.Format("图片作者：{0}", Info.Image.Author);
                else
                    TL_Image_Author.Text = String.Empty;
                if (!String.IsNullOrEmpty(Info.Image?.PixivID))
                    TL_Image_PixivId.Text = String.Format("Pixiv ID：{0}", Info.Image.PixivID);
                else
                    TL_Image_PixivId.Text = String.Empty;
            }
        }

        #endregion

        #endregion



    }
}
