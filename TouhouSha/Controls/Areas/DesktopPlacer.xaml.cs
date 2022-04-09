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
    /// DesktopPlacer.xaml 的交互逻辑
    /// </summary>
    public partial class DesktopPlacer : UserControl, IGameBoardArea
    {
        public DesktopPlacer()
        {
            InitializeComponent();
        }

        #region IGameBoardArea

        bool IGameBoardArea.KeptCards => true;

        private List<Card> areacards = new List<Card>();
        IList<Card> IGameBoardArea.Cards => areacards;

        public int LineCount => Config.ScreenConfig.DesktopLineCount;
        public int ColumnCount => (int)(ActualHeight / CardView.DefaultWidth);
        
        Point? IGameBoardArea.GetExpectedPosition(Canvas cardcanvas, Card card)
        {
            Point p0 = TranslatePoint(new Point(0, 0), cardcanvas);
            int index = areacards.IndexOf(card);
            if (index < 0) return null;
            int rows = Math.Min(LineCount, (areacards.Count() - 1) / ColumnCount + 1);
            int columns = Math.Min(ColumnCount, areacards.Count());
            int rowindex = index / columns;
            int colindex = index % columns;
            double aw = ActualWidth;
            double ah = ActualHeight;
            double uw = CardView.DefaultWidth;
            double uh = CardView.DefaultHeight;
            double x0 = (aw - uw * columns) / 2;
            //double y0 = (ah - uh * rows) / 2;
            double y0 = Math.Min(0, ah - uh * rows);
            if (y0 < 0)
            {
                y0 = 0;
                uh = (ah - uh) / rows;
            } 
            if (rowindex >= rows - 1)
            {
                columns = areacards.Count() - (rows - 1) * columns;
                colindex = index - (rows - 1) * columns;
                x0 = (aw - uw * columns) / 2;
                if (x0 < 0)
                {
                    x0 = 0;
                    uw = (aw - uw) / columns;
                }
            }
            return new Point(
                p0.X + x0 + colindex * uw,
                p0.Y + y0 + rowindex * uh);
        }
        
        #endregion
    }
}
