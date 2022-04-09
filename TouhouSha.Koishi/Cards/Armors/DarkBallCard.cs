using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;

namespace TouhouSha.Koishi.Cards.Armors
{
    /// <summary>
    /// 防具【黑球】
    /// </summary>
    /// <remarks>
    /// 黑色的【杀】对你无效。
    /// </remarks>
    public class DarkBallCard : SelfArmor
    {
        public const string DefaultKeyName = "黑球";

        public DarkBallCard()
        {
            KeyName = DefaultKeyName;
            CardType = new CardType(Enum_CardType.Equip, new CardSubType(Enum_CardSubType.Weapon));
        }

        public override Card Create()
        {
            return new DarkBallCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "黑色的【杀】对你无效。",
                Image = ImageHelper.LoadCardImage("Cards", "DarkBall")
            };
        }

        public override double GetWorthForTarget()
        {
            return 3;
        }
    }

    public class DarkBall_Skill : Skill
    {
        public DarkBall_Skill(Card _armor)
        {
            this.armor = _armor;
            Triggers.Add(new DarkBall_Trigger(armor));
        }

        private Card armor;
        public Card Armor { get { return this.armor; } }

        public override Skill Clone()
        {
            return new DarkBall_Skill(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new DarkBall_Skill(newcard);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }
    }

    public class DarkBall_Trigger : CardTrigger, ITriggerInEvent
    {
        public DarkBall_Trigger(Card _card) : base(_card)
        {
            Condition = new DarkBall_Condition(card);
        }
       
        string ITriggerInEvent.EventKeyName { get => CardEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(ctx.Ev);
            ev.Targets.Remove(card.Owner);
        }
    }

    public class DarkBall_Condition : ConditionFilterFromCard
    {
        public DarkBall_Condition(Card _card) : base(_card)
        {

        }

        public override bool Accept(Context ctx)
        {
            if (card.Owner == null) return false;
            if (card.Zone?.KeyName?.Equals(Zone.Equips) != true) return false;
            if (!(ctx.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(ctx.Ev);
            if (!KillCard.IsKill(ev.Card)) return false;
            if (!ev.Targets.Contains(card.Owner)) return false;
            if (ev.Card.CardColor?.SeemAs(Enum_CardColor.Black) != true) return false;
            return true;
        }
    }
    
}
