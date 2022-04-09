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
    /// 武器【御币】
    /// </summary>
    /// <remarks>
    /// 武器范围：2
    /// 你打出一张杀时，你可以丢弃攻击范围内一名角色的手牌。
    /// </remarks>
    public class GoheCard : SelfWeapon
    {
        public const string Normal = "御币";

        public GoheCard()
        {
            KeyName = Normal;
            Skills.Add(new SkillGohe(this));
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
                Description = "攻击距离2。你打出一张杀时，你可以丢弃攻击范围内一名角色的手牌。",
                Image = ImageHelper.LoadCardImage("Cards", "Gohe")
            };
        }
        public override double GetWorthForTarget()
        {
            return 3;
        }
    }
    

    public class SkillGohe : Skill
    {
        public SkillGohe(Card _weapon)
        {
            this.weapon = _weapon;
            IsLocked = false;
            Calculators.Add(new WeaponKillRangePlusCalculator(weapon, 1));
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        public override Skill Clone()
        {
            return new SkillGohe(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new SkillGohe(newcard);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }
    }

    public class SkillGoheTrigger : CardTrigger, ITriggerInState, ITriggerAsk
    {
        public const string DefaultKeyName = "御币选人丢卡";

        public class TriggerCondition : ConditionFilterFromCard
        {
            public const string DefaultKeyName = "御币选人丢卡条件";

            public TriggerCondition(Card _weapon, SkillGohe _weaponskill) : base(_weapon)
            {
                this.weapon = _weapon;
                this.weaponskill = _weaponskill;
                KeyName = DefaultKeyName;
            }

            private Card weapon;
            public Card Weapon { get { return this.weapon; } }

            private SkillGohe weaponskill;
            public SkillGohe WeaponSkill { get { return this.weaponskill; } }

            public override bool Accept(Context ctx)
            {
                State state = ctx.World.GetCurrentState();
                if (state == null) return false;
                if (!(state.Ev is CardEvent)) return false;
                CardEvent ev = (CardEvent)(state.Ev);
                if (!KillCard.IsKill(ev.Card)) return false;
                if (weapon.Zone?.KeyName?.Equals(Zone.Equips) != true) return false;
                if (weapon.Zone.Owner != ev.Source) return false;
                if (!weapon.Zone.Cards.Contains(weapon)) return false;
                return true;
            }
        }

        public SkillGoheTrigger(Card _weapon, SkillGohe _weaponskill) : base(_weapon)
        {
            this.weapon = _weapon;
            this.weaponskill = _weaponskill;
            Condition = new TriggerCondition(weapon, weaponskill);
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        private SkillGohe weaponskill;
        public SkillGohe WeaponSkill { get { return this.weaponskill; } }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_BeforeStart; }

        string ITriggerAsk.Message { get => "是否要发动【御币】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return  weapon.Owner; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            Player player = state.Owner;
            ctx.World.SelectPlayer(GoheCard.Normal, "请选择一个目标。",
                player,
                new GoheTargetFilter(weapon),
                true, 10,
                (targets) =>
                {
                    CardSkillEvent ev_skill = new CardSkillEvent();
                    ev_skill.Reason = state.Ev;
                    ev_skill.Source = player;
                    ev_skill.Targets.Clear();
                    ev_skill.Targets.AddRange(targets);
                    ev_skill.Card = weapon;
                    ctx.World.InvokeEvent(ev_skill);
                    if (ev_skill.Cancel) return;
                    ctx.World.DiscardTargetCard(player, targets.FirstOrDefault(), 1, ev_skill, true, true);
                },
                () => { });
        }
    }

    public class GoheTargetFilter : PlayerFilterFromCard
    {
        public const string DefaultKeyName = "御币发动的目标";

        public GoheTargetFilter(Card _weapon) : base(_weapon)
        {
            KeyName = DefaultKeyName;
        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            Player player = state.Owner;
            if (player == null) return false;
            if (selecteds.Count() > 0) return false;
            if (!ctx.World.IsInDistance2Kill(ctx, player, want)) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() > 0;
        }
    }


}
