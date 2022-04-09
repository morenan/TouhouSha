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

namespace TouhouSha.Reimu.Charactors.Homos
{
    /// <summary>
    /// 角色【红美铃】
    /// </summary>
    /// <remarks>
    /// 势力：红魔 4勾玉
    /// 【龙拳】：当你打出【杀】或者【闪】时，你可以选择一名其他角色，获取其一张手牌。
    /// 【龙门】：觉醒技，你的回合开始阶段，当你体力为全场最少（之一）时，你失去一点体力上限，选择恢复一点体力或者摸两张牌，获得技能【太极】。
    /// 【太极】：你可以将【杀】当【闪】，【闪】当【杀】打出。你以此法打出的【杀】不计入使用杀的次数。
    /// </remarks>
    public class HongMeiLing : Charactor
    {
        public HongMeiLing()
        {
            MaxHP = 4;
            HP = 4;
            Country = Remilia.CountryNameOfLeader;
            Skills.Add(new HongMeiLing_Skill_0());
            Skills.Add(new HongMeiLing_Skill_1());
        }

        public override Charactor Clone()
        {
            return new HongMeiLing();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "红美铃";
            info.AbilityRadar = new AbilityRadar() { Attack = 3, Defence = 4, Control = 2, Auxiliary = 1, LastStages = 4, Difficulty = 5 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "HongMeiLing");
            info.Skills.Add(new SkillInfo()
            {
                Name = "龙拳",
                Description = "当你使用或者打出【杀】或者【闪】时，你可以选择一名其他角色，获取其一张手牌。"
            });
            SkillInfo skill_1 = new SkillInfo()
            {
                Name = "龙门",
                Description = "觉醒技，你的回合开始阶段，当你体力为全场最少（之一）时，你失去一点体力上限，选择恢复一点体力或者摸两张牌，并获得技能【太极】。"
            };
            skill_1.AttachedSkills.Add(new SkillInfo()
            {
                Name = "太极",
                Description = "你可以将【杀】当【闪】，【闪】当【杀】打出。你以此法打出的【杀】不计入使用杀的次数。"
            });
            info.Skills.Add(skill_1);
            return info;
        }
    }

    public class HongMeiLing_Skill_0 : Skill
    {
        public HongMeiLing_Skill_0()
        {
            IsLocked = false;
            Triggers.Add(new HongMeiLing_Skill_0_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new HongMeiLing_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new HongMeiLing_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "龙拳",
                Description = "当你打出【杀】或者【闪】时，你可以选择一名其他角色，获取其一张手牌。"
            };
        }
    }

    public class HongMeiLing_Skill_0_Trigger_0 : SkillTrigger, ITriggerInEvent, ITriggerAsk
    {
        public HongMeiLing_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new HongMeiLing_Skill_0_Trigger_0_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => CardEvent.DefaultKeyName; }

        string ITriggerAsk.Message { get => "是否要发动【龙拳】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return  skill.Owner; }

        public override void Action(Context ctx)
        {
            ctx.World.SelectPlayer(skill.KeyName, "请选择一名角色，获取其一张牌。", skill.Owner,
                new FulfillNumberPlayerFilter(1, 1, skill.Owner),
                true, 15,
                (targets) =>
                {
                    SkillEvent ev_skill = new SkillEvent();
                    ev_skill.Reason = ctx.Ev;
                    ev_skill.Skill = skill;
                    ev_skill.Source = skill.Owner;
                    ev_skill.Targets.Clear();
                    ev_skill.Targets.AddRange(targets);
                    ctx.World.InvokeEvent(ev_skill);
                    if (ev_skill.Cancel) return;
                    foreach (Player target in ev_skill.Targets)
                        ctx.World.StealTargetCard(skill.Owner, target, 1, ev_skill, true, true);
                }, null);
        }
    } 
    
