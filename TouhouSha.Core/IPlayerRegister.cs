using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core
{
    public interface IPlayerRegister
    {
        IPlayerConsole CreateConsole(Player player);
    }
}
