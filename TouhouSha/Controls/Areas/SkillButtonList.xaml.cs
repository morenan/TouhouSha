using System;
using System.Collections.Generic;
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

namespace TouhouSha.Controls
{
    /// <summary>
    /// SkillButtonList.xaml 的交互逻辑
    /// </summary>
    public partial class SkillButtonList : UserControl
    {
        public SkillButtonList()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }


        #region Properties

        #region Player

        static public readonly DependencyProperty PlayerProperty = DependencyProperty.Register(
            "Player", typeof(Player), typeof(SkillButtonList),
            new PropertyMetadata(null, OnPropertyChanged_Player));

        public Player Player
        {
            get { return (Player)GetValue(PlayerProperty); }
            set { SetValue(PlayerProperty, value); }
        }

        static public void OnPropertyChanged_Player(object d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SkillButtonList) ((SkillButtonList)d).OnPlayerChanged(e);
        }

        public virtual void OnPlayerChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is Player)
            {
                Player player = (Player)(e.OldValue);
                player.Skills.CollectionChanged -= OnSkillsCollectionChanged;    
            }
            if (e.NewValue is Player)
            {
                Player player = (Player)(e.NewValue);
                player.Skills.CollectionChanged += OnSkillsCollectionChanged;
            }
            ButtonReset();
        }

        #endregion

        #endregion

        #region Number

        private GameBoard gameboard;

        #endregion 

        #region Method

        protected void ButtonReset()
        {
            Player player = Player;
            if (player == null) return;
            while (GD_Buttons.Children.Count < player.Skills.Count)
            {
                SkillButton button = new SkillButton();
                button.VerticalAlignment = VerticalAlignment.Stretch;
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                Grid.SetRow(button, GD_Buttons.Children.Count / 2);
                Grid.SetColumn(button, GD_Buttons.Children.Count & 1);
                GD_Buttons.Children.Add(button);
            }
            for (int i = 0; i < player.Skills.Count; i++)
            {
                SkillButton button = GD_Buttons.Children[i] as SkillButton;
                button.Skill = player.Skills[i];
                button.Visibility = Visibility.Visible;
            }
            for (int i = player.Skills.Count + 1; i < GD_Buttons.Children.Count; i++)
            {
                SkillButton button = GD_Buttons.Children[i] as SkillButton;
                button.Skill = null;
                button.Visibility = Visibility.Collapsed;
            }
        }
       
        public void ButtonEnable(Context ctx, Enum_GameBoardAction action)
        {
            for (int i = 0; i < GD_Buttons.Children.Count; i++)
            {
                SkillButton button = GD_Buttons.Children[i] as SkillButton;
                Skill skill = button.Skill;
                if (skill is ISkillInitative && action == Enum_GameBoardAction.CardUsing)
                    button.IsEnabled = ((ISkillInitative)skill).UseCondition?.Accept(ctx) == true;
                else if (skill is ISkillCardMultiConverter2 && IsActionUseCard(action))
                    button.IsEnabled = IsSkillCanConverter((ISkillCardMultiConverter2)skill);
                else if (skill is ISkillCardConverter && IsActionUseCard(action))
                    button.IsEnabled = ((ISkillCardConverter)skill).UseCondition?.Accept(ctx) == true;
                else
                    button.IsEnabled = false;
            }
        }

        protected bool IsActionUseCard(Enum_GameBoardAction action)
        {
            switch (action)
            {
                case Enum_GameBoardAction.CardUsing:
                case Enum_GameBoardAction.CardSelecting:
                case Enum_GameBoardAction.PlayerAndCardSelecting:
                    return true;
            }
            return false;
        }

        protected bool IsSkillCanConverter(ISkillCardMultiConverter2 conv2)
        {
            CardFilter cardfilter = gameboard?.GetSelectCardFilter();
            Context ctx = gameboard?.World?.GetContext();
            if (conv2 is ISkillCardConverter)
            {
                ISkillCardConverter conv = (ISkillCardConverter)conv2;
                if (conv.UseCondition?.Accept(ctx) != true) return false;
            }
            if (cardfilter is ICardFilterRequiredCardTypes)
            {
                ICardFilterRequiredCardTypes required = (ICardFilterRequiredCardTypes)cardfilter;
                try
                {
                    conv2.SetEnabledCardTypes(ctx, required.RequiredCardTypes);
                    return conv2.EnabledCardTypes != null && conv2.EnabledCardTypes.Count() > 0;
                }
                catch (Exception exce)
                {
                }
                finally
                {
                    conv2.CancelEnabledCardTypes(ctx);
                }
            }
            return true;
        }

        #endregion

        #region Event Handler

        private void OnSkillsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.Invoke(ButtonReset);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.gameboard = App.FindAncestor<GameBoard>(this);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.gameboard = null;
        }

        #endregion
    }
}
