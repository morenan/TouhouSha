using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TouhouSha.Core;

namespace TouhouSha.Controls
{
    public interface IGameBoardArea
    {
        bool KeptCards { get; }
        IList<Card> Cards { get; }
        Point? GetExpectedPosition(Canvas cardcanvas, Card card);
    }
}
