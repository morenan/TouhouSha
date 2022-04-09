using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Cards;

namespace TouhouSha.Reimu.Charactors.Forevers
{
    /// <summary>
    /// 角色【藤原妹红】
    /// </summary>
    /// <remarks>
    /// 竹林 HP:4
    /// 【蓬莱】锁定技，你的回合开始阶段，若你已受伤，你回复2点体力，若你未受伤，你增加一点体力上限并回复一点体力。
    /// 【火鸟】你的可以将你的一张红色牌当作无视距离的【火杀】使用或打出。当你用【火杀】指定目标时可以发动：
    ///      你令目标回复其一点体力，丢弃其等同于其体力上限的牌。
    ///      结算完毕后，你对目标造成2点火焰伤害。令其摸等同于其体力上限的牌。
    /// </remarks>
    public class Mokou : Charactor
    {
        public Mokou()
        {
            MaxHP = 4;
            HP = 4;
            Country = Kaguya.CountryNameOfLeader;
            Skills.Add(new Skill_PengLai());
            Skills.Add(new Mokou_Skill_1());
        }

        public override Charactor Clone()
        {
            return new Mokou();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "藤原妹红";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Mokou");
            info.AbilityRadar = new AbilityRadar() { Attack = 4, Defence = 3, Control = 3, Auxiliary = 2, LastStages = 3, Difficulty = 5 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }

    }

    public class Mokou_Skill_1 : Skill, ISkillCardConverter, ISkillCardMultiConverter2
    {
        public const string Marked = "火鸟标记";

        public Mokou_Skill_1()
        {
            this.usecondition = new Mokou_Skill_1_UseCondition(this);
            this.cardfilter = new Mokou_Skill_1_CardFilter(this);
            this.cardconverter = new Mokou_Skill_1_CardConverter(this);
            Triggers.Add(new Mokou_Skill_1_Trigger_0(this));
            Triggers.Add(new Mokou_Skill_1_Trigger_1(this));
        }

        private Mokou_Skill_1_UseCondition usecondition;
        ConditionFilter ISkillCardConverter.UseCondition { get => usecondition; }

        private Mokou_Skill_1_CardConverter cardconverter;
        CardCalculator ISkillCardConverter.CardConverter { get => cardconverter; }

        private Mokou_Skill_1_CardFilter cardfilter;
        CardFilter ISkillCardConverter.CardFilter { get => cardfilter; }

        private List<string> enabledcardtypes;
        IEnumerable<string> ISkillCardMultiConverter2.EnabledCardTypes { get => enabledcardtypes; }

        void ISkillCardMultiConverter2.SetEnabledCardTypes(Context ctx, IEnumerable<string> cardtypes)
        {
            if (cardtypes.Contains(KillCard.Fire))
                this.enabledcardtypes = new List<string>() { KillCard.Fire };
        }

        void ISkillCardMultiConverter2.CancelEnabledCardTypes(Context ctx)
        {
            this.enabledcardtypes = null;
        }

        public override Skill Clone()
        {
            return new Mokou_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Mokou_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            SkillInfo skillinfo = new SkillInfo()
            {
                Name = "快晴",
                Description = "出牌阶段限X次（X为你已经损失的体力值且至少为1），你可以将一张红色牌当作造成火焰伤害的【决斗】来使用，" +
                    "这张【决斗】赢的一方若手牌数小于体力上限，摸牌直到和体力相等上限。"
            };
            return skillinfo;
        }
    }
  
    public class Mokou_Skill_1_UseCondition : ConditionFilterFromSkill
    {
        public Mokou_Skill_1_UseCondition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }

    public class Mokou_Skill_1_CardFilter : CardFilterFromSkill
    {
        public Mokou_Skill_1_CardFilter(Skill _skill) : base(_skill)
        {

        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() >= 1) return false;
            if (want.CardColor?.SeemAs(Enum_CardColor.Red) != true) return false;
            switch (want.Zone?.KeyName)
            {
                case Zone.Hand:
                case Zone.Equips:
                    break;
                default:
                    return false;
            }
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() >= 1;
        }
    }

    public class Mokou_Skill_1_CardConverter : CardCalculatorFromSkill
    {
        public Mokou_Skill_1_CardConverter(Skill _skill) : base(_skill)
        {

        }

        public override Card GetValue(Context ctx, Card oldvalue)
        {
            Card firekill = ctx.World.GetCardInstance(KillCard.Fire);
            if (firekill == null) return oldvalue;
            firekill = firekill.Clone(oldvalue);
            return firekill;
        }
    }

    public class Mokou_Skill_1_Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Mokou_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Mokou_Skill_1_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_BeforeStart + 1; }

        string ITriggerAsk.Message { get => "是否要发动【火鸟】？"; }
        
        Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (state.Owner == null) return;
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = state.Ev;
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(state.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            state.Ev.SetValue(Mokou_Skill_1.Marked, 1);
            state.Ev.SetValue(String.Format("{0}_{1}", Mokou_Skill_1.Marked, state.Owner.KeyName), 1);
            ctx.World.Heal(skill.Owner, state.Owner, 1, HealEvent.Normal, ev_skill);
            ctx.World.DiscardCard(state.Owner, state.Owner.MaxHP, ev_skill, true);
        }
    }

    public class Mokou_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Mokou_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner == null) return false;
            if (!(state.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (ev.Source != skill.Owner) return false;
            if (ev.Card?.KeyName?.Equals(KillCard.Fire) != true) return false;
            return true;
        }
    }
    
    public class Mokou_Skill_1_Trigger_1 : SkillTrigger, ITriggerInEvent
    {
        public Mokou_Skill_1_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Mokou_Skill_1_Trigger_1_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => CardDoneEvent.DefaultKeyName; }
        
        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is CardDoneEvent)) return;
            CardDoneEvent ev = (CardDoneEvent)(ctx.Ev);
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.AddRange(ev.Targets);
            foreach (Player target in ev.Targets)
            {
                if (ev.GetValue(String.Format("{0}_{1}", Mokou_Skill_1.Marked, target.KeyName)) != 1) continue;
                ctx.World.Damage(skill.Owner, target, 2, DamageEvent.Fire, ev_skill);
                ctx.World.DrawCard(target, target.MaxHP, ev_skill);
            }
        }
    }

    public class Mokou_Skill_1_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Mokou_Skill_1_Trigger_1_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is CardDoneEvent)) return false;
            CardDoneEvent ev = (CardDoneEvent)(ctx.Ev);
            if (ev.GetValue(Mokou_Skill_1.Marked) != 1) return false;
            if (ev.Source != skill.Owner) return false;
            return true;
        }
    }
    
    
}
