using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
using TouhouSha.Core;

namespace TouhouSha.Controls
{
    /// <summary>
    /// CardView.xaml 的交互逻辑
    /// </summary>
    public partial class CardView : UserControl
    {
        static public double DefaultWidth = 72;
        static public double DefaultHeight = 100;

        public CardView()
        {
            InitializeComponent();
            Anims = new ObservableCollection<CardAnim>();
            Width = DefaultWidth;
            Height = DefaultHeight;
        }

        #region Properties

        #region Card

        static public readonly DependencyProperty CardProperty = DependencyProperty.Register(
            "Card", typeof(Card), typeof(CardView),
            new PropertyMetadata(null, OnPropertyChanged_Card));
        
        public Card Card
        {
            get { return (Card)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }

        private static void OnPropertyChanged_Card(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardView) ((CardView)d).OnCardChanged(e);
        }

        protected virtual void OnCardChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is Card)
            {
                Card oldcard = (Card)(e.OldValue);
                oldcard.PropertyChanged -= OnCardPropertyChanged;
            }
            if (e.NewValue is Card)
            {
                Card newcard = (Card)(e.NewValue);
                newcard.PropertyChanged += OnCardPropertyChanged;
            }
            if (Card != null)
            {
                //IM_Card.Source = App.GetCardImage(Card.KeyName);
                IM_Card.Source = App.GetCardInfo(Card)?.Image?.Source;
                IM_FaceDown.Source = App.FaceDownCardImage;
                HT_Color.Text = App.GetCardColorEmojiText(Card.CardColor);
                HT_Color.Foreground = App.GetCardWpfColor(Card.CardColor);
                HT_Point.Text = App.GetCardPokerPoint(Card.CardPoint);
                HT_Point.Foreground = App.GetCardWpfColor(Card.CardColor);
                HT_Comment.Text = Card.Comment;
            }
        }

        private void OnCardPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Comment":
                    Dispatcher.Invoke(() => { HT_Comment.Text = Card?.Comment ?? String.Empty; });
                    break;
            }
        }

        #endregion

        #region IsFaceDown

        static public readonly DependencyProperty IsFaceDownProperty = DependencyProperty.Register(
            "IsFaceDown", typeof(bool), typeof(CardView),
            new PropertyMetadata(false, OnPropertyChanged_IsFaceDown));

        public bool IsFaceDown
        {
            get { return (bool)GetValue(IsFaceDownProperty); }
            set { SetValue(IsFaceDownProperty, value); }
        }

        private static void OnPropertyChanged_IsFaceDown(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardView) ((CardView)d).OnIsFaceDownChanged(e);
        }

        protected virtual void OnIsFaceDownChanged(DependencyPropertyChangedEventArgs e)
        {
            IM_FaceDown.Visibility = IsFaceDown ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Position
        
        static public readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position", typeof(Point), typeof(CardView),
            new PropertyMetadata(new Point(0, 0), OnPropertyChanged_Position));

        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        private static void OnPropertyChanged_Position(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardView) ((CardView)d).OnPositionChanged(e);
        }

        protected virtual void OnPositionChanged(DependencyPropertyChangedEventArgs e)
        {
            Canvas.SetLeft(this, Position.X);
            Canvas.SetTop(this, Position.Y);
        }

        #endregion

        #region Comment

        static public readonly DependencyProperty CommentProperty = DependencyProperty.Register(
            "Comment", typeof(string), typeof(CardView),
            new PropertyMetadata(String.Empty, OnPropertyChanged_Comment));

        public string Comment
        {
            get { return (string)GetValue(CommentProperty); }
            set { SetValue(CommentProperty, value); }
        }

        private static void OnPropertyChanged_Comment(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardView) ((CardView)d).OnCommentChanged(e);
        }

        protected virtual void OnCommentChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region Anims

        static public readonly DependencyProperty AnimsProperty = DependencyProperty.Register(
            "Anims", typeof(ObservableCollection<CardAnim>), typeof(CardView),
            new PropertyMetadata(null, OnPropertyChanged_Anims));

        public ObservableCollection<CardAnim> Anims
        {
            get { return (ObservableCollection<CardAnim>)GetValue(AnimsProperty); }
            private set { SetValue(AnimsProperty, value); }
        }

        private static void OnPropertyChanged_Anims(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardView) ((CardView)d).OnAnimsChanged(e);
        }

        protected virtual void OnAnimsChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region About Selection System

        #region IsEnterSelecting

        static public readonly DependencyProperty IsEnterSelectingProperty = DependencyProperty.Register(
            "IsEnterSelecting", typeof(bool), typeof(CardView),
            new PropertyMetadata(false, OnPropertyChanged_IsEnterSelecting));

        public bool IsEnterSelecting
        {
            get { return (bool)GetValue(IsEnterSelectingProperty); }
            set { SetValue(IsEnterSelectingProperty, value); }
        }

        private static void OnPropertyChanged_IsEnterSelecting(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardView) ((CardView)d).OnIsEnterSelectingChanged(e);
        }

        protected virtual void OnIsEnterSelectingChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region CanSelect

        static public readonly DependencyProperty CanSelectProperty = DependencyProperty.Register(
            "CanSelect", typeof(bool), typeof(CardView),
            new PropertyMetadata(false, OnPropertyChanged_CanSelect));

        public bool CanSelect
        {
            get { return (bool)GetValue(CanSelectProperty); }
            set { SetValue(CanSelectProperty, value); }
        }

        private static void OnPropertyChanged_CanSelect(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardView) ((CardView)d).OnCanSelectChanged(e);
        }

        protected virtual void OnCanSelectChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region IsSelected

        static public readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(CardView),
            new PropertyMetadata(false, OnPropertyChanged_IsSelected));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        private static void OnPropertyChanged_IsSelected(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardView) ((CardView)d).OnIsSelectedChanged(e);
        }

        protected virtual void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region SelectingMoveOffset

        static public readonly DependencyProperty SelectingMoveOffsetProperty = DependencyProperty.Register(
            "SelectingMoveOffset", typeof(double), typeof(CardView),
            new PropertyMetadata(0.0d, OnPropertyChanged_SelectingMoveOffset));

        public double SelectingMoveOffset
        {
            get { return (double)GetValue(SelectingMoveOffsetProperty); }
            set { SetValue(SelectingMoveOffsetProperty, value); }
        }

        private static void OnPropertyChanged_SelectingMoveOffset(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardView) ((CardView)d).OnSelectingMoveOffsetChanged(e);
        }

        protected virtual void OnSelectingMoveOffsetChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion 

        #endregion

        #endregion

        #region Methods

        internal void Animation()
        {
            foreach (CardAnim anim in Anims.ToArray())
                HandleAnim(anim);
        }

        protected void HandleAnim(CardAnim anim)
        {
            if (anim is CardMove)
            {
                CardMove move = (CardMove)anim;
                Position = move.GetPoint();
            }
            if (anim is CardShowWait)
            {
                CardShowWait showwait = (CardShowWait)anim;
                Opacity = showwait.GetOpacity();
            }
            if (anim is CardWaitHide)
            {
                CardWaitHide waithide = (CardWaitHide)anim;
                Opacity = waithide.GetOpacity();
            }
            if (++anim.Time > anim.TimeMax)
            {
                if (anim is CardWaitHide)
                    Hided?.Invoke(this, new EventArgs());
                Anims.Remove(anim);
            }
        }

        #endregion

        #region Event Handler

        public event System.EventHandler Hided;
        private bool ismouseoverafterselected;

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsSelectedProperty
             && IsSelected)
                ismouseoverafterselected = false;
            if (e.Property == IsMouseOverProperty
             && IsSelected && IsMouseOver)
                ismouseoverafterselected = true;
            if (e.Property == IsEnterSelectingProperty
             || e.Property == CanSelectProperty
             || e.Property == IsMouseOverProperty
             || e.Property == IsSelectedProperty)
            {
                if (!IsEnterSelecting || CanSelect || (IsSelected && !ismouseoverafterselected))
                {
                    DoubleAnimation anim = new DoubleAnimation();
                    anim.To = 1.0d;
                    anim.Duration = new Duration(TimeSpan.FromSeconds(0.1));
                    BeginAnimation(OpacityProperty, anim);
                }
                else
                {
                    DoubleAnimation anim = new DoubleAnimation();
                    anim.To = 0.7d;
                    anim.Duration = new Duration(TimeSpan.FromSeconds(0.1));
                    BeginAnimation(OpacityProperty, anim);
                }
            }
            if (e.Property == IsEnterSelectingProperty
             || e.Property == CanSelectProperty
             || e.Property == IsMouseOverProperty
             || e.Property == IsSelectedProperty)
            {
                if (IsEnterSelecting && CanSelect && IsMouseOver && !IsSelected)
                {
                    DoubleAnimation anim = new DoubleAnimation();
                    anim.To = 1.0d;
                    anim.Duration = new Duration(TimeSpan.FromSeconds(0.1));
                    GD_MouseOver.BeginAnimation(OpacityProperty, anim);

                }
                else
                {
                    DoubleAnimation anim = new DoubleAnimation();
                    anim.To = 0.0d;
                    anim.Duration = new Duration(TimeSpan.FromSeconds(0.1));
                    GD_MouseOver.BeginAnimation(OpacityProperty, anim);
                }
            }
            if (e.Property == IsEnterSelectingProperty
             || e.Property == IsSelectedProperty)
            {
                if (IsEnterSelecting && IsSelected)
                {
                    if (GD_Selected.Opacity != 1.0d)
                    {
                        DoubleAnimation anim = new DoubleAnimation();
                        anim.To = 1.0d;
                        anim.Duration = new Duration(TimeSpan.FromSeconds(0.1));
                        GD_Selected.BeginAnimation(OpacityProperty, anim);
                    }
                    if (SelectingMoveOffset != 20.0d)
                    {
                        DoubleAnimation anim = new DoubleAnimation();
                        anim.To = 20.0d;
                        anim.Duration = new Duration(TimeSpan.FromSeconds(0.1));
                        BeginAnimation(SelectingMoveOffsetProperty, anim);
                    }
                }
                else
                {
                    if (GD_Selected.Opacity != 0.0d)
                    {
                        DoubleAnimation anim = new DoubleAnimation();
                        anim.To = 0.0d;
                        anim.Duration = new Duration(TimeSpan.FromSeconds(0.1));
                        GD_Selected.BeginAnimation(OpacityProperty, anim);
                    }
                    if (SelectingMoveOffset != 0.0d)
                    {
                        DoubleAnimation anim = new DoubleAnimation();
                        anim.To = 0.0d;
                        anim.Duration = new Duration(TimeSpan.FromSeconds(0.1));
                        BeginAnimation(SelectingMoveOffsetProperty, anim);
                    }
                }
            }
            if (e.Property == PositionProperty
             || e.Property == SelectingMoveOffsetProperty)
            {
                Canvas.SetLeft(this, Position.X);
                Canvas.SetTop(this, Position.Y - SelectingMoveOffset);
            }
        }

        #endregion
    }

    public class CardAnim
    {
        private int time = 0;
        public int Time
        {
            get { return this.time; }
            set { this.time = value; }
        }

        private int timemax = 40;
        public int TimeMax
        {
            get { return this.timemax; }
            set { this.timemax = value; }
        }

        private CardRate rate = new CardLinearRate();
        public CardRate Rate
        {
            get { return this.rate; }
            set { this.rate = value; }
        }
    }

    public class CardMove : CardAnim
    {
        private Point from;
        public Point From
        {
            get { return this.from; }
            set { this.from = value; }
        }

        private Point to;
        public Point To
        {
            get { return this.to; }
            set { this.to = value; }
        }

        public Point GetPoint()
        {
            return from + (to - from) * (Rate?.GetRate(Time, TimeMax) ?? 1.0d);
        }
    }

    public class CardShowWait : CardAnim
    {
        private int waittime = 35;
        public int WaitTime
        {
            get { return this.waittime; }
            set { this.waittime = value; }
        }
       
        public double GetOpacity()
        {
            if (Time >= WaitTime) return 1.0d;
            return (Rate?.GetRate(Time, WaitTime) ?? 1.0d);
        }
    }

    public class CardWaitHide : CardAnim
    {
        private int waittime = 35;
        public int WaitTime
        {
            get { return this.waittime; }
            set { this.waittime = value; }
        }

        public double GetOpacity()
        {
            if (Time < WaitTime) return 1.0d;
            return 1.0d - (Rate?.GetRate(Time - WaitTime, TimeMax - WaitTime) ?? 1.0d);
        }
    }
    
    public class CardShowWaitHide : CardAnim
    {
        private int showtime = 5;
        public int ShowTime
        {
            get { return this.showtime; }
            set { this.showtime = value; }
        }

        private int waittime = 30;
        public int WaitTime
        {
            get { return this.waittime; }
            set { this.waittime = value; }
        }
        
        public double GetOpacity()
        {
            if (Time < ShowTime) return Rate?.GetRate(Time, ShowTime) ?? 1.0d;
            else if (Time < ShowTime + WaitTime) return 1.0d;
            else return 1.0d - (Rate?.GetRate(Time - ShowTime - WaitTime, TimeMax - ShowTime - WaitTime) ?? 1.0d);
        }

    }
    
    public abstract class CardRate
    {
        abstract public double GetRate(int time, int timemax);
    }

    public class CardLinearRate : CardRate
    {
        public override double GetRate(int time, int timemax)
        {
            return 1.0d * time / timemax;
        }
    }

    public class CardSquireFasterRate : CardRate
    {
        public override double GetRate(int time, int timemax)
        {
            return 1.0d * time * time / timemax / timemax;
        }
    }

    public class CardSquireRate : CardRate
    {
        public override double GetRate(int time, int timemax)
        {
            return 1.0d - Math.Pow(timemax - time, 2) / Math.Pow(timemax, 2);
        }

    }

    

    
}
