using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    /// <summary>
    /// 卡片使用或者打出的事件
    /// </summary>
    /// <remarks>
    /// 如何区分使用和打出？
    /// 如果Reason也是CardEventBase，那么就是一张牌响应另一张牌打出了，否则均视为使用。
    /// </remarks>
    public class CardEventBase : Event, IMultiTargetsEvent
    {

        private Player source;
        public Player Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        private Card card;
        public Card Card
        {
            get { return this.card; }
            set { this.card = value; }
        }

        private Skill skill;
        public Skill Skill
        {
            get { return this.skill; }
            set { this.skill = value; }
        }

        private List<Player> targets = new List<Player>();
        public List<Player> Targets
        {
            get { return this.targets; }
        }

        public override Player GetHandleStarter()
        {
            return Source;
        }

        public override bool IsStopHandle()
        {
            if (source == null || !source.IsAlive) return true;
            if (targets.FirstOrDefault(_target => _target.IsAlive) == null) return true;
            if (card == null) return true;
            return false;
        }

    }
    
    public class CardEvent : CardEventBase
    {
        public const string DefaultKeyName = "卡片使用发起";
        public const string DamageValue = "伤害基础数值";
        public const string HealValue = "回复基础数值";
        
        public CardEvent()
        {
            KeyName = DefaultKeyName;
        }

    }
    
    public class CardPreviewEvent : CardEventBase
    {
        public const string DefaultKeyName = "卡片使用确认";

        public CardPreviewEvent()
        {
            KeyName = DefaultKeyName;
        }

    }

    public class CardDoneEvent : CardEventBase
    {
        public const string DefaultKeyName = "卡片使用完毕";

        public CardDoneEvent()
        {
            KeyName = DefaultKeyName;
        }

    }
}
