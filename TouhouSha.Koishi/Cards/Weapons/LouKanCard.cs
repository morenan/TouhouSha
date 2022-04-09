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
    /// 武器【楼观剑】
    /// </summary>
    /// <remarks>
    /// 武器范围：3
    /// 你打出的【杀】被目标的【闪】响应后，结算完毕你可以再对其打出一张【杀】。
    /// </remarks>
    public class LouKanCard : SelfWeapon
    {
        public const string Normal = "楼观剑";
        
        public LouKanCard()
        {
            KeyName = Normal;
            Skills.Add(new SkillLouKan(this));
        }

        public override Card Create()
        {
            return new LouKanCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "攻击距离3。你打出的【杀】被目标的【闪】响应后，结算完毕你可以再对其打出一张【杀】。",
                Image = ImageHelper.LoadCardImage("Cards", "LouKan")
            };
        }
        public override double GetWorthForTarget()
        {
            return 3;
        }
    }
    
    public class SkillLouKan : Skill
    {
        public SkillLouKan(Card _weapon)
        {
            this.weapon = _weapon;
            IsLocked = true;
            Calculators.Add(new WeaponKillRangePlusCalculator(weapon, 2));
            Triggers.Add(new LouKanTrigger(weapon, this));
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        public override Skill Clone()
        {
            return new SkillLouKan(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new SkillLouKan(newcard);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }
    }

    public class LouKanTrigger : CardTrigger, ITriggerInEvent, ITriggerAsk
    {
        public LouKanTrigger(Card _weapon, Skill _weaponskill) : base(_weapon)
        {
            this.weapon = _weapon;
            this.weaponskill = _weaponskill;
            Condition = new LouKanCondition(card);
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        private Skill weaponskill;
        public Skill WeaponSkill { get { return this.weaponskill; } }
        
        string ITriggerInEvent.EventKeyName { get => StateChangeEvent.DefaultKeyName; }

        string ITriggerAsk.Message { get => "是否要使用【楼观剑】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return  card.Owner; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is StateChangeEvent)) return;
            StateChangeEvent ev = (StateChangeEvent)(ctx.Ev);
            if (ev.OldState?.KeyName?.Equals(State.Handle) != true) return;
            if (!(ev.OldState.Ev is CardEvent)) return;
            CardEvent ev1 = (CardEvent)(ev.OldState.Ev);
            CardSkillEvent ev2 = new CardSkillEvent();
            ev2.Reason = ev;
            ev2.Card = weapon;
            ev2.Skill = weaponskill;
            ev2.Source = ev1.Source;
            ev2.Targets.Clear();
            ev2.Targets.Add(ev.OldState.Owner);
            ctx.World.InvokeEvent(ev2);
            if (ev2.Cancel) return;
            ctx.World.RequireCard(LouKanCard.Normal, "请继续打出一张【杀】。", ev1.Source,
                new TargetCardFilter(1, 1, KillCard.Normal, KillCard.Thunder, KillCard.Fire),
                true, 10,
                (cards) => { ctx.World.UseCard(ev1.Source, ev.OldState.Owner, cards.FirstOrDefault(), null); },
                () => { });
        }
    }

    public class LouKanCondition : ConditionFilterFromCard
    {
        public LouKanCondition(Card _weapon) : base(_weapon)
        {
        }
        
        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is StateChangeEvent)) return false;
            StateChangeEvent ev = (StateChangeEvent)(ctx.Ev);
            if (ev.OldState?.KeyName?.Equals(State.Handle) != true) return false;
            if (!(ev.OldState.Ev is CardEvent)) return false;
            CardEvent ev1 = (CardEvent)(ev.OldState.Ev);
            if (!KillCard.IsKill(ev1.Card)) return false;
            if (card.Zone?.KeyName?.Equals(Zone.Equips) != true) return false;
            if (card.Zone.Owner != ev1.Source) return false;
            if (!card.Zone.Cards.Contains(card)) return false;
            return true;
        }
    }
}
