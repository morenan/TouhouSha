using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.AnotherLands
{
    /// <summary>
    /// 角色【小野塚小町】
    /// </summary>
    /// <remarks>
    /// HP:4/4 彼岸
    /// 【渡魂】出牌阶段限一次，你可以弃置一张牌并指定场上其他一名角色，将其体力值相等数量的牌扣置到其角色牌上（回合结束返回持有者手卡）。
    /// 每扣置一张牌，你计算与其的距离-1直到回合结束。然后若该角色的体力值或者手牌数为全场最少的（或之一），你选择一项：1. 回复一点体力并摸一张牌。2.本回合你对其造成的伤害+1。 
    /// </remarks>
    public class Komachi : Charactor
    {
        public Komachi()
        {
            MaxHP = 4;
            HP = 4;
            Country = Shiki.CountryNameOfLeader;
            Skills.Add(new Komachi_Skill_0());
        }
        
        public override Charactor Clone()
        {
            return new Komachi();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "小野塚小町";
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            info.Image = ImageHelper.LoadCardImage("Charactors", "Komachi");
            info.AbilityRadar = new AbilityRadar() { Attack = 5, Defence = 2, Auxiliary = 1, Control = 3, LastStages = 4, Difficulty = 5 };
            return info;
        }

    }

    public class Komachi_Skill_0 : Skill, ISkillInitative
    {
        public const string Used = "已经使用过渡魂";
        public const string Zone_Cover = "渡魂";
        public const string Selection_0 = "0";
        public const string Selection_1 = "1";
        public const string DamagePlus = "渡魂选择伤害+1";

        static public Player DamageTarget;

        public Komachi_Skill_0()
        {
            this.usecondition = new Komachi_Skill_0_UseCondition(this);
            this.targetfilter = new Komachi_Skill_0_TargetFilter(this);
            this.costfilter = new Komachi_Skill_0_CostFilter(this);
            Triggers.Add(new Komachi_Skill_0_Trigger_0(this));
            Triggers.Add(new Komachi_Skill_0_Trigger_1(this));
            Calculators.Add(new Komachi_Skill_0_DistMinus(this));
        }

        public override Skill Clone()
        {
            return new Komachi_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Komachi_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "渡魂",
                Description = "出牌阶段限一次，你可以弃置一张牌并指定场上其他一名角色，将其体力值相等数量的牌扣置到其角色牌上（回合结束返回持有者手卡）。" +
                    "每扣置一张牌，你计算与其的距离-1直到回合结束。然后若该角色的体力值或者手牌数为全场最少的（或之一），你选择一项：1. 回复一点体力并摸一张牌。2.本回合你对其造成的伤害+1。"
            };
        }

        private Komachi_Skill_0_UseCondition usecondition;
        ConditionFilter ISkillInitative.UseCondition => usecondition;

        private Komachi_Skill_0_TargetFilter targetfilter;
        PlayerFilter ISkillInitative.TargetFilter => targetfilter;

        private Komachi_Skill_0_CostFilter costfilter;
        CardFilter ISkillInitative.CostFilter => costfilter;

        void ISkillInitative.Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return;
            state.SetValue(Used, 1);
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = this;
            ev_skill.Source = skilluser;
            ev_skill.Targets.Clear();
            ev_skill.Targets.AddRange(targets);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            foreach (Player target in targets)
            {
                Zone coverzone = target.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone_Cover) == true);
                if (coverzone == null)
                {
                    coverzone = new Zone();
                    coverzone.KeyName = Zone_Cover;
                    coverzone.Flag = Enum_ZoneFlag.LabelOnPlayer;
                    coverzone.Flag |= Enum_ZoneFlag.CardUnvisibled;
                    target.Zones.Add(coverzone);
                }
                Zone handzone = target.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                ctx.World.SelectTargetCard(skilluser, target, target.HP, true, false,
                    (cards) =>
                    {
                        ctx.World.MoveCards(skilluser, cards, coverzone, ev_skill);
                        int handmin = ctx.World.Players.Min(_player =>
                        {
                            Zone _handzone = _player.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                            if (_handzone == null) return 0;
                            return _handzone.Cards.Count();
                        });
                        int hpmin = ctx.World.Players.Min(_player => _player.HP);
                        if (target.HP <= hpmin || handzone != null && handzone.Cards.Count() <= handmin)
                        {
                            ctx.World.ShowList("渡魂选择附加项", "请选择一项。", skilluser,
                                new List<object>() { Selection_0, Selection_1 }, 1,
                                false, 15,
                                (selected) =>
                                {
                                    switch (selected.ToString())
                                    {
                                        case Selection_0:
                                            ctx.World.Heal(skilluser, skilluser, 1, HealEvent.Normal, ev_skill);
                                            ctx.World.DrawCard(skilluser, 1, ev_skill);
                                            break;
                                        case Selection_1:
                                            if (state.Ev == null) break;
                                            state.Ev.SetValue(DamagePlus, 1);
                                            DamageTarget = target;
                                            break;
                                    }
                                }, null);
                        }
                    }, null);
            }
        }
    }

    public class Komachi_Skill_0_UseCondition : ConditionFilterFromSkill
    {
        public Komachi_Skill_0_UseCondition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            return state.GetValue(Komachi_Skill_0.Used) != 1;
        }

    }

    public class Komachi_Skill_0_TargetFilter : PlayerFilterFromSkill
    {
        public Komachi_Skill_0_TargetFilter(Skill _skill) : base(_skill)
        {

        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            if (selecteds.Count() >= 1) return false;
            if (want == skill.Owner) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() >= 1;
        }

    }

    public class Komachi_Skill_0_CostFilter : CardFilterFromSkill
    {
        public Komachi_Skill_0_CostFilter(Skill _skill) : base(_skill)
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

    public class Komachi_Skill_0_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public Komachi_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Komachi_Skill_0_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName => State.End;
        int ITriggerInState.StateStep => 0;

        public override void Action(Context ctx)
        {
            foreach (Player player in ctx.World.Players)
            {
                if (!player.IsAlive) continue;
                Zone coverzone = player.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Komachi_Skill_0.Zone_Cover) == true);
                if (coverzone == null) continue;
                Zone handzone = player.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                if (handzone == null) continue;
                ctx.World.MoveCards(skill.Owner, coverzone.Cards.ToList(), handzone, null);
            }
        }
    }

    public class Komachi_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Komachi_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            return true;
        }

    }

    public class Komachi_Skill_0_Trigger_1 : SkillTrigger, ITriggerInState
    {
        public Komachi_Skill_0_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Komachi_Skill_0_Trigger_1_Condition(skill);
        }

        string ITriggerInState.StateKeyName => State.Damaging;

        int ITriggerInState.StateStep => StateChangeEvent.Step_Start;

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is DamageEvent)) return;
            DamageEvent ev = (DamageEvent)(ctx.Ev);
            ev.DamageValue++;
        }
    }

    public class Komachi_Skill_0_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Komachi_Skill_0_Trigger_1_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state.Owner != skill.Owner) return false;
            if (state.Ev == null) return false;
            if (state.Ev.GetValue(Komachi_Skill_0.DamagePlus) != 1) return false;
            if (!(ctx.Ev is DamageEvent)) return false;
            DamageEvent ev = (DamageEvent)(ctx.Ev);
            if (ev.Source != skill.Owner) return false;
            if (ev.Target != Komachi_Skill_0.DamageTarget) return false;
            if (ev.DamageType?.Equals(DamageEvent.Lost) == true) return false;
            return true;
        }

    }

    public class Komachi_Skill_0_DistMinus : CalculatorFromSkill
    {
        public Komachi_Skill_0_DistMinus(Skill _skill) : base(_skill)
        {

        }

        public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            if (obj != skill.Owner) return oldvalue;
            State state = ctx.World.GetPlayerState();
            if (state == null) return oldvalue;
            if (state.Ev == null) return oldvalue;
            switch (propertyname)
            {
                //case World.DistanceMinus:
                //    return oldvalue + state.Ev.GetValue(Suwako_Skill_3.DistMinus);
                default:
                    if (propertyname.EndsWith(World.DistanceMinus))
                    {
                        string target_keyname = propertyname.Substring(0, propertyname.Length - World.DistanceMinus.Length);
                        Player target = ctx.World.Players.FirstOrDefault(_target => _target.KeyName?.Equals(target_keyname) == true);
                        if (target == null) return oldvalue;
                        Zone coverzone = target.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Komachi_Skill_0.Zone_Cover) == true);
                        return oldvalue + coverzone.Cards.Count();
                    }
                    break;
            }
            return oldvalue;
        }

    }
}
