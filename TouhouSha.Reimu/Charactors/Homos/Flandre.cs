using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Koishi.Cards;
using TouhouSha.Koishi.Cards.Weapons;
using TouhouSha.Koishi.Triggers;
using TouhouSha.Koishi.Calculators;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Homos
{
    /// <summary>
    /// 角色【芙兰朵路】
    /// </summary>
    /// <remarks>
    /// 势力：红魔 4勾玉
    /// 【血族】：当你造成伤害时，每有一点你可以选择其中一项：1.恢复一点体力，2.摸一张牌。
    /// 【炎剑】：当你没有装备武器时，视作你装备了【瓦莱丁】。
    /// 【分身】：出牌阶段，你可以将牌堆顶的牌置于你的武将牌上作为【分身】。
    ///     你每有一个【分身】，你的体力上限-1。
    ///     当你造成伤害或者受到伤害时，你可以丢弃X张【分身】，令此伤害+X或者-X。
    /// </remarks>
    public class Flandre : Charactor
    {
        public Flandre()
        {
            MaxHP = 4;
            HP = 4;
            Country = Remilia.CountryNameOfLeader;
            Skills.Add(new Flandre_Skill_0());
            Skills.Add(new Flandre_Skill_1());
            Skills.Add(new Flandre_Skill_2());
        }

        public override Charactor Clone()
        {
            return new Flandre();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "芙兰朵路";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Flandre");
            info.AbilityRadar = new AbilityRadar() { Attack = 5, Defence = 4.5, Control = 1, Auxiliary = 1, LastStages = 5, Difficulty = 5 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }
    }

    public class Flandre_Skill_0 : Skill
    {
        public Flandre_Skill_0()
        {
            IsLocked = true;
            Triggers.Add(new Flandre_Skill_0_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Flandre_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Flandre_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "血族",
                Description = "锁定技，当你造成伤害时，每有一点你可以选择其中一项：1.恢复一点体力，2.摸一张牌。"
            };
        }
    }

    public class Flandre_Skill_0_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public const string Selection_0 = "恢复一点体力";
        public const string Selection_1 = "摸一张牌";

        public Flandre_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Flandre_Skill_0_Trigger_0_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => DamageEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is DamageEvent)) return;
            DamageEvent ev = (DamageEvent)(ctx.Ev);
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            for (int i = 0; i < ev.DamageValue; i++)
            {
                string selection = Selection_1;
                if (skill.Owner.HP < skill.Owner.MaxHP)
                    ctx.World.ShowList(skill.KeyName, "请选择一项：", skill.Owner,
                        new List<object>() { Selection_0, Selection_1 },
                        1, false, 15,
                        (selecteds) => { selection = selecteds.FirstOrDefault().ToString(); },
                        null);
                switch (selection)
                {
                    case Selection_0:
                        ctx.World.Heal(skill.Owner, skill.Owner, 1, HealEvent.Normal, ev_skill);
                        break;
                    case Selection_1:
                        ctx.World.DrawCard(skill.Owner, 1, ev_skill);
                        break;
                }
            }
        }

    }

    public class Flandre_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Flandre_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is DamageEvent)) return false;
            DamageEvent ev = (DamageEvent)(ctx.Ev);
            if (ev.Source != skill.Owner) return false;
            if (ev.DamageType?.Equals(DamageEvent.Lost) == true) return false;
            return true;
        }
    }

    public class Flandre_Skill_1 : Skill
    {
        public Flandre_Skill_1()
        {
            IsLocked = true;
            Triggers.Add(new Flandre_Skill_1_Trigger_0(this));
            Calculators.Add(new Flandre_Skill_1_RangeCalculator(this));
        }
        public override Skill Clone()
        {
            return new Flandre_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Flandre_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "炎剑",
                Description = "锁定技，当你没有装备武器时，视作你装备了【瓦莱丁】。"
            };
        }
    }

    public class Flandre_Skill_1_RangeCalculator : CalculatorFromSkill, ICalculatorProperty
    {
        string ICalculatorProperty.PropertyName { get => World.KillRange; }

        public Flandre_Skill_1_RangeCalculator(Skill _skill) : base(_skill)
        {

        }

        public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            switch (propertyname)
            {
                case World.KillRange:
                    if (skill.Owner != obj) return oldvalue;
                    EquipZone equipzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
                    if (equipzone == null) return oldvalue;
                    EquipCell equipcell = equipzone.Cells.FirstOrDefault(_cell => _cell.E == Enum_CardSubType.Weapon);
                    if (equipcell == null) return oldvalue;
                    if (!equipcell.IsEnabled) return oldvalue;
                    if (equipcell.CardIndex >= 0) return oldvalue;
                    return oldvalue + 3;
            }
            return oldvalue;
        }
    }
    
    public class Flandre_Skill_1_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public Flandre_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Flandre_Skill_1_Trigger_0_Condition(skill);
        }
        
        string ITriggerInEvent.EventKeyName { get => CardEvent.DefaultKeyName; }
        
        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(ctx.Ev);
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Source = ev.Source;
            ev_skill.Skill = skill;
            ev_skill.Targets.Clear();
            ev_skill.Targets.AddRange(ev.Targets);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            Card firekill = ev.Card.Clone();
            firekill.OriginCards.Clear();
            firekill.OriginCards.Add(ev.Card);
            firekill.KeyName = KillCard.Fire;
            ev.Card = firekill;
        }
    }

    public class Flandre_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Flandre_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }
        
        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(ctx.Ev);
            if (!KillCard.IsKill(ev.Card)) return false;
            if (skill.Owner != ev.Source) return false;
            EquipZone equipzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
            if (equipzone == null) return false;
            EquipCell equipcell = equipzone.Cells.FirstOrDefault(_cell => _cell.E == Enum_CardSubType.Weapon);
            if (equipcell == null) return false;
            if (!equipcell.IsEnabled) return false;
            if (equipcell.CardIndex >= 0) return false;
            return true;
        }
    }

    public class Flandre_Skill_2 : Skill, ISkillInitative
    {
        public const string Zone_Rune = "分身";

        public Flandre_Skill_2()
        {
            this.usecondition = new Flandre_Skill_2_UseCondition(this);
            this.targetfilter = new Flandre_Skill_2_TargetFilter(this);
            this.costfilter = new Flandre_Skill_2_CostFilter(this);
        }

        private Flandre_Skill_2_UseCondition usecondition;
        public ConditionFilter UseCondition { get => usecondition; }

        private Flandre_Skill_2_TargetFilter targetfilter;
        public PlayerFilter TargetFilter { get => targetfilter; }

        private Flandre_Skill_2_CostFilter costfilter;
        public CardFilter CostFilter { get => costfilter; }

        public void Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
        {
            SkillEvent ev = new SkillEvent();
            ev.Skill = this;
            ev.Source = skilluser;
            ev.Targets.Clear();
            ev.Targets.AddRange(targets);
            ctx.World.InvokeEvent(ev);
            if (ev.Cancel) return;
            foreach (Player target in targets)
            {
                IEnumerable<Card> drawcards = ctx.World.GetDrawTops(1);
                Zone runezone = target.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone_Rune) == true);
                if (runezone == null) continue;
                ctx.World.MoveCards(skilluser, drawcards, runezone, ev);
            }
        }
        public override Skill Clone()
        {
            return new Flandre_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new Flandre_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "分身",
                Description = "出牌阶段，你可以将牌堆顶的牌置于你的武将牌上作为【分身】。" +
                    "你每有一个【分身】，你的体力上限-1。" +
                    "你造成伤害或者受到伤害时，你可以丢弃X张【分身】，令此伤害+X或者-X。"
            };
        }
    }

    public class Flandre_Skill_2_UseCondition : ConditionFilterFromSkill
    {
        public Flandre_Skill_2_UseCondition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            return skill.Owner.MaxHP > 1;
        }

    }

    public class Flandre_Skill_2_TargetFilter : PlayerFilterFromSkill 
    {
        public Flandre_Skill_2_TargetFilter(Skill _skill) : base(_skill)
        {
            
        }

        public override Enum_PlayerFilterFlag GetFlag(Context ctx)
        {
            return Enum_PlayerFilterFlag.ForceAll;
        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            return want == skill.Owner;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() >= 1;
        }

    }

    public class Flandre_Skill_2_CostFilter : CardFilterFromSkill
    {
        public Flandre_Skill_2_CostFilter(Skill _skill) : base(_skill)
        {
            
        }

        public override Enum_CardFilterFlag GetFlag(Context ctx)
        {
            return Enum_CardFilterFlag.ForceAll;
        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            return false;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return true;
        }
    }

    public class Flandre_Skill_2_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public Flandre_Skill_2_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Flandre_Skill_2_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.GameStart; }

        int ITriggerInState.StateStep { get => 0; }

        public override void Action(Context ctx)
        {
            Zone runezone = new Zone();
            runezone.Owner = skill.Owner;
            runezone.KeyName = Flandre_Skill_2.Zone_Rune;
            runezone.Flag = Enum_ZoneFlag.LabelOnPlayer;
            runezone.Owner.Zones.Add(runezone);
        }
    }

    public class Flandre_Skill_2_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Flandre_Skill_2_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }

    public class Flandre_Skill_2_Trigger_1 : SkillTrigger, ITriggerInEvent
    {
        public Flandre_Skill_2_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Flandre_Skill_2_Trigger_1_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => MoveCardDoneEvent.DefaultKeyName; }
        
        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is MoveCardDoneEvent)) return;
            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
            if (ev.OldZone?.Owner == skill.Owner
             && ev.OldZone.KeyName?.Equals(Flandre_Skill_2.Zone_Rune) == true)
            {
                ctx.World.ChangeMaxHp(skill.Owner, 1, ev);
                return;
            }
            if (ev.NewZone?.Owner == skill.Owner
             && ev.NewZone.KeyName?.Equals(Flandre_Skill_2.Zone_Rune) == true)
            {
                ctx.World.ChangeMaxHp(skill.Owner, -1, ev);
                return;
            }
        }
    }

    public class Flandre_Skill_2_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Flandre_Skill_2_Trigger_1_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is MoveCardDoneEvent)) return false;
            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
            if (ev.OldZone?.Owner == skill.Owner
             && ev.OldZone.KeyName?.Equals(Flandre_Skill_2.Zone_Rune) == true) return true;
            if (ev.NewZone?.Owner == skill.Owner
             && ev.NewZone.KeyName?.Equals(Flandre_Skill_2.Zone_Rune) == true) return true;
            return false;
        }
    }
    
    public class Flandre_Skill_2_Trigger_2 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Flandre_Skill_2_Trigger_2(Skill _skill) : base(_skill)
        {
            Condition = new Flandre_Skill_2_Trigger_2_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Damaging; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        string ITriggerAsk.Message { get => "是否要发动【分身】？"; }
       
        Player ITriggerAsk.GetAsked(Context ctx) { return  skill.Owner; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (!(state.Ev is DamageEvent)) return;
            DamageEvent ev = (DamageEvent)(state.Ev);
            Zone runezone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Flandre_Skill_2.Zone_Rune) == true);
            if (runezone == null) return;
            Zone discardzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
            if (discardzone == null) return;
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            if (ev.Source == skill.Owner)
            {
                while (runezone.Cards.Count() > 0 
                    && ev.DamageValue > 0
                    && ctx.World.Ask(skill.Owner, skill.KeyName, "是否要去除一个分身令伤害+1？"))
                {
                    ctx.World.MoveCard(skill.Owner, runezone.Cards.FirstOrDefault(), discardzone, ev);
                    ev.DamageValue++;
                }
                        
            }
            if (ev.Target == skill.Owner)
            {
                while (runezone.Cards.Count() > 0
                    && ev.DamageValue > 0
                    && ctx.World.Ask(skill.Owner, skill.KeyName, "是否要去除一个分身令伤害-1？"))
                {
                    ctx.World.MoveCard(skill.Owner, runezone.Cards.FirstOrDefault(), discardzone, ev);
                    ev.DamageValue--;
                }
            }
        }
    }

    public class Flandre_Skill_2_Trigger_2_Condition : ConditionFilterFromSkill
    {
        public Flandre_Skill_2_Trigger_2_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (!(state.Ev is DamageEvent)) return false;
            DamageEvent ev = (DamageEvent)(state.Ev);
            Zone runezone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Flandre_Skill_2.Zone_Rune) == true);
            if (runezone == null) return false;
            Zone discardzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
            if (discardzone == null) return false;
            if (runezone.Cards.Count() == 0) return false;
            if (ev.Source == skill.Owner) return true;
            if (ev.Target == skill.Owner) return true;
            return false;
        }
    }
}
