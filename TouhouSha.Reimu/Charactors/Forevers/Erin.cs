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
    /// 角色【八意永琳】
    /// </summary>
    /// <remarks>
    /// HP:3 永远
    /// 【蓬莱】锁定技，你的回合开始阶段，若你已受伤，你回复2点体力，若你未受伤，你增加一点体力上限并回复一点体力。
    /// 【制药】你可以将红色牌当作【桃】使用，出牌阶段你可以对其他受伤角色使用【桃】。当你使用♦【桃】时，你摸一张牌，你使用的♥【桃】额外回复一点体力。
    /// 【银河】你的回合结束阶段可以发动，你展示牌堆顶4张牌，并获得其中所有的红色牌，剩下的弃置。
    /// 【密葬】限定技，你的回合结束阶段可以丢弃X张不同花色的手牌并指定X名你以外的角色，直到你的下一次回合开始阶段将其移出游戏，移回游戏时你对其造成一点伤害。
    /// </remarks>
    public class Erin : Charactor
    {
        public Erin()
        {
            MaxHP = 3;
            HP = 3;
            Country = Kaguya.CountryNameOfLeader;
            Skills.Add(new Skill_PengLai());
            Skills.Add(new Erin_Skill_1());
            Skills.Add(new Erin_Skill_2());
            //Skills.Add(new Erin_Skill_3());
        }

        public override Charactor Clone()
        {
            return new Erin();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "八意永琳";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Erin");
            info.AbilityRadar = new AbilityRadar() { Attack = 1, Defence = 5, Control = 1, Auxiliary = 5, LastStages = 5, Difficulty = 5 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }

    }

    public class Skill_PengLai : Skill
    {
        public Skill_PengLai()
        {
            IsLocked = true;
            Triggers.Add(new Skill_PengLai_Trigger(this));
        }

        public override Skill Clone()
        {
            return new Skill_PengLai();
        }

        public override Skill Clone(Card newcard)
        {
            return new Skill_PengLai();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "蓬莱",
                Description = "锁定技，你的回合开始阶段，若你已受伤，你回复1点体力（场上角色大于3时额外回复1点体力），若你未受伤，你增加一点体力上限并回复一点体力。"
            };
        }
    }

    public class Skill_PengLai_Trigger : SkillTrigger, ITriggerInState
    {
        public Skill_PengLai_Trigger(Skill _skill) : base(_skill)
        {
            Condition = new Skill_PengLai_Trigger_Condition(skill);
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
            if (skill.Owner.HP < skill.Owner.MaxHP)
            {
                //int playercount = ctx.World.Players.Count(_player => _player.IsAlive);
                //ctx.World.Heal(skill.Owner, skill.Owner, playercount, HealEvent.Normal, ev_skill);
                ctx.World.Heal(skill.Owner, skill.Owner, 1, HealEvent.Normal, ev_skill);
            }
            else
            {
                ctx.World.ChangeMaxHp(skill.Owner, 1, ev_skill);
                ctx.World.Heal(skill.Owner, skill.Owner, 1, HealEvent.Normal, ev_skill);
            }
        }
    }

    public class Skill_PengLai_Trigger_Condition : ConditionFilterFromSkill
    {
        public Skill_PengLai_Trigger_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            return true;
        }
    }

    public class Erin_Skill_1 : Skill, ISkillInitative, ISkillCardConverter, ISkillCardMultiConverter2
    {
        public Erin_Skill_1()
        {
            this.usecondition = new Erin_Skill_1_UseCondition(this);
            this.cardfilter = new Erin_Skill_1_CardFilter(this);
            this.cardconverter = new Erin_Skill_1_CardConverter(this);
            this.targetfilter = new Erin_Skill_1_TargetFilter(this);
            Triggers.Add(new Erin_Skill_1_Trigger_0(this));
        }

        private Erin_Skill_1_UseCondition usecondition;
        ConditionFilter ISkillInitative.UseCondition { get => usecondition; }
        ConditionFilter ISkillCardConverter.UseCondition { get => usecondition; }

        private Erin_Skill_1_CardFilter cardfilter;
        CardFilter ISkillInitative.CostFilter { get => cardfilter; }
        CardFilter ISkillCardConverter.CardFilter { get => cardfilter; }

        private Erin_Skill_1_CardConverter cardconverter;
        CardCalculator ISkillCardConverter.CardConverter { get => cardconverter; }

        private Erin_Skill_1_TargetFilter targetfilter;
        PlayerFilter ISkillInitative.TargetFilter { get => targetfilter; }

        void ISkillInitative.Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
        {
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = this;
            ev_skill.Source = skilluser;
            ev_skill.Targets.Clear();
            ev_skill.Targets.AddRange(targets);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            Card card = cardconverter.GetValue(ctx, costs.FirstOrDefault());
            CardEvent ev_card = new CardEvent();
            ev_card.Reason = ev_skill;
            ev_card.Card = card;
            ev_card.Source = ev_skill.Source;
            ev_card.Targets.Clear();
            ev_card.Targets.AddRange(ev_skill.Targets);
            ctx.World.InvokeEvent(ev_card);
        }

        public void SetEnabledCardTypes(Context ctx, IEnumerable<string> cardtypes)
        {
            if (cardtypes.Contains(PeachCard.Normal))
                this.enabledcardtypes = new List<string>() { PeachCard.Normal };
        }

        public void CancelEnabledCardTypes(Context ctx)
        {
            this.enabledcardtypes = null;
        }

        private List<string> enabledcardtypes;
        public IEnumerable<string> EnabledCardTypes
        {
            get { return this.enabledcardtypes; }
        }

        public override Skill Clone()
        {
            return new Erin_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Erin_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "制药",
                Description = "你可以将红色牌当作【桃】使用，出牌阶段你可以对其他受伤角色使用【桃】。当你使用♦【桃】时，你摸一张牌，你使用的♥【桃】额外回复一点体力。"
            };
        }
    }

    public class Erin_Skill_1_UseCondition : ConditionFilterFromSkill
    {
        public Erin_Skill_1_UseCondition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }

    public class Erin_Skill_1_CardFilter : CardFilterFromSkill
    {
        public Erin_Skill_1_CardFilter(Skill _skill) : base(_skill)
        {
            
        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() >= 1) return false;
            if (want.Zone.KeyName?.Equals(Zone.Hand) != true) return false;
            if (want.CardColor?.SeemAs(Enum_CardColor.Red) != true) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() >= 1;
        }
    }

    public class Erin_Skill_1_CardConverter : CardCalculatorFromSkill
    {
        public Erin_Skill_1_CardConverter(Skill _skill) : base(_skill)
        {

        }

        public override Card GetValue(Context ctx, Card oldvalue)
        {
            Card peach = ctx.World.GetCardInstance(PeachCard.Normal);
            peach = peach.Clone(oldvalue);
            return peach;
        }

    }

    public class Erin_Skill_1_TargetFilter : PlayerFilterFromSkill
    {
        public Erin_Skill_1_TargetFilter(Skill _skill) : base(_skill)
        {
            
        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            if (selecteds.Count() >= 1) return false;
            if (want.HP >= want.MaxHP) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() >= 1;
        }
    }
    
    public class Erin_Skill_1_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public Erin_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Erin_Skill_1_Trigger_0_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => CardPreviewEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is CardPreviewEvent)) return;
            CardPreviewEvent ev = (CardPreviewEvent)(ctx.Ev);
            if (ev.Card.CardColor?.SeemAs(Enum_CardColor.Heart) == true)
            {
                int healvalue = ev.GetValue(CardEvent.HealValue);
                ev.SetValue(CardEvent.HealValue, healvalue + 1);
            }
            if (ev.Card.CardColor?.SeemAs(Enum_CardColor.Diamond) == true)
            {
                ctx.World.DrawCard(skill.Owner, 1, ev);
            }
        }
    }

    public class Erin_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Erin_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is CardPreviewEvent)) return false;
            CardPreviewEvent ev = (CardPreviewEvent)(ctx.Ev);
                if (ev.Card?.KeyName?.Equals(PeachCard.Normal) != true) return false;
            if (ev.Source != skill.Owner) return false;
            return true;
        }
    }

    public class Erin_Skill_2 : Skill
    {
        public Erin_Skill_2()
        {
            Triggers.Add(new Erin_Skill_2_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Erin_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new Erin_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "银河",
                Description = "你的回合结束阶段可以发动，你展示牌堆顶4张牌，并获得其中所有的红色牌，剩下的弃置。"
            };
        }

    }
  
    public class Erin_Skill_2_Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Erin_Skill_2_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Erin_Skill_2_Trigger_0_Condition(skill);
            Priority = 100;
        }

        string ITriggerInState.StateKeyName { get => State.End; }
        
        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        string ITriggerAsk.Message { get => "是否要发动【银河】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }

        public override void Action(Context ctx)
        {
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            List<Card> topcards = ctx.World.GetDrawTops(4);
            List<Card> redcards = topcards.Where(_card => _card.CardColor?.SeemAs(Enum_CardColor.Red) == true).ToList();
            List<Card> othercards = topcards.Where(_card => _card.CardColor?.SeemAs(Enum_CardColor.Red) != true).ToList();
            if (redcards.Count() > 0)
            {
                Zone handzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                if (handzone != null)
                    ctx.World.MoveCards(skill.Owner, redcards, handzone, ev_skill, Enum_MoveCardFlag.FaceUp);
            }
            if (othercards.Count() > 0)
            {
                Zone discardzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
                if (discardzone != null)
                    ctx.World.MoveCards(skill.Owner, redcards, discardzone, ev_skill, Enum_MoveCardFlag.FaceUp);
            }
        }
    }

    public class Erin_Skill_2_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Erin_Skill_2_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            return true;
        }
    }
    
    public class Erin_Skill_3 : Skill
    {
        public const string Used = "已经使用过";
        static public Player SkillSource;
        static public readonly List<Player> RemovedPlayers = new List<Player>();

        static public void RestoreAllRemovePlayers(Context ctx)
        {
            for (int i = 0; i < RemovedPlayers.Count(); i++)
            {
                Player player = RemovedPlayers[i];
                if (player == null) continue;
                ctx.World.Players.Insert(i, player);
            }
            SkillSource = null;
            RemovedPlayers.Clear();
        }
        
        public Erin_Skill_3()
        {
            IsOnce = true;
            Triggers.Add(new Erin_Skill_3_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Erin_Skill_3();
        }

        public override Skill Clone(Card newcard)
        {
            return new Erin_Skill_3();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "密葬",
                Description = "限定技，你的回合结束阶段可以丢弃X张不同花色的手牌并指定X名你以外的角色，直到你的下一次回合开始阶段将其移出游戏，移回游戏时你对其造成一点伤害。"
            };
        }
    }

    public class Erin_Skill_3_Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Erin_Skill_3_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Erin_Skill_3_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.End; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        string ITriggerAsk.Message { get => "是否要发动【密葬】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }

        public override void Action(Context ctx)
        {
            List<Player> alives = ctx.World.Players.Where(_player => _player.IsAlive).ToList();
            ctx.World.RequireCard("密葬丢卡", "请丢弃任意张不同花色的卡。", skill.Owner,
                new Erin_Skill_3_DifferenceColors(alives.Count() - 1),
                true, 20,
                (cards) =>
                {
                    ctx.World.SelectPlayer("密葬选择目标", "请选择目标。", skill.Owner,
                        new FulfillNumberPlayerFilter(cards.Count(), cards.Count(), skill.Owner),
                        false, 15,
                        (targets) =>
                        {
                            SkillEvent ev_skill = new SkillEvent();
                            ev_skill.Skill = skill;
                            ev_skill.Source = skill.Owner;
                            ev_skill.Targets.Clear();
                            ev_skill.Targets.AddRange(targets);
                            skill.SetValue(Erin_Skill_3.Used, 1);
                            ctx.World.InvokeEvent(ev_skill);
                            if (ev_skill.Cancel) return;
                            Erin_Skill_3.SkillSource = skill.Owner;
                            Erin_Skill_3.RemovedPlayers.Clear();
                            foreach (Player target in ev_skill.Targets.ToArray())
                            {
                                int id = ctx.World.Players.IndexOf(target);
                                while (Erin_Skill_3.RemovedPlayers.Count() <= id)
                                    Erin_Skill_3.RemovedPlayers.Add(null);
                                Erin_Skill_3.RemovedPlayers[id] = target;
                            }
                            foreach (Player target in ev_skill.Targets.ToArray())
                                ctx.World.Players.Remove(target);
                        }, null);
                }, null);
        }
    }

    public class Erin_Skill_3_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Erin_Skill_3_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            Zone handzone = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return false;
            if (handzone.Cards.Count() == 0) return false;
            return true;
        }
    }

    public class Erin_Skill_3_DifferenceColors : CardFilter
    {
        public Erin_Skill_3_DifferenceColors(int _max)
        {
            this.max = _max;
        }

        private int max;
        public int Max { get { return this.max; } }
         
        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (want.CardColor == null) return false;
            if (selecteds.FirstOrDefault(_card => _card.CardColor?.SeemAs(want.CardColor.E) == true) != null) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() >= 1;
        }
    }

    public class Erin_Skill_3_Trigger_1 : Trigger, ITriggerInState
    {
        public Erin_Skill_3_Trigger_1()
        {
            Condition = new Erin_Skill_3_Trigger_1_Condition();
        }

        string ITriggerInState.StateKeyName { get => State.Begin; }

        int ITriggerInState.StateStep { get => 0; }

        public override void Action(Context ctx)
        {
            Erin_Skill_3.RestoreAllRemovePlayers(ctx);
        }

    }

    public class Erin_Skill_3_Trigger_1_Condition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Owner != Erin_Skill_3.SkillSource) return false;
            return true;
        }
    }

    public class Erin_Skill_3_Trigger_2 : Trigger, ITriggerInEvent
    {
        public Erin_Skill_3_Trigger_2()
        {
            Condition = new Erin_Skill_3_Trigger_2_Condition();
        }

        string ITriggerInEvent.EventKeyName { get => DieEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            Erin_Skill_3.RestoreAllRemovePlayers(ctx);
        }
    }

    public class Erin_Skill_3_Trigger_2_Condition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is DieEvent)) return false;
            DieEvent ev = (DieEvent)(ctx.Ev);
            if (ev.Source != Erin_Skill_3.SkillSource) return false;
            return true;
        }
    }
}
