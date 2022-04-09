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
using TouhouSha.Koishi.Triggers;

namespace TouhouSha.Koishi.Cards.Weapons
{
    /// <summary>
    /// 武器【冈格尼尔】
    /// </summary>
    /// <remarks>
    /// 武器范围：3
    /// 当你的【杀】被【闪】抵消前，你可以丢弃两张牌使这个【闪】无效。
    /// </remarks>
    public class GungnirCard : SelfWeapon
    {
        public const string Normal = "冈格尼尔";
        
        public GungnirCard()
        {
            KeyName = Normal;
            Skills.Add(new SkillGungnir(this));
        }
        public override Card Create()
        {
            return new GoheCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "攻击距离3。当你的【杀】被【闪】抵消前，你可以丢弃两张牌使这个【闪】无效。",
                Image = ImageHelper.LoadCardImage("Cards", "Gungnir")
            };
        }
        public override double GetWorthForTarget()
        {
            return 3;
        }
    }
    
    public class SkillGungnir : Skill
    {
        public SkillGungnir(Card _weapon)
        {
            this.weapon = _weapon;
            Calculators.Add(new WeaponKillRangePlusCalculator(weapon, 2));
            Triggers.Add(new GungnirTrigger(weapon, this));
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }
        
        public override Skill Clone()
        {
            return new SkillGungnir(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new SkillGungnir(newcard);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }

    }

    public class GungnirTrigger : CardTrigger, ITriggerInState, ITriggerAsk
    {
        public GungnirTrigger(Card _weapon, Skill _weaponskill) : base(_weapon)
        {
            this.weapon = _weapon;
            this.weaponskill = _weaponskill;
            Condition = new GungnirTriggerCondition(weapon);
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        private Skill weaponskill;
        public Skill WeaponSkill { get { return this.weaponskill; } }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start + 1; }

        string ITriggerAsk.Message { get => "是否要发动【冈格尼尔】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return  weapon.Owner; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(ctx.Ev);
            CardSkillEvent ev_skill = new CardSkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Source = ev.Source;
            ev_skill.Card = weapon;
            ev_skill.Skill = weaponskill;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(state.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            ctx.World.DiscardCardCanCancel(ev.Source, 2, ev_skill, true,
                (cards) => { ev.SetValue(MissKillTrigger.HasMissed, 0); },
                () => { });
        }
    }

    public class GungnirTriggerCondition : ConditionFilterFromCard
    {
        public GungnirTriggerCondition(Card _weapon) : base(_weapon)
        {

        }
        
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state?.KeyName?.Equals(State.Handle) != true) return false;
            if (!(state.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (!KillCard.IsKill(ev.Card)) return false;
            if (ev.GetValue(MissKillTrigger.HasMissed) == 0) return false;
            if (card.Zone?.KeyName?.Equals(Zone.Equips) != true) return false;
            if (card.Zone.Owner != ev.Source) return false;
            if (!card.Zone.Cards.Contains(card)) return false;
            return true;
        }
    }
    
}
