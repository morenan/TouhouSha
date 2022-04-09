using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;

namespace TouhouSha.AI
{
    public class AIRegister : IPlayerRegister
    {
        public IPlayerConsole CreateConsole(Player player)
        {
            return new AIConsole(player);
        }
    }
}
