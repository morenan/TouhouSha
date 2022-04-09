using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.AIs;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Cards;

namespace TouhouSha.Koishi.Triggers
{
    public class NegateQuestTrigger : Trigger, ITriggerInState
    {
        public const string DefaultKeyName = "非延时锦囊牌询问无懈可击";

        public NegateQuestTrigger()
        {
            KeyName = DefaultKeyName;
            Condition = new NegateQuestCondition();
        }

        string ITriggerInState.StateKeyName => State.Handle;

        int ITriggerInState.StateStep => StateChangeEvent.Step_Start - 2;

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (!(state.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(state.Ev);
            List<Player> cannegates = new List<Player>();
            foreach (Player player in ctx.World.GetAlivePlayers())
            {
                Zone handzone = player.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                if (handzone != null && handzone.Cards.FirstOrDefault(_card => _card.KeyName?.Equals(NegateCard.Normal) == true) != null)
                {
                    cannegates.Add(player);
                    continue;
                }
                foreach (Skill skill in player.Skills)
                {
                    if (skill is ISkillCardMultiConverter2)
                    {
                        ISkillCardMultiConverter2 conv = (ISkillCardMultiConverter2)skill;
                        if (!conv.UseCondition.Accept(ctx)) continue;
                        conv.SetEnabledCardTypes(ctx, new string[] { NegateCard.Normal });
                        bool cannegate = conv.EnabledCardTypes != null && conv.EnabledCardTypes.Contains(NegateCard.Normal);
                        conv.CancelEnabledCardTypes(ctx);
                        if (cannegate)
                        {
                            cannegates.Add(player);
                            break;
                        }
                    }
                    else if (skill is ISkillCardConverter)
                    {
                        ISkillCardConverter conv = (ISkillCardConverter)skill;
                        if (!conv.UseCondition.Accept(ctx)) continue;
                        foreach (Zone zone in player.Zones)
                            foreach (Card card in zone.Cards)
                            {
                                Card newcard = conv.CardConverter.GetValue(ctx, card);
                                if (newcard.KeyName?.Equals(NegateCard.Normal) == true)
                                {
                                    cannegates.Add(player);
                                    break;
                                }
                            }
                    }
                }
            }
            if (cannegates.Count() == 0) return;
            ctx.World.RequireCardParallel(
                NegateCard.Normal,
                String.Format("{0}对{1}打出了{2}，可以打出一张无懈可击取消之。",
                    ev.Source.Name, state.Owner.Name, ev.Card.GetInfo().Name),
                cannegates,
                new TargetCardFilter(1, 1, NegateCard.Normal),
                true,
                Config.GameConfig.Timeout_Handle,
                (cards) =>
                {
                    CardEvent ev_card = new CardEvent();
                    ev_card.Card = cards.FirstOrDefault();
                    ev_card.Reason = ev;
                    ev_card.Source = ev.Source;
                    ev_card.Targets.Clear();
                    ev_card.Targets.Add(state.Owner);
                    ctx.World.InvokeEvent(ev_card);
                    if (!ev_card.Cancel)
                    {
                        state.SetValue(NegateCard.HasBeenNegated, 1);
                        if (ev.Card.KeyName?.Equals(NegateCard.Normal) == true) ev.Cancel = true;
                        state.Step = StateChangeEvent.Step_AfterEnd - 1;
                    }
                },
                () =>
                {

                });
        }
    }
    
    public class NegateQuestCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.GetValue(NegateCard.HasBeenNegated) == 1) return false;
            if (state.Ev is CardEvent)
            {
                CardEvent ev = (CardEvent)(state.Ev);
                if (ev.Card.CardType?.SubType?.E != Enum_CardSubType.Immediate) return false;
                return true;
            }
            if (state.Ev is JudgeEvent
             && state.Ev.Reason is DelaySpellEvent)
                return true;
            return false;
        }

    }

    public class NegateQuestCardFilter : CardFilter, ICardFilterRequiredCardTypes
    {
        public NegateQuestCardFilter(int _mincount, int _maxcount)
        {
            this.mincount = _maxcount;
            this.maxcount = _maxcount;
        }

        private int mincount = 1;
        private int maxcount = 1;

        IEnumerable<string> ICardFilterRequiredCardTypes.RequiredCardTypes
        {
            get { return new string[] { NegateCard.Normal }; }
        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() + 1 > maxcount) return false;
            if (want.Zone?.KeyName?.Equals(Zone.Hand) != true) return false;
            if (want.KeyName?.Equals(NegateCard.Normal) != true) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return (selecteds.Count() >= mincount);
        }

        public override CardFilterWorth GetWorthAI()
        {
            return new NegateQuestCardWorthAI();
        }
    }

    public class NegateQuestCardWorthAI : CardFilterWorth
    {
        public override double GetWorthNo(Context ctx, SelectCardBoardCore core)
        {
            State state = ctx.World.GetCurrentState();
            if (state?.Ev is CardEvent)
            {
                CardEvent ev = (CardEvent)(state.Ev);
                if (ev.Targets.Count() == 0) return 0.0d;
                return ev.Targets.Sum(_target => -AITools.WorthAcc.GetWorthUse(ctx, core.Controller, ev.Card, _target, new Player[] { }));
            }
            if (state?.Ev is JudgeEvent
             && state.Ev.Reason is DelaySpellEvent)
            {
                DelaySpellEvent ev = (DelaySpellEvent)state.Ev.Reason;
                return -AITools.WorthAcc.GetWorth(ctx, core.Controller, ev.Card);
            }
            return 0.0d;
        }

        public override double GetWorth(Context ctx, SelectCardBoardCore core, IEnumerable<Card> selecteds, Card want)
        {
            return -AITools.WorthAcc.GetWorth(ctx, want.Owner, want);
        }
    }
}
