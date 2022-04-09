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
    /// 防具【帽子先生】
    /// </summary>
    /// <remarks>
    /// 当一张牌指定多个目标，且你为其中之一时，取消对你的结算。
    /// </remarks>
    public class KoishiHatCard : SelfArmor
    {
        public const string Normal = "帽子先生";

        public KoishiHatCard()
        {
            KeyName = Normal;
            Skills.Add(new KoishiHat_Skill_0(this));
        }

        public override Card Create()
        {
            return new KoishiHatCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "当一张牌指定多个目标，且你为其中之一时，取消对你的结算。",
                Image = ImageHelper.LoadCardImage("Cards", "KoishiHat")
            };
        }
        public override double GetWorthForTarget()
        {
            return 3;
        }
    }
    
    public class KoishiHat_Skill_0 : Skill
    {
        public KoishiHat_Skill_0(Card _armor)
        {
            this.armor = _armor;
            IsLocked = true;
            Triggers.Add(new KoishiHat_Skill_0_Trigger(armor));
        }

        private Card armor;
        public Card Armor { get { return this.armor; } }

        public override Skill Clone()
        {
            return new KoishiHat_Skill_0(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new KoishiHat_Skill_0(newcard);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }

    }

    public class KoishiHat_Skill_0_Trigger : CardTrigger
    {
        public KoishiHat_Skill_0_Trigger(Card _card) : base(_card)
        {
            Condition = new KoishiHat_Skill_0_Trigger_Condition(card);
        }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(ctx.Ev);
            ev.Targets.Remove(card.Owner);
        }
    }

    public class KoishiHat_Skill_0_Trigger_Condition : ConditionFilterFromCard
    {
        public KoishiHat_Skill_0_Trigger_Condition(Card _card) : base(_card)
        {

        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(ctx.Ev);
            if (ev.Targets.Count() < 2) return false;
            if (!ev.Targets.Contains(card.Owner)) return false;
            return true;
        }

    }

    
}
