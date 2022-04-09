using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TouhouSha.Controls.Cools
{
    public interface ICoolAnim
    {
        void AnimationStart();
        bool IsVisible { get; }

        event DependencyPropertyChangedEventHandler IsVisibleChanged;
    }
}
