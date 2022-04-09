using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public interface IMultiTargetsEvent
    {
        List<Player> Targets { get; }
    }
}
