using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Triggers;
using TouhouSha.Reimu.Charactors.SelfCrafts;

namespace TouhouSha.Reimu.Charactors.Fairy
{
    /// <summary>
    /// 角色【大妖精】
    /// </summary>
    /// <remarks>
    /// 妖精 HP:3
    /// 【自然】摸牌阶段你可以多摸X张牌，你的手牌上限加X（X为场上判定区的牌的数量）。
    /// 【应援】当你于回合内使用的牌置入弃牌堆后，你可以将之交给一名其他角色（相同牌名的牌每回合限一次）。每以此法交给两张牌，你回复一点体力。
    /// </remarks>
    public class Daiyosi : Charactor
    {
        public Daiyosi()
        {
            MaxHP = 3;
            HP = 3;
            Country = Crino.CountryNameOfLeader;
            Skills.Add(new Daiyosi_Skill_0());
            Skills.Add(new Daiyosi_Skill_1()); 
        }

        public override Charactor Clone()
        {
            return new Daiyosi();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "大妖精";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Daiyosi");
            info.AbilityRadar = new AbilityRadar() { Attack = 1, Defence = 2, Control = 1, Auxiliary = 5, LastStages = 1, Difficulty = 5 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }
    }

    public class Daiyosi_Skill_0 : Skill
    {
        public Daiyosi_Skill_0()
        {
            IsLocked = true;
            Calculators.Add(new Daiyosi_Skill_0_DrawMore(this));
            Calculators.Add(new Daiyosi_Skill_1_HandCapacity(this));
        }

        public override Skill Clone()
        {
            return new Daiyosi_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Daiyosi_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "自然",
                Description = "锁定技，摸牌阶段你可以多摸X张牌，你的手牌上限加X（X为场上判定区的牌的数量）。",
            };
        }
    }

    public class Daiyosi_Skill_0_DrawMore : CalculatorFromSkill
    {
        public Daiyosi_Skill_0_DrawMore(Skill _skill) : base(_skill)
        {
           
        }

        public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            switch (propertyname)
            {
                case DrawTrigger.ExtraDrawNumber:
                    if (obj != skill.Owner) return oldvalue;
                    return oldvalue + ctx.World.Players.Sum(_player =>
                    {
                        if (!(_player.IsAlive)) return 0;
                        Zone judgezone = _player.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
                        if (judgezone == null) return 0;
                        return judgezone.Cards.Count();
                    });
            }
            return oldvalue;
        }
    }
    
    public class Daiyosi_Skill_1_HandCapacity : CalculatorFromSkill, ICalculatorProperty
    {
        public Daiyosi_Skill_1_HandCapacity(Skill _skill) : base(_skill)
        {

        }

        string ICalculatorProperty.PropertyName { get => DiscardTrigger.ExtraHandCapacity; }

        public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            switch (propertyname)
            {
                case DiscardTrigger.ExtraHandCapacity:
                    if (obj != skill.Owner) return oldvalue;
                    return oldvalue + ctx.World.Players.Sum(_player =>
                    {
                        if (!(_player.IsAlive)) return 0;
                        Zone judgezone = _player.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
                        if (judgezone == null) return 0;
                        return judgezone.Cards.Count();
                    });
            }
            return oldvalue;
        }
    }

    public class Daiyosi_Skill_1 : Skill
    {
        public const string Marked = "_已经被应援过";

        public override Skill Clone()
        {
            return new Daiyosi_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Daiyosi_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "支援",
                Description = "当你于回合内使用的牌置入弃牌堆后，你可以将之交给一名其他角色（相同牌名的牌每回合限一次）。每以此法交给两张牌，你回复一点体力。",
            };
        }
    }

    public class Daiyosi_Skill_1_Trigger_0 : SkillTrigger, ITriggerInEvent, ITriggerAsk
    {
        public Daiyosi_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Daiyosi_Skill_1_Trigger_0_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => CardDoneEvent.DefaultKeyName; }
        
        string ITriggerAsk.Message { get => "是否要发动【应援】？"; }
       
        Player ITriggerAsk.GetAsked(Context ctx)
        {
            return skill.Owner;
        }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return;
            if (!(ctx.Ev is CardDoneEvent)) return;
            CardDoneEvent ev = (CardDoneEvent)(ctx.Ev);
            ctx.World.SelectPlayer(skill.KeyName, "请选择一名角色交给他。", skill.Owner,
                new FulfillNumberPlayerFilter(1, 1, skill.Owner),
                true, 15,
                (targets) =>
                {
                    SkillEvent ev_skill = new SkillEvent();
                    ev_skill.Reason = ev;
                    ev_skill.Skill = skill;
                    ev_skill.Source = skill.Owner;
                    ev_skill.Targets.Clear();
                    ev_skill.Targets.AddRange(targets);
                    ctx.World.InvokeEvent(ev_skill);
                    if (ev_skill.Cancel) return;
                    Player target = ev_skill.Targets.FirstOrDefault();
                    if (target == null) return;
                    Zone handzone = target.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                    if (handzone == null) return;
                    state.SetValue(ev.Card.KeyName + Daiyosi_Skill_1.Marked, 1);
                    ctx.World.MoveCard(skill.Owner, ev.Card, handzone, ev_skill);
                }, null);
        }
    }

    public class Daiyosi_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Daiyosi_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            if (state.KeyName?.Equals(State.UseCard) != true) return false;
            if (!(ctx.Ev is CardDoneEvent)) return false;
            CardDoneEvent ev = (CardDoneEvent)(ctx.Ev);
            if (ev.Source != skill.Owner) return false;
            if (state.GetValue(ev.Card.KeyName + Daiyosi_Skill_1.Marked) == 1) return false;
            switch (ev.Card.Zone.KeyName)
            {
                case Zone.Discard:
                case Zone.Desktop:
                    break;
                default:
                    return false;
            }
            return true;
        }
    }
    
   


}
