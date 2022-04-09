using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core
{
    public class Context
    {
        public Context(World _world, Event _ev)
        {
            this.world = _world;
            this.ev = _ev;
        }

        private World world;
        public World World
        {
            get { return this.world; }
        }
        
        private Event ev;
        public Event Ev
        {
            get { return this.ev; }
            set { this.ev = value; }
        }

        public Context Clone()
        {
            Context that = new Context(world, ev);
            return that;
        }
    }
}
