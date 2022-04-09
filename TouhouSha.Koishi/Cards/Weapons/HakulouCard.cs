using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;
using TouhouSha.Core.Events;
using TouhouSha.Koishi.Calculators;


namespace TouhouSha.Koishi.Cards.Weapons
{
    /// <summary>
    /// 武器【白楼剑】
    /// </summary>
    /// <remarks>
    /// 武器范围：2
    /// 你的【杀】造成伤害后，你可以弃置一张牌并回复一点体力。
    /// </remarks>
    public class HakulouCard : SelfWeapon
    {
        public const string Normal = "白楼剑";

        public HakulouCard()
        {
            KeyName = Normal;
            Skills.Add(new SkillHakulou(this));
        }
        public override Card Create()
        {
            return new HakulouCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "攻击距离2。你的【杀】造成伤害后，你可以弃置一张牌并回复一点体力。",
                Image = ImageHelper.LoadCardImage("Cards", "Hakulou")
            };
        }
        public override double GetWorthForTarget()
        {
            return 3;
        }
    }
    
    public class SkillHakulou : Skill
    {
        public SkillHakulou(Card _weapon)
        {
            this.weapon = _weapon;
            IsLocked = true;
            Calculators.Add(new WeaponKillRangePlusCalculator(weapon, 1));
            Triggers.Add(new HakulouTrigger(weapon, this));
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        public override Skill Clone()
        {
            return new SkillHakulou(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new SkillHakulou(newcard);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }
    }

    public class HakulouTrigger : CardTrigger, ITriggerInState, ITriggerAsk
    {
        public HakulouTrigger(Card _weapon, Skill _weaponskill) : base(_weapon)
        {
            this.weapon = _weapon;
            this.weaponskill = _weaponskill;
            Condition = new HakulouTriggerCondition(_weapon, _weaponskill);
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        private Skill weaponskill;
        public Skill WeaponSkill { get { return this.weaponskill; } }

        string ITriggerInState.StateKeyName { get => State.Damaged; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_AfterEnd; }

        string ITriggerAsk.Message { get => "是否要发动【白楼剑】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return  weapon.Owner; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (!(state.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(state.Ev);
            Player player = weapon.Zone?.Owner;
            if (player == null) return;
            CardSkillEvent ev_skill = new CardSkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Card = weapon;
            ev_skill.Skill = weaponskill;
            ev_skill.Source = player;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(player);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            ctx.World.DiscardCard(player, 1, ev_skill, true);
            ctx.World.Heal(player, player, 1, HealEvent.Normal, ev_skill);
        }
    }

    public class HakulouTriggerCondition : ConditionFilterFromCard
    {
        public HakulouTriggerCondition(Card _weapon, Skill _weaponskill) : base(_weapon)
        {
            this.weapon = _weapon;
            this.weaponskill = _weaponskill;
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        private Skill weaponskill;
        public Skill WeaponSkill { get { return this.weaponskill; } }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.KeyName?.Equals(State.Damaged) != true) return false;
            if (!(state.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (!KillCard.IsKill(ev.Card)) return false;
            if (weapon.Zone?.KeyName?.Equals(Zone.Equips) != true) return false;
            if (weapon.Zone.Owner != ev.Source) return false;
            if (!weapon.Zone.Cards.Contains(weapon)) return false;
            Player player = weapon.Zone.Owner;
            if (player == null) return false;
            CardSkillEvent ev_skill = new CardSkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Card = weapon;
            ev_skill.Skill = weaponskill;
            ev_skill.Source = player;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(player);
            if (!ctx.World.CanDiscardCardFulfillNumber(player, 1, ev_skill, true)) return false;
            return true;
            
        }


    }


}
