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

namespace TouhouSha.Controls
{
    /// <summary>
    /// DrawHeapPlacer.xaml 的交互逻辑
    /// </summary>
    public partial class DrawHeapPlacer : UserControl, IGameBoardArea
    {
        public DrawHeapPlacer()
        {
            InitializeComponent();
        }

        #region IGameBoardArea

        bool IGameBoardArea.KeptCards => false;

        IList<Card> IGameBoardArea.Cards => new List<Card>();

        Point? IGameBoardArea.GetExpectedPosition(Canvas cardcanvas, Card card)
        {
            Point p0 = TranslatePoint(new Point(0, 0), cardcanvas);
            double aw = ActualWidth;
            double ah = ActualHeight;
            double uw = CardView.DefaultWidth;
            double uh = CardView.DefaultHeight;
            double x0 = (aw - uw) / 2;
            double y0 = (ah - uh) / 2;
            return new Point(
                p0.X + x0,
                p0.Y + y0);
        }

        #endregion
    }
}
