using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Calculators;

namespace TouhouSha.Koishi.Cards.Weapons
{
    /// <summary>
    /// 武器【八卦炉】
    /// </summary>
    /// <remarks>
    /// 武器范围：3
    /// 你打出一张杀时，可以进行一次判定。判定结果为红色，你摸两张牌，为黑色，你弃两张手牌。
    /// </remarks>
    public class HakkeroCard : SelfWeapon
    {
        public const string Normal = "八卦炉";

        public HakkeroCard()
        {
            KeyName = Normal;
            Skills.Add(new SkillHakkero(this));
        }
        public override Card Create()
        {
            return new HakkeroCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "攻击距离3。你打出一张杀时，可以进行一次判定。判定结果为红色，你摸两张牌，为黑色，你弃两张手牌。",
                Image = ImageHelper.LoadCardImage("Cards", "Hakkero")
            };
        }
        public override double GetWorthForTarget()
        {
            return 3;
        }
    }
    
    public class SkillHakkero : Skill
    {
        public SkillHakkero(Card _weapon)
        {
            this.weapon = _weapon;
            IsLocked = false;
            Calculators.Add(new WeaponKillRangePlusCalculator(weapon, 2));
            Triggers.Add(new SkillHakkeroEnterJudge(weapon, this));
            Triggers.Add(new SkillHakkeroApplyJudge(weapon, this));
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        public override Skill Clone()
        {
            return new SkillHakkero(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new SkillHakkero(newcard);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }
    }

    public class SkillHakkeroEnterJudge : CardTrigger, ITriggerInState, ITriggerAsk
    {
        public const string DefaultKeyName = "八卦炉进判定";

        public class TriggerCondition : ConditionFilterFromCard
        {
            public const string DefaultKeyName = "八卦炉进判定条件";
           
            public TriggerCondition(Card _weapon, SkillHakkero _weaponskill) : base(_weapon)
            {
                this.weapon = _weapon;
                this.weaponskill = _weaponskill;
                KeyName = DefaultKeyName;
            }

            private Card weapon;
            public Card Weapon { get { return this.weapon; } }

            private SkillHakkero weaponskill;
            public SkillHakkero WeaponSkill { get { return this.weaponskill; } }
            
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

        public SkillHakkeroEnterJudge(Card _weapon, SkillHakkero _weaponskill) : base(_weapon)
        {
            this.weapon = _weapon;
            this.weaponskill = _weaponskill;
            Condition = new TriggerCondition(weapon, weaponskill);
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        private SkillHakkero weaponskill;
        public SkillHakkero WeaponSkill { get { return this.weaponskill; } }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_BeforeStart; }

        string ITriggerAsk.Message { get => "是否要发动【八卦炉】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return  weapon.Owner; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (!(state.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(state.Ev);
            CardSkillEvent ev_skill = new CardSkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Card = weapon;
            ev_skill.Skill = weaponskill;
            ev_skill.Source = ev.Source;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(ev.Source);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            JudgeEvent ev_judge = new JudgeEvent();
            ev_judge.Reason = ev_skill;
            ev_judge.JudgeNumber = 1;
            ev_judge.JudgeTarget = ev.Source;
            ev_judge.JudgeCards.Clear();
            ctx.World.InvokeEvent(ev_judge);
        }
    }

    public class SkillHakkeroApplyJudge : CardTrigger, ITriggerInState
    {
        public const string DefaultKeyName = "八卦炉判定生效";

        public class TriggerCondition : ConditionFilter
        {
            public const string DefaultKeyName = "八卦炉判定生效条件";

            public TriggerCondition()
            {
                KeyName = DefaultKeyName;
            }
            
            public override bool Accept(Context ctx)
            {
                State state = ctx.World.GetCurrentState();
                if (state == null) return false;
                if (!(state.Ev is JudgeEvent)) return false;
                JudgeEvent ev0 = (JudgeEvent)(state.Ev);
                if (!(ev0.Reason is CardSkillEvent)) return false;
                if (ev0.JudgeCards.Count() != 1) return false;
                CardSkillEvent ev1 = (CardSkillEvent)(ev0.Reason);
                if (ev1.Card?.KeyName?.Equals(HakkeroCard.Normal) != true) return false;
                return true;
            }
        }

        public SkillHakkeroApplyJudge(Card _weapon, Skill _weaponskill) : base(_weapon)
        {
            Condition = new TriggerCondition();
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        private Skill weaponskill;
        public Skill WeaponSkill { get { return this.weaponskill; } }
        
        string ITriggerInState.StateKeyName { get => State.Judge; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (!(state.Ev is JudgeEvent)) return;
            JudgeEvent ev0 = (JudgeEvent)(state.Ev);
            if (!(ev0.Reason is CardSkillEvent)) return;
            if (ev0.JudgeCards.Count() != 1) return;
            CardSkillEvent ev1 = (CardSkillEvent)(ev0.Reason);
            if (ev1.Card?.KeyName?.Equals(HakkeroCard.Normal) != true) return;
            Card judgecard = ev0.JudgeCards[0];
            if (judgecard.CardColor.SeemAs(Enum_CardColor.Red))
            {
                ctx.World.DrawCard(state.Owner, 2, ev1);
                return;
            }
            if (judgecard.CardColor.SeemAs(Enum_CardColor.Black))
            {
                ctx.World.DiscardCard(state.Owner, 2, ev1, false);
                return;
            }
        }
    }







}
