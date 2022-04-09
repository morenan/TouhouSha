using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs.Events
{
    public class UIEvent : EventArgs
    {
    }

    public delegate void UIEventHandler(object sender, UIEvent e);
}
