using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Calculators;

namespace TouhouSha.Koishi.Cards.Weapons
{
    /// <summary>
    /// 武器【莱瓦丁】
    /// </summary>
    /// <remarks>
    /// 武器范围：4
    /// 你的【杀】均视作【火杀】。
    /// </remarks>
    public class LaevatainCard : SelfWeapon
    {
        public const string Normal = "莱瓦丁";

        public LaevatainCard()
        {
            KeyName = Normal;
            Skills.Add(new SkillLaevatain(this));
        }
        public override Card Create()
        {
            return new LaevatainCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "攻击距离4。你的【杀】均视作【火杀】。",
                Image = ImageHelper.LoadCardImage("Cards", "Laevatain")
            };
        }
        public override double GetWorthForTarget()
        {
            return 3;
        }
    }
    

    public class SkillLaevatain : Skill
    {
        public SkillLaevatain(Card _weapon)
        {
            this.weapon = _weapon;
            Calculators.Add(new WeaponKillRangePlusCalculator(weapon, 3));
            Triggers.Add(new LaevatainTrigger(weapon, this));
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }


        public override Skill Clone()
        {
            return new SkillLaevatain(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new SkillLaevatain(newcard);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }

    }

    public class LaevatainTrigger : CardTrigger, ITriggerInState
    {
        public LaevatainTrigger(Card _weapon, Skill _weaponskill) : base(_weapon)
        {
            this.weapon = _weapon;
            this.weaponskill = _weaponskill;
            Condition = new LaevatainTriggerCondition(weapon);
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        private Skill weaponskill;
        public Skill WeaponSkill { get { return this.weaponskill; } }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_BeforeStart; }
        
        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(ctx.Ev);
            Card firekill = ev.Card.Clone();
            firekill.OriginCards.Clear();
            firekill.OriginCards.Add(ev.Card);
            firekill.KeyName = KillCard.Fire;
            ev.Card = firekill;
        }
    }

    public class LaevatainTriggerCondition : ConditionFilterFromCard
    {
        public LaevatainTriggerCondition(Card _weapon) : base(_weapon)
        {

        }
        

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state?.KeyName?.Equals(State.Handle) != true) return false;
            if (!(state.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (!KillCard.IsKill(ev.Card)) return false;
            if (card.Zone?.KeyName?.Equals(Zone.Equips) != true) return false;
            if (card.Zone.Owner != ev.Source) return false;
            if (!card.Zone.Cards.Contains(card)) return false;
            return true;
        }
    }

}
