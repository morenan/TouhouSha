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
    /// 角色【上白泽慧音】
    /// </summary>
    /// <remarks>
    /// HP:3/14 竹林
    /// 【白泽】转换技：
    ///     阳：当场上角色回复体力时，你将弃牌堆中不同点数的牌尽可能的放置到你的角色牌上作为【历史】。你每有一个【历史】，你的体力上限-1。
    ///     阴：当场上角色受到伤害前，你将所有的【历史】以任意顺序放置到牌堆底，并防止此伤害。
    /// 【神器】其他角色的出牌阶段限一次，可以选择将装备栏里的一张装备移动到你的装备栏，你与其各回复一点体力。
    /// 【危机】其他角色受到不小于两点的伤害时，可令其弃置一张手牌，并回复一点体力。
    /// </remarks>
    public class Keine : Charactor
    {
        public const string Zone_History = "历史";

        public Keine()
        {
            MaxHP = 14;
            HP = 3;
            Country = Kaguya.CountryNameOfLeader;
            Skills.Add(new Keine_Skill_0());
            Skills.Add(new Keine_Skill_1());
            //Skills.Add(new Keine_Skill_2());
        }

        public override Charactor Clone()
        {
            return new Keine();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "上白泽慧音";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Keine");
            info.AbilityRadar = new AbilityRadar() { Attack = 1, Defence = 5, Control = 3, Auxiliary = 5, LastStages = 2, Difficulty = 0 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }
    }

    public class Keine_Skill_0 : Skill
    {
        public const string Dark = "阴";
       
        public Keine_Skill_0()
        {
            Triggers.Add(new Keine_Skill_0_Trigger_0(this));
            Triggers.Add(new Keine_Skill_0_Trigger_1(this));
            Triggers.Add(new Keine_Skill_0_Trigger_2(this));
        }

        public override Skill Clone()
        {
            return new Keine_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Keine_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "白泽",
                Description = "转换技：\n" +
                    "\t阳：当场上角色回复体力时，你将弃牌堆中不同点数的牌尽可能的放置到你的角色牌上作为【历史】。你每有一个【历史】，你的体力上限-1。\n" +
                    "\t阴：当场上角色受到伤害前，你将所有的【历史】以任意顺序放置到牌堆底，并防止此伤害。"
            };
        }
    }
    
    public class Keine_Skill_0_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public Keine_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Keine_Skill_0_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.GameStart; }
        
        int ITriggerInState.StateStep { get => 0; }

        public override void Action(Context ctx)
        {
            Zone historyzone = new Zone();
            historyzone.Owner = skill.Owner;
            historyzone.KeyName = Keine.Zone_History;
            skill.Owner.Zones.Add(historyzone);
        }
    }

    public class Keine_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Keine_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }

    public class Keine_Skill_0_Trigger_1 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Keine_Skill_0_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Keine_Skill_0_Trigger_1_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Healed; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        string ITriggerAsk.Message { get => "是否要发动【白泽】？"; }
        
        Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (!(state.Ev is HealEvent)) return;
            HealEvent ev = (HealEvent)(state.Ev);
            Zone discardzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
            if (discardzone == null) return;
            Zone historyzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Keine.Zone_History) == true);
            if (historyzone == null) return;
            List<List<Card>> cardofpts = new List<List<Card>>();
            List<Card> selecteds = new List<Card>();
            foreach (Card card in discardzone.Cards)
            {
                if (card.CardPoint <= 0) continue;
                while (card.CardPoint >= cardofpts.Count())
                    cardofpts.Add(null);
                List<Card> cardlist = cardofpts[card.CardPoint];
                if (cardlist == null)
                {
                    cardlist = new List<Card>();
                    cardofpts[card.CardPoint] = cardlist;
                }
                cardlist.Add(card);
            }
            if (cardofpts.Count() == 0) return;
            skill.SetValue(Keine_Skill_0.Dark, 1);
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            for (int i = 0;  i < cardofpts.Count(); i++)
            {
                List<Card> cardlist = cardofpts[i];
                if (cardlist == null) continue;
                if (cardlist.Count() == 1)
                {
                    selecteds.Add(cardlist[0]);
                    continue;
                }
                DesktopCardBoardCore desktop_core = new DesktopCardBoardCore();
                DesktopCardBoardZone desktop_zone = new DesktopCardBoardZone(desktop_core);
                desktop_zone.Message = "相同点数的卡";
                desktop_core.CardFilter = new Keine_Skill_0_SelectOneCard();
                desktop_core.Flag = Enum_DesktopCardBoardFlag.CannotNo;
                desktop_core.Zones.Add(desktop_zone);
                ctx.World.ShowDesktop(skill.Owner, desktop_core, new List<IList<Card>> { cardlist }, false, null);
                selecteds.AddRange(desktop_core.SelectedCards);
            }
            if (selecteds.Count() == 0) return;
            ctx.World.MoveCards(skill.Owner, selecteds, historyzone, ev_skill);
            ctx.World.ChangeMaxHp(skill.Owner, -selecteds.Count(), ev_skill);
        }
    }

    public class Keine_Skill_0_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Keine_Skill_0_Trigger_1_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (!(state.Ev is HealEvent)) return false;
            HealEvent ev = (HealEvent)(state.Ev);
            if (ev.HealValue <= 0) return false;
            if (skill.GetValue(Keine_Skill_0.Dark) == 1) return false;
            return true;
        }
    }

    public class Keine_Skill_0_Trigger_2 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Keine_Skill_0_Trigger_2(Skill _skill) : base(_skill)
        {
            Condition = new Keine_Skill_0_Trigger_2_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Damaging; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        string ITriggerAsk.Message { get => "是否要发动【白泽】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (!(state.Ev is DamageEvent)) return;
            DamageEvent ev = (DamageEvent)(state.Ev);
            Zone drawzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Draw) == true);
            if (drawzone == null) return;
            Zone historyzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Keine.Zone_History) == true);
            if (historyzone == null) return;
            skill.SetValue(Keine_Skill_0.Dark, 0);
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            ev.DamageValue = 0;
            ev.Cancel = true;
            if (historyzone.Cards.Count() == 0) return;
            DesktopCardBoardCore desktop_core = new DesktopCardBoardCore();
            DesktopCardBoardZone desktop_zone = new DesktopCardBoardZone(desktop_core);
            desktop_zone.Message = "整理顺序并放置到牌堆底";
            desktop_core.Flag = Enum_DesktopCardBoardFlag.CannotNo;
            desktop_core.Zones.Add(desktop_zone);
            ctx.World.ShowDesktop(skill.Owner, desktop_core, new List<IList<Card>> { historyzone.Cards.ToList() }, false, null);
            ctx.World.MoveCards(skill.Owner, desktop_zone.Cards.ToList(), drawzone, ev_skill, Enum_MoveCardFlag.MoveToFirst);
            ctx.World.ChangeMaxHp(skill.Owner, desktop_zone.Cards.Count(), ev_skill);
        }
    }

    public class Keine_Skill_0_Trigger_2_Condition : ConditionFilterFromSkill
    {
        public Keine_Skill_0_Trigger_2_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (!(state.Ev is DamageEvent)) return false;
            DamageEvent ev = (DamageEvent)(state.Ev);
            if (ev.Cancel) return false;
            if (ev.DamageValue <= 0) return false;
            if (ev.DamageType?.Equals(DamageEvent.Lost) == true) return false;
            if (skill.GetValue(Keine_Skill_0.Dark) == 0) return false;
            return true;
        }
    }

    public class Keine_Skill_0_SelectOneCard : CardFilter
    {
        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            return selecteds.Count() < 1;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() >= 1;
        }
    }

    public class Keine_Skill_1 : Skill, ISkillInitative
    {
        public const string Used = "已经使用过神器";
       
        public Keine_Skill_1()
        {
            this.usecondition = new Keine_Skill_1_UseCondition(this);
            this.targetfilter = new Keine_Skill_1_TargetFilter(this);
            this.costfilter = new Keine_Skill_1_CostFilter(this);
            Triggers.Add(new Keine_Skill_1_Trigger_0(this));   
        }

        private Keine_Skill_1_UseCondition usecondition;
        ConditionFilter ISkillInitative.UseCondition { get => usecondition; }

        private Keine_Skill_1_TargetFilter targetfilter;
        PlayerFilter ISkillInitative.TargetFilter { get => targetfilter; }

        private Keine_Skill_1_CostFilter costfilter;
        CardFilter ISkillInitative.CostFilter { get => costfilter; }

        void ISkillInitative.Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return;
            if (state.Ev == null) return;
            state.Ev.SetValue(Keine_Skill_1.Used, 1);
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = this;
            ev_skill.Source = skilluser;
            ev_skill.Targets.Clear();
            ev_skill.Targets.AddRange(targets);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            Player target = ev_skill.Targets.FirstOrDefault();
            if (target == null) return;
            EquipZone equipzone = target.Zones.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
            if (equipzone == null) return;
            ctx.World.MoveCards(skilluser, costs, equipzone, ev_skill);
            ctx.World.Heal(target, target, 1, HealEvent.Normal, ev_skill);
            ctx.World.Heal(target, skilluser, 1, HealEvent.Normal, ev_skill);
        }

        public override Skill Clone()
        {
            return new Keine_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Keine_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "神器",
                Description = "其他角色的出牌阶段限一次，可以选择将装备栏里的一张装备移动到你的装备栏，你与其各回复一点体力。"
            };
        }
    }

    public class Keine_Skill_1_UseCondition : ConditionFilterFromSkill
    {
        public Keine_Skill_1_UseCondition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Ev == null) return false;
            if (state.Owner != skill.Owner) return false;
            if (state.Ev.GetValue(Keine_Skill_1.Used) == 1) return false;
            return true;
        }
    }

    public class Keine_Skill_1_TargetFilter : PlayerFilterFromSkill
    {
        public Keine_Skill_1_TargetFilter(Skill _skill) : base(_skill)
        {
            
        }

        public override Enum_PlayerFilterFlag GetFlag(Context ctx)
        {
            return Enum_PlayerFilterFlag.ForceAll;
        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            if (selecteds.Count() >= 1) return false;
            return want.Charactors.FirstOrDefault(_char => _char is Keine) != null;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() >= 1;
        }
    }

    public class Keine_Skill_1_CostFilter : CardFilterFromSkill
    {
        public Keine_Skill_1_CostFilter(Skill _skill) : base(_skill)
        {

        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() >= 1) return false;
            if (want.Zone?.KeyName?.Equals(Zone.Equips) != true) return false;
            Player keine = ctx.World.Players.FirstOrDefault(_player => _player.Charactors.FirstOrDefault(_char => _char is Keine) != null);
            if (keine == null) return false;
            EquipZone equipzone = keine.Zones.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
            if (equipzone == null) return false;
            if (want.CardType.SubType == null) return false;
            int cellindex = (int)(want.CardType.SubType.E);
            if (cellindex < 0) return false;
            if (cellindex >= equipzone.Cells.Count()) return false;
            if (!equipzone.Cells[cellindex].IsEnabled) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() >= 1;
        }
    }

    public class Keine_Skill_1_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public Keine_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Keine_Skill_1_Trigger_0_Condition(_skill);
        }

        string ITriggerInState.StateKeyName { get => State.GameStart; }

        int ITriggerInState.StateStep { get => 0; }

        public override void Action(Context ctx)
        {
            Player owner = skill.Owner;
            foreach (Player player in ctx.World.Players)
            {
                if (player == owner)
                    owner.Skills.Remove(skill);
                else
                    owner.Skills.Add(skill.Clone());
            }
        }
    }

    public class Keine_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Keine_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            return true;
        }


    }

    public class Keine_Skill_2 : Skill
    {
        public Keine_Skill_2()
        {
            Triggers.Add(new Keine_Skill_2_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Keine_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new Keine_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "危机",
                Description = "其他角色受到不小于两点的伤害时，可令其弃置一张手牌，并回复一点体力。",
            };
        }
    }

    public class Keine_Skill_2_Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Keine_Skill_2_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Keine_Skill_2_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Damaged; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        string ITriggerAsk.Message { get => "是否要发动【危机】？"; }
   
        Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }     

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (state.Owner == null) return;
            if (!(state.Ev is DamageEvent)) return;
            DamageEvent ev = (DamageEvent)(state.Ev);
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(state.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            ctx.World.DiscardCard(state.Owner, 1, ev_skill, false);
            ctx.World.Heal(skill.Owner, state.Owner, 1, HealEvent.Normal, ev_skill);
        }
    }

    public class Keine_Skill_2_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Keine_Skill_2_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner == null) return false;
            if (!(state.Ev is DamageEvent)) return false;
            DamageEvent ev = (DamageEvent)(state.Ev);
            if (ev.DamageValue <= 1) return false;
            if (ev.DamageType?.Equals(DamageEvent.Lost) == true) return false;
            Zone handzone = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return false;
            if (handzone.Cards.Count() == 0) return false;
            return true;
        }
    }

    


}