    public class HongMeiLing_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public HongMeiLing_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(ctx.Ev);
            if (ev.Source != skill.Owner) return false;
            if (KillCard.IsKill(ev.Card)) return true;
            if (ev.Card.KeyName?.Equals(MissCard.Normal) == true) return true;
            return false;
        }

    }

    public class HongMeiLing_Skill_1 : Skill
    {
        public HongMeiLing_Skill_1()
        {
            IsOnce = true;
            IsLocked = true;
            Triggers.Add(new HongMeiLing_Skill_1_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new HongMeiLing_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new HongMeiLing_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "龙门",
                Description = "觉醒技，你的回合开始阶段，当你体力为全场最少（之一）时，你失去一点体力上限，恢复一点体力或者摸两张牌，获得技能【太极】。"
            };
        }
    }

    public class HongMeiLing_Skill_1_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public const string Selection_0 = "摸两张牌";
        public const string Selection_1 = "恢复一点体力";

        public HongMeiLing_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new HongMeiLing_Skill_1_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Begin; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        public override void Action(Context ctx)
        {
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            ctx.World.ChangeMaxHp(skill.Owner, -1, ev_skill);
            ctx.World.ShowList(skill.KeyName, "请选择一项。", skill.Owner,
                new List<object>() { Selection_0, Selection_1 }, 1,
                false, 15,
                (selected) =>
                {
                    switch (selected.ToString())
                    {
                        case Selection_0:
                            ctx.World.DrawCard(skill.Owner, 2, ev_skill);
                            break;
                        case Selection_1:
                            ctx.World.Heal(skill.Owner, skill.Owner, 1, HealEvent.Normal, ev_skill);
                            break;
                    }
                }, null);
            Player owner = skill.Owner;
            owner.Skills.Remove(skill);
            owner.Skills.Add(new HongMeiLing_Skill_2());
        }
    }

    public class HongMeiLing_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public HongMeiLing_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            if (ctx.World.Players.FirstOrDefault(_player =>
            {
                if (_player == skill.Owner) return false;
                if (!_player.IsAlive) return false;
                if (_player.HP < skill.Owner.HP) return true;
                return false;
            }) != null) return false;
            return true;
        }

    }

    public class HongMeiLing_Skill_2 : Skill, ISkillCardMultiConverter, ISkillCardMultiConverter2
    {

        private HongMeiLing_Skill_2_UseCondition condition;
        ConditionFilter ISkillCardConverter.UseCondition
        {
            get { return this.condition; }
        }

        private HongMeiLing_Skill_2_CardFilter cardfilter;
        CardFilter ISkillCardConverter.CardFilter
        {
            get { return this.cardfilter; }
        }

        private HongMeiLing_Skill_2_CardConverter cardconverter;
        CardCalculator ISkillCardConverter.CardConverter
        {
            get { return this.cardconverter; }
        }
        
        IEnumerable<string> ISkillCardMultiConverter.GetCardTypes(Context ctx)
        {
            return enabledcardtypes ?? new List<string>() { KillCard.Normal, MissCard.Normal };
        }

        void ISkillCardMultiConverter.SetSelectedCardType(Context ctx, string cardtype)
        {
            this.selectedcardtype = cardtype;
            cardfilter.SeemAs = ctx.World.GetCardInstance(cardtype);
        }

        private string selectedcardtype;
        string ISkillCardMultiConverter.SelectedCardType
        {
            get
            {
                return this.selectedcardtype;
            }
        }

        private List<string> enabledcardtypes;
        public IEnumerable<string> EnabledCardTypes
        {
            get
            {
                return this.enabledcardtypes;
            }
        }

        void ISkillCardMultiConverter2.SetEnabledCardTypes(Context ctx, IEnumerable<string> cardtypes)
        {
            this.enabledcardtypes = new List<string>();
            foreach (string cardtype in cardtypes)
            {
                switch (cardtype)
                {
                    case KillCard.Normal:
                    case MissCard.Normal:
                        enabledcardtypes.Add(cardtype);
                        break;
                }
            }
        }

        void ISkillCardMultiConverter2.CancelEnabledCardTypes(Context ctx)
        {
            this.enabledcardtypes = null;
        }

        public override Skill Clone()
        {
            return new HongMeiLing_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new HongMeiLing_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "太极",
                Description = "你可以将【杀】当【闪】，【闪】当【杀】打出。你以此法打出的【杀】不计入使用杀的次数。"
            };
        }
    }
   
    public class HongMeiLing_Skill_2_UseCondition : ConditionFilterFromSkill
    {
        public HongMeiLing_Skill_2_UseCondition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            Zone handzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return false;
            if (handzone.Cards.Count() == 0) return false;
            return true;
        }
    }

    public class HongMeiLing_Skill_2_CardFilter : CardFilterFromSkill
    {
        public HongMeiLing_Skill_2_CardFilter(Skill _skill) : base(_skill)
        {

        }

        private Card seemas;
        public Card SeemAs
        {
            get { return this.seemas; }
            set { this.seemas = value; }
        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() >= 1) return false;
            if (want.Zone?.KeyName?.Equals(Zone.Hand) != true) return false;
            switch (seemas?.KeyName)
            {
                case KillCard.Normal:
                    return want.KeyName?.Equals(MissCard.Normal) == true;
                case MissCard.Normal:
                    return KillCard.IsKill(want);
            }
            return false;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() >= 0;
        }
    }

    public class HongMeiLing_Skill_2_CardConverter : CardCalculatorFromSkill
    {
        public HongMeiLing_Skill_2_CardConverter(Skill _skill) : base(_skill)
        {

        }

        public override Card GetValue(Context ctx, Card oldvalue)
        {
            if (KillCard.IsKill(oldvalue))
            {
                Card misscard = ctx.World.GetCardInstance(MissCard.Normal);
                misscard = misscard.Clone(oldvalue);
                return misscard;
            }
            else if (oldvalue?.KeyName?.Equals(MissCard.Normal) == true)
            {
                Card killcard = ctx.World.GetCardInstance(KillCard.Normal);
                killcard = killcard.Clone(oldvalue);
                return killcard;
            }
            return oldvalue;
        }

    }

    


}
