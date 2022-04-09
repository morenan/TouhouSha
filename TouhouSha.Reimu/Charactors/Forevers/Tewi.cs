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
    /// 角色【因幡帝】
    /// </summary>
    /// <remarks>
    /// HP:3 竹林
    /// 【幸运】场上的判定牌生效前，你可以获得这张判定牌并令其重新进行判定。
    /// 【挖坑】出牌阶段，你可以将一张手牌作为【兔子陷阱】放置到一名角色的判定区（若其判定区没有【兔子陷阱】）。
    /// 【兔子陷阱】根据判定结果以下效果适用：
    ///     ♥：你视作对自己使用了一张【桃】。如果【兔子陷阱】的花色也是♥，这张【桃】额外回复一点体力。
    ///     ♦：你视作对自己使用了一张【无中生有】。如果【兔子陷阱】的花色也是♦，这张【无中生有】额外摸一张牌。
    ///     ♠：你受到一点雷电伤害。如果【兔子陷阱】的花色也是♠，你额外受到一点雷电伤害。
    ///     ♣：你弃置两张牌。如果【兔子陷阱】的花色也是♦，你额外弃置一张牌。
    /// </remarks>
    public class Tewi : Charactor
    {
        public Tewi()
        {
            MaxHP = 3;
            HP = 3;
            Country = Kaguya.CountryNameOfLeader;
            Skills.Add(new Tewi_Skill_0());
            Skills.Add(new Tewi_Skill_1());
        }

        public override Charactor Clone()
        {
            return new Tewi();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "因幡帝";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Tewi");
            info.AbilityRadar = new AbilityRadar() { Attack = 2, Defence = 3, Control = 3, Auxiliary = 4, LastStages = 4, Difficulty = 2 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }

    }
    
    public class Tewi_Skill_0 : Skill
    {
        public Tewi_Skill_0()
        {
            Triggers.Add(new Tewi_Skill_0_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Tewi_Skill_0();
        }
        
        public override Skill Clone(Card newcard)
        {
            return new Tewi_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "幸运",
                Description = "场上的判定牌生效前，你可以获得这张判定牌并将牌堆顶一张牌重新作为判定牌。"
            };
        }
    }
   
    public class Tewi_Skill_0_Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Tewi_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Tewi_Skill_0_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        string ITriggerAsk.Message { get => "是否要发动【幸运】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }
        
        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state?.KeyName?.Equals(State.Handle) != true) return;
            if (!(state.Ev is JudgeEvent)) return;
            JudgeEvent ev = (JudgeEvent)(state.Ev);
            Zone desktop = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Desktop) == true);
            if (desktop == null) return;
            Zone drawzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Draw) == true);
            if (drawzone == null) return;
            Zone handzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return;
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(ev.JudgeTarget);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            List<Card> topone = ctx.World.GetDrawTops(1);
            ctx.World.MoveCards(skill.Owner, ev.JudgeCards, handzone, ev_skill);
            ctx.World.MoveCards(skill.Owner, topone, desktop, ev_skill);
            ev.JudgeCards.Clear();
            ev.JudgeCards.AddRange(topone);
        }
    }

    public class Tewi_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Tewi_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state?.KeyName?.Equals(State.Handle) != true) return false;
            if (!(state.Ev is JudgeEvent)) return false;
            return true;
        }
    }

    public class Tewi_Skill_1 : Skill, ISkillCardConverter
    {
        public Tewi_Skill_1()
        {
            this.usecondition = new Tewi_Skill_1_UseCondition(this);
            this.cardfilter = new Tewi_Skill_1_CardFilter(this);
            this.cardconverter = new Tewi_Skill_1_CardConverter(this);
        }

        private Tewi_Skill_1_UseCondition usecondition;

        private Tewi_Skill_1_CardFilter cardfilter;

        private Tewi_Skill_1_CardConverter cardconverter;
        
        ConditionFilter ISkillCardConverter.UseCondition => usecondition;

        CardFilter ISkillCardConverter.CardFilter => cardfilter;

        CardCalculator ISkillCardConverter.CardConverter => cardconverter;

        public override Skill Clone()
        {
            return new Tewi_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Tewi_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            SkillInfo skillinfo = new SkillInfo()
            {
                Name = "挖坑",
                Description = "出牌阶段，你可以将一张手牌作为【兔子陷阱】放置到一名角色的判定区（若其判定区没有【兔子陷阱】）。"
            };
            skillinfo.AttachedSkills.Add(new SkillInfo()
            {
                Name = "兔子陷阱",
                Description = "延时锦囊牌。根据判定结果以下效果适用：\n" +
                    "\t♥：你视作对自己使用了一张【桃】。如果【兔子陷阱】的花色也是♥，这张【桃】额外回复一点体力。\n" +
                    "\t♦：你视作对自己使用了一张【无中生有】。如果【兔子陷阱】的花色也是♦，这张【无中生有】额外摸一张牌。\n" +
                    "\t♠：你受到一点雷电伤害。如果【兔子陷阱】的花色也是♠，你额外受到一点雷电伤害。\n" +
                    "\t♣：你弃置两张牌。如果【兔子陷阱】的花色也是♣，你额外弃置一张牌。"
            });
            return skillinfo;
        }
    }

    public class Tewi_Skill_1_UseCondition : ConditionFilterFromSkill
    {
        public Tewi_Skill_1_UseCondition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            if (state.KeyName?.Equals(State.UseCard) != true) return false;
            return true;
        }
    }

    public class Tewi_Skill_1_CardFilter : CardFilterFromSkill
    {
        public Tewi_Skill_1_CardFilter(Skill _skill) : base(_skill)
        {

        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() >= 1) return false;
            if (want.Zone?.KeyName?.Equals(Zone.Hand) != true) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() >= 1;
        }
    }

    public class Tewi_Skill_1_CardConverter : CardCalculatorFromSkill
    {
        public Tewi_Skill_1_CardConverter(Skill _skill) : base(_skill)
        {

        }

        public override Card GetValue(Context ctx, Card oldvalue)
        {
            return RabbitTrap.Instance.Clone(oldvalue);
        }
    }
    
    public class RabbitTrap : Card
    {
        public const string DefaultKeyName = "兔子陷阱";
        static public readonly RabbitTrap Instance = new RabbitTrap();

        public RabbitTrap()
        {
            KeyName = DefaultKeyName;
            CardType = new CardType(Enum_CardType.Spell, new CardSubType(Enum_CardSubType.Delay));
            UseCondition = new UseRabbitTrapCondition();
            TargetFilter = new UseRabbitTrapTargetFilter();
        }

        public override Card Create()
        {
            return new RabbitTrap();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = "兔子陷阱",
                Description = "延时锦囊牌。根据判定结果以下效果适用：\n" +
                    "\t♥：你视作对自己使用了一张【桃】。如果【兔子陷阱】的花色也是♥，这张【桃】额外回复一点体力。\n" +
                    "\t♦：你视作对自己使用了一张【无中生有】。如果【兔子陷阱】的花色也是♦，这张【无中生有】额外摸一张牌。\n" +
                    "\t♠：你受到一点雷电伤害。如果【兔子陷阱】的花色也是♠，你额外受到一点雷电伤害。\n" +
                    "\t♣：你弃置两张牌。如果【兔子陷阱】的花色也是♣，你额外弃置一张牌。"
            };
        }
        public override double GetWorthForTarget()
        {
            return 0;
        }
    }

    public class UseRabbitTrapCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用兔子陷阱的条件";

        public UseRabbitTrapCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            Player player = state.Owner;
            if (player == null) return false;
            return true;
        }
    }

    public class UseRabbitTrapTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用兔子陷阱的目标";

        public UseRabbitTrapTargetFilter()
        {
            KeyName = DefaultKeyName;
        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            Player player = state.Owner;
            if (player == null) return false;
            if (player == want) return false;
            if (!ctx.World.IsInDistance(ctx, player, want)) return false;
            Zone judgezone = want.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
            if (judgezone == null) return false;
            Card trap = judgezone.Cards.FirstOrDefault(_card => _card.KeyName?.Equals(RabbitTrap.DefaultKeyName) == true);
            if (trap != null) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return true;
        }
    }

}
