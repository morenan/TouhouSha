using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// HandBoard.xaml 的交互逻辑
    /// </summary>
    public partial class HandBoard : UserControl, IGameBoardArea
    {
        public HandBoard()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }
        
        #region Number

        private GameBoard gameboard;

        #endregion

        #region Method

        #endregion

        #region IGameBoardArea

        bool IGameBoardArea.KeptCards => true;

        private List<Card> areacards = new List<Card>();
        IList<Card> IGameBoardArea.Cards => areacards;

        Point? IGameBoardArea.GetExpectedPosition(Canvas cardcanvas, Card card)
        {
            Point p0 = TranslatePoint(new Point(0, 0), cardcanvas);
            int index = areacards.IndexOf(card);
            if (index < 0) return null;
            double aw = ActualWidth;
            double ah = ActualHeight;
            double uw = CardView.DefaultWidth;
            double uh = CardView.DefaultHeight;
            double x0 = (aw - uw * areacards.Count()) / 2;
            double y0 = (ah - uh) / 2;
            if (x0 < 0)
            {
                x0 = 0;
                uw = (aw - uw) / areacards.Count();
            }
            return new Point(
                p0.X + x0 + index * uw,
                p0.Y + y0);
        }

        #endregion

        #region Event Handler

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
