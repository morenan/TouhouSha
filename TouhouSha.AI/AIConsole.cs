using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;
using TouhouSha.Core.AIs;
using TouhouSha.Core.Events;

namespace TouhouSha.AI
{
    public interface IAIConsole : IPlayerConsole
    {

    }

    public abstract class AIConsoleBase : IAIConsole
    {
        protected AIConsoleBase(Player _owner)
        {
            this.owner = _owner;
            WorthAccessment _worthacc = new WorthAccessment(owner);
            CardGausser _cardgausser = new CardGausser(owner);
            AssGausser _assgausser = new AssGausser(owner);
            _worthacc.CardGausser = _cardgausser;
            _worthacc.AssGausser = _assgausser;
            _cardgausser.WorthAcc = _worthacc;
            _assgausser.WorthAcc = _worthacc;
            SetupDataProcesser(_worthacc, _cardgausser, _assgausser);
        }

        private Player owner;
        public Player Owner 
        { 
            get { return this.owner; } 
        }

        private IWorthAccessment worthacc;
        public IWorthAccessment WorthAcc
        {
            get { return this.worthacc; }
        }

        private ICardGausser cardgausser;
        public ICardGausser CardGausser
        {
            get { return this.cardgausser; }
        }

        private IAssGausser assgausser;
        public IAssGausser AssGausser
        {
            get { return this.assgausser; }
        }
       
        public void SetupDataProcesser(IWorthAccessment _worthacc, ICardGausser _cardgausser, IAssGausser _assgausser)
        {
            this.worthacc = _worthacc;
            this.cardgausser = _cardgausser;
            this.assgausser = _assgausser;
        }

        void IPlayerConsole.MarkCards(IList<Card> cards)
        {

        }

        void IPlayerConsole.MarkCharactors(IList<Charactor> charactors)
        {

        }

        void IPlayerConsole.MarkFilters(IList<Filter> filters)
        {

        }

        void IPlayerConsole.MarkPlayers(IList<Player> players)
        {

        }

        void IPlayerConsole.MarkSkills(IList<Skill> skills)
        {

        }

        void IPlayerConsole.MarkZones(IList<Zone> zones)
        {

        }

        public abstract Event QuestEventInUseCardState(Context ctx);

        public abstract bool Ask(Context ctx, string keyname, string message, int timeout);

        public virtual void SelectCharactor(SelectCharactorBoardCore core)
        {
            core.World.Shuffle(core.Charactors);
            core.SelectedCharactor = core.Charactors.FirstOrDefault();
        }

        void IPlayerConsole.SelectCards(SelectCardBoardCore core)
        {
            Context ctx = core.World.GetContext();
            if (core.CardFilter == null) return;
            if ((core.CardFilter.GetFlag(ctx) & Enum_CardFilterFlag.ForceAll) != Enum_CardFilterFlag.None)
            {
                List<Card> selecteds = new List<Card>();
                foreach (Zone zone in owner.Zones)
                    foreach (Card card in zone.Cards)
                    {
                        Card cardconv = ctx.World.CalculateCard(ctx, card);
                        if (core.CardFilter.CanSelect(ctx, selecteds, cardconv))
                            selecteds.Add(cardconv);
                    }
                core.SelectedCards.Clear();
                core.SelectedCards.AddRange(selecteds);
                core.IsYes = true;
                return;
            }
            SelectCards(ctx, core);
        }

        protected abstract void SelectCards(Context ctx, SelectCardBoardCore core);

        void IPlayerConsole.SelectPlayers(SelectPlayerBoardCore core)
        {
            Context ctx = core.World.GetContext();
            if (core.PlayerFilter == null) return;
            if ((core.PlayerFilter.GetFlag(ctx) & Enum_PlayerFilterFlag.ForceAll) != Enum_PlayerFilterFlag.None)
            {
                List<Player> selecteds = new List<Player>();
                foreach (Player player in core.World.GetAlivePlayersStartHere(owner))
                    if (core.PlayerFilter.CanSelect(ctx, selecteds, player))
                        selecteds.Add(player);
                core.SelectedPlayers.Clear();
                core.SelectedPlayers.AddRange(selecteds);
                core.IsYes = true;
                return;
            }
            SelectPlayers(ctx, core);
        }

        protected abstract void SelectPlayers(Context ctx, SelectPlayerBoardCore core);

        void IPlayerConsole.SelectDesktop(DesktopCardBoardCore core)
        {
            Context ctx = core.World.GetContext();
            SelectDesktop(ctx, core);
        }

        void IPlayerConsole.ControlDesktop(DesktopCardBoardCore core)
        {
            Context ctx = core.World.GetContext();
            ControlDesktop(ctx, core);
        }

        void IPlayerConsole.CloseDesktop(DesktopCardBoardCore core)
        {

        }

        protected abstract void SelectDesktop(Context ctx, DesktopCardBoardCore core);

        protected abstract void ControlDesktop(Context ctx, DesktopCardBoardCore core);

        void IPlayerConsole.SelectList(ListBoardCore core)
        {
            Context ctx = core.World.GetContext();
            SelectList(ctx, core);
        }

        protected abstract void SelectList(Context ctx, ListBoardCore core);
    }


    public class AIConsole : AIConsoleBase
    {
        public class CardConvertAction
        {
            public CardConvertAction(Card _card, double _cost = 0, ISkillCardConverter _converter = null, Card _fromcard = null)
            {
                this.card = _card;
                this.fromcard = _fromcard;
                this.cost = _cost;
                this.converter = _converter;
            }

            private ISkillCardConverter converter;
            public ISkillCardConverter Converter { get { return this.converter; } }

            private Card fromcard;
            public Card FromCard { get { return this.fromcard; } set { this.fromcard = value; } }

            private double cost;
            public double Cost { get { return this.cost; } }

            private Card card;
            public Card Card { get { return this.card; } }
            
        }

        public AIConsole(Player _owner) : base(_owner)
        {

        }

        public override bool Ask(Context ctx, string keyname, string message, int timeout)
        {
            return false;
        }

        protected CardConvertAction GetMinCostConvertAction(Context ctx, ISkillCardConverter conv)
        {
            HashSet<Card> handles = new HashSet<Card>();
            List<Card> selections = new List<Card>();
            double convertworth = 0;
            while (!conv.CardFilter.Fulfill(ctx, selections))
            {
                double optworth = -10000;
                Card optcard = null;
                Card optseemas = null;
                foreach (Zone zone in Owner.Zones)
                    foreach (Card card in zone.Cards)
                    {
                        if (handles.Contains(card)) continue;
                        Card seemas = ctx.World.CalculateCard(ctx, card);
                        if (!conv.CardFilter.CanSelect(ctx, selections, seemas)) continue;
                        double worth = -WorthAcc.GetWorth(ctx, Owner, card);
                        if (worth > optworth)
                        {
                            optworth = worth;
                            optcard = card;
                            optseemas = seemas;
                        }
                    }
                if (optcard == null) break;
                if (optseemas == null) break;
                handles.Add(optcard);
                selections.Add(optseemas);
                convertworth += optworth;
            }
            if (conv.CardFilter.Fulfill(ctx, selections))
            {
                Card seemas = selections.Count() > 1
                    ? conv.CardConverter.GetCombine(ctx, selections)
                    : conv.CardConverter.GetValue(ctx, selections.FirstOrDefault());
                CardConvertAction convact = new CardConvertAction(seemas, convertworth, conv);
                return convact;
            }
            return null;
        }

        protected void UpdateMinCostConvertAction(Context ctx, ISkillCardConverter conv, Dictionary<string, CardConvertAction> dict, Card cardfrom)
        {
            CardConvertAction convact0 = null;
            CardConvertAction convact1 = GetMinCostConvertAction(ctx, conv);
            if (convact1 == null) return;
            convact1.FromCard = cardfrom;
            if (String.IsNullOrEmpty(convact1.Card?.KeyName)) return;
            if (!dict.TryGetValue(convact1.Card.KeyName, out convact0))
                dict.Add(convact1.Card.KeyName, convact0);
            else if (convact0.Cost < convact1.Cost)
                dict[convact1.Card.KeyName] = convact1;
        }

        public override Event QuestEventInUseCardState(Context ctx)
        {
            Dictionary<string, CardConvertAction> convacts = new Dictionary<string, CardConvertAction>();
            List<KeyValuePair<Card, ISkillCardConverter>> convlist = new List<KeyValuePair<Card, ISkillCardConverter>>();
            Zone handzone = null;
            Zone equipzone = null;
            foreach (Zone zone in Owner.Zones) 
                switch (zone.KeyName)
                {
                    case Zone.Hand: handzone = zone; break;
                    case Zone.Equips: equipzone = zone; break;
                }

            #region 搜集卡牌转换技能
            foreach (Skill skill in Owner.Skills)
                if (skill is ISkillCardConverter)
                    convlist.Add(new KeyValuePair<Card, ISkillCardConverter>(null, (ISkillCardConverter)skill));
            if (equipzone != null)
                foreach (Card card in equipzone.Cards)
                    foreach (Skill skill in card.Skills)
                        if (skill is ISkillCardConverter)
                            convlist.Add(new KeyValuePair<Card, ISkillCardConverter>(card, (ISkillCardConverter)skill));
            #endregion

            #region 搜集非转换出牌行为
            if (handzone != null)
                foreach (Card card in handzone.Cards)
                {
                    Card seemas = ctx.World.CalculateCard(ctx, card);
                    if (String.IsNullOrEmpty(seemas?.KeyName)) continue;
                    if (convacts.ContainsKey(seemas.KeyName)) continue;
                    convacts.Add(seemas.KeyName, new CardConvertAction(seemas));
                }
            #endregion

            #region 搜集转换出牌行为
            foreach (KeyValuePair<Card, ISkillCardConverter> kvp in convlist)
            {
                if (kvp.Value is ISkillCardMultiConverter)
                {
                    ISkillCardMultiConverter multi = (ISkillCardMultiConverter)kvp.Value;
                    foreach (string cardtype in multi.GetCardTypes(ctx))
                    {
                        multi.SetSelectedCardType(ctx, cardtype);
                        UpdateMinCostConvertAction(ctx, kvp.Value, convacts, kvp.Key);
                    }
                }
                else
                {
                    UpdateMinCostConvertAction(ctx, kvp.Value, convacts, kvp.Key);
                }
            }
            #endregion

            // 最优化的出牌行为的价值。
            double optcardworth = double.MinValue;
            // 最优化的技能发动的价值。
            double optskillworth = double.MinValue;
            // 最优化的出牌行为。
            CardConvertAction optcard = null;
            // 最优化的技能发动。
            Skill optskill = null;

            #region 搜集初动技能
            List<ISkillInitative> initskilllist = new List<ISkillInitative>();
            foreach (Skill skill in Owner.Skills)
                if (skill is ISkillInitative)
                    initskilllist.Add((ISkillInitative)skill);
            if (equipzone != null)
                foreach (Card card in equipzone.Cards)
                    foreach (Skill skill in card.Skills)
                        if (skill is ISkillInitative)
                            initskilllist.Add((ISkillInitative)skill);
            #endregion

            #region 找到最优化出牌
            foreach (CardConvertAction convact in convacts.Values)
            {
                ConditionFilter usecondition = ctx.World.TryReplaceNewCondition(convact.Card.UseCondition, null);
                if (usecondition == null) continue;
                if (!usecondition.Accept(ctx)) continue;
                CardTargetFilterAuto autoai = convact.Card.GetAutoAI();
                CardTargetFilterWorth worthai = convact.Card.GetWorthAI();
                double cardworth = -10000;
                if (autoai != null)
                {
                    autoai.AITools = new AITools(WorthAcc, CardGausser, AssGausser);
                    cardworth = autoai.GetUseWorth(ctx, convact.Card, Owner);
                }
                else if (worthai != null)
                {
                    worthai.AITools = new AITools(WorthAcc, CardGausser, AssGausser);
                    cardworth = worthai.GetUseWorth(ctx, convact.Card, Owner, 
                        ctx.World.TryReplaceNewPlayerFilter(convact.Card.TargetFilter, null));
                }
                else
                {
                    cardworth = Math.Abs(convact.Card.GetWorthForTarget());
                }
                if (cardworth + convact.Cost > optcardworth)
                {
                    optcardworth = cardworth + convact.Cost;
                    optcard = convact;
                }
            }
            #endregion

            #region 找到最优化技能
            foreach (ISkillInitative initskill in initskilllist)
            {
                Skill skill = (Skill)initskill;
                ConditionFilter usecondition = ctx.World.TryReplaceNewCondition(initskill.UseCondition, null);
                if (usecondition == null) continue;
                if (!usecondition.Accept(ctx)) continue;
                InitativeSkillAuto autoai = skill.GetAutoAI();
                InitativeSkillWorth worthai = skill.GetWorthAI();
                double skillworth = double.MinValue;
                if (autoai != null)
                {
                    autoai.AITools = new AITools(WorthAcc, CardGausser, AssGausser);
                    skillworth = autoai.GetWorth(ctx, skill, Owner);
                }
                else if (worthai != null)
                {
                    worthai.AITools = new AITools(WorthAcc, CardGausser, AssGausser);
                    skillworth = worthai.GetWorth(ctx, skill, Owner);
                }
                if (skillworth > optskillworth)
                {
                    optskillworth = skillworth;
                    optskill = skill;
                }
            }
            #endregion

            #region 最优化出牌正收益
            if (optcardworth > 0 
             && optcardworth > optskillworth)
            {
                CardTargetFilterAuto autoai = optcard.Card.GetAutoAI();
                CardTargetFilterWorth worthai = optcard.Card.GetWorthAI();
                List<Player> targets = null;
                if (autoai != null)
                {
                    autoai.AITools = new AITools(WorthAcc, CardGausser, AssGausser);
                    targets = autoai.GetSelection(ctx, optcard.Card, Owner);
                }
                else if (worthai != null)
                {
                    PlayerFilter targetfilter = ctx.World.TryReplaceNewPlayerFilter(optcard.Card.TargetFilter, null);
                    targets = new List<Player>();
                    AITools.StepOptimizeAlgorithm(ctx, targetfilter, targets, target => worthai.GetWorth(ctx, optcard.Card, Owner, targets, target));
                    if (!targetfilter.Fulfill(ctx, targets))
                        return new LeaveUseCardStateEvent(ctx.World.GetPlayerState());
                }
                if (targets == null)
                    return new LeaveUseCardStateEvent(ctx.World.GetPlayerState());
                CardEvent ev_card = new CardEvent();
                if (optcard.Converter is Skill)
                {
                    if (optcard.FromCard != null)
                    {
                        CardSkillEvent ev_skill = new CardSkillEvent();
                        ev_skill.Card = optcard.FromCard;
                        ev_skill.Skill = (Skill)(optcard.Converter);
                        ev_skill.Source = Owner;
                        ev_skill.Targets.Clear();
                        ev_card.Reason = ev_skill;
                        ctx.World.InvokeEvent(ev_skill);
                        if (ev_skill.Cancel)
                            return new LeaveUseCardStateEvent(ctx.World.GetPlayerState());
                    }
                    else
                    {
                        SkillEvent ev_skill = new SkillEvent();
                        ev_skill.Skill = (Skill)(optcard.Converter);
                        ev_skill.Source = Owner;
                        ev_skill.Targets.Clear();
                        ev_card.Reason = ev_skill;
                        ctx.World.InvokeEvent(ev_skill);
                        if (ev_skill.Cancel)
                            return new LeaveUseCardStateEvent(ctx.World.GetPlayerState());
                    }
                }
                ev_card.Card = optcard.Card;
                ev_card.Source = Owner;
                ev_card.Targets.Clear();
                ev_card.Targets.AddRange(targets);
                ctx.World.InvokeEvent(ev_card);
            }
            #endregion

            #region 最优化技能正收益
            if (optskillworth > 0
             && optskillworth > optcardworth)
            {
                ISkillInitative skillinit = (ISkillInitative)optskill;
                InitativeSkillAuto autoai = optskill.GetAutoAI();
                InitativeSkillWorth worthai = optskill.GetWorthAI();
                List<Player> targets = new List<Player>();
                List<Card> costs = new List<Card>();
                if (autoai != null)
                {
                    autoai.AITools = new AITools(WorthAcc, CardGausser, AssGausser);
                    if (!autoai.GetSelection(ctx, optskill, Owner, targets, costs))
                        return new LeaveUseCardStateEvent(ctx.World.GetPlayerState());
                }
                else if (worthai != null)
                {
                    PlayerFilter targetfilter = ctx.World.TryReplaceNewPlayerFilter(skillinit.TargetFilter, null);
                    CardFilter costfilter = ctx.World.TryReplaceNewCardFilter(skillinit.CostFilter, null);
                    AITools.StepOptimizeAlgorithm(ctx, targetfilter, targets, target => worthai.GetWorthSelect(ctx, optskill, Owner, targets, target));
                    AITools.StepOptimizeAlgorithm(ctx, Owner, costfilter, costs, cost => worthai.GetWorthSelect(ctx, optskill, Owner, costs, cost));
                    if (!targetfilter.Fulfill(ctx, targets))
                        return new LeaveUseCardStateEvent(ctx.World.GetPlayerState());
                    if (!costfilter.Fulfill(ctx, costs))
                        return new LeaveUseCardStateEvent(ctx.World.GetPlayerState());
                }
                SkillInitativeEvent ev_skill = new SkillInitativeEvent();
                ev_skill.User = Owner;
                ev_skill.Skill = optskill;
                ev_skill.Targets.Clear();
                ev_skill.Targets.AddRange(targets);
                ev_skill.Costs.Clear();
                ev_skill.Costs.AddRange(costs);
                ctx.World.InvokeEvent(ev_skill);
            }
            #endregion 

            // 结束出牌阶段
            return new LeaveUseCardStateEvent(ctx.World.GetPlayerState()); 
        }

        protected override void SelectCards(Context ctx, SelectCardBoardCore core)
        {
            CardFilter cardfilter = ctx.World.TryReplaceNewCardFilter(core.CardFilter, ctx.Ev);
            CardFilterWorth filterworth = cardfilter.GetWorthAI();
            CardFilterAuto filterauto = cardfilter.GetAutoAI();
            if (filterauto != null)
            {
                filterauto.AITools = new AITools(WorthAcc, CardGausser, AssGausser); 
                if (core.CanCancel && filterauto.GetNo(ctx, core))
                {
                    core.SelectedCards.Clear();
                    core.IsYes = false;
                    return;
                }
                core.SelectedCards.Clear();
                core.SelectedCards.AddRange(filterauto.GetSelection(ctx, core));
                core.IsYes = true;
                return;
            }
            if (filterworth != null)
            {
                filterworth.AITools = new AITools(WorthAcc, CardGausser, AssGausser);
                List<Card> selections = new List<Card>();
                double worthno = filterworth.GetWorthNo(ctx, core);
                double worthyes = AITools.StepOptimizeAlgorithm(ctx, core.Controller, cardfilter, selections, card => filterworth.GetWorth(ctx, core, selections, card));
                if (core.CanCancel && worthno > worthyes)
                {
                    core.SelectedCards.Clear();
                    core.IsYes = false;
                }
                else
                {
                    core.SelectedCards.Clear();
                    core.SelectedCards.AddRange(selections);
                    core.IsYes = true;
                }
                return;
            }
            if (core.CanCancel)
            {
                core.SelectedCards.Clear();
                core.IsYes = false;
            }
            else
            {
                List<Card> selections = new List<Card>();
                AITools.StepOptimizeAlgorithm(ctx, core.Controller, cardfilter, selections, card => 0.0d);
                core.SelectedCards.Clear();
                core.SelectedCards.AddRange(selections);
                core.IsYes = true;
            }
        }

        protected override void SelectPlayers(Context ctx, SelectPlayerBoardCore core)
        {
            PlayerFilter playerfilter = ctx.World.TryReplaceNewPlayerFilter(core.PlayerFilter, ctx.Ev);
            PlayerFilterWorth filterworth = playerfilter.GetWorthAI();
            PlayerFilterAuto filterauto = playerfilter.GetAutoAI();
            if (filterauto != null)
            {
                filterauto.AITools = new AITools(WorthAcc, CardGausser, AssGausser);
                if (core.CanCancel && filterauto.GetNo(ctx, core))
                {
                    core.SelectedPlayers.Clear();
                    core.IsYes = false;
                    return;
                }
                core.SelectedPlayers.Clear();
                core.SelectedPlayers.AddRange(filterauto.GetSelection(ctx, core));
                core.IsYes = true;
                return;
            }
            if (filterworth != null)
            {
                filterworth.AITools = new AITools(WorthAcc, CardGausser, AssGausser);
                List<Player> selections = new List<Player>();
                double worthno = filterworth.GetWorthNo(ctx, core);
                double worthyes = AITools.StepOptimizeAlgorithm(ctx, playerfilter, selections, player => filterworth.GetWorth(ctx, core, selections, player));
                if (core.CanCancel && worthno > worthyes)
                {
                    core.SelectedPlayers.Clear();
                    core.IsYes = false;
                }
                else
                {
                    core.SelectedPlayers.Clear();
                    core.SelectedPlayers.AddRange(selections);
                    core.IsYes = true;
                }
                return;
            }
            if (core.CanCancel)
            {
                core.SelectedPlayers.Clear();
                core.IsYes = false;
            }
            else
            {
                List<Player> selections = new List<Player>();
                AITools.StepOptimizeAlgorithm(ctx, playerfilter, selections, player => 0.0d);
                core.SelectedPlayers.Clear();
                core.SelectedPlayers.AddRange(selections);
                core.IsYes = true;
            }
        }

        protected override void SelectDesktop(Context ctx, DesktopCardBoardCore core)
        {
            if (core.IsAsync) return;
            ControlDesktop(ctx, core);
        }

        protected override void ControlDesktop(Context ctx, DesktopCardBoardCore core)
        {
            CardFilter cardfilter = ctx.World.TryReplaceNewCardFilter(core.CardFilter, ctx.Ev);
            DesktopCardFilterWorth filterworth = cardfilter.GetDesktopWorthAI();
            DesktopCardFilterAuto filterauto = cardfilter.GetDesktopAutoAI();
            if (filterauto != null)
            {
                filterauto.AITools = new AITools(WorthAcc, CardGausser, AssGausser);
                if ((core.Flag & Enum_DesktopCardBoardFlag.CannotNo) == Enum_DesktopCardBoardFlag.None
                 && filterauto.GetNo(ctx, core))
                {
                    core.SelectedCards.Clear();
                    core.IsYes = false;
                    return;
                }
                core.SelectedCards.Clear();
                core.SelectedCards.AddRange(filterauto.GetSelection(ctx, core));
                core.IsYes = true;
                return;
            }
            if (filterworth != null)
            {
                filterworth.AITools = new AITools(WorthAcc, CardGausser, AssGausser);
                List<Card> selections = new List<Card>();
                double worthno = filterworth.GetWorthNo(ctx, core);
                double worthyes = AITools.StepOptimizeAlgorithm(ctx, core.Controller, cardfilter, selections, card => filterworth.GetWorth(ctx, core, selections, card));
                if ((core.Flag & Enum_DesktopCardBoardFlag.CannotNo) == Enum_DesktopCardBoardFlag.None
                 && worthno > worthyes)
                {
                    core.SelectedCards.Clear();
                    core.IsYes = false;
                }
                else
                {
                    core.SelectedCards.Clear();
                    core.SelectedCards.AddRange(selections);
                    core.IsYes = true;
                }
                return;
            }
            if ((core.Flag & Enum_DesktopCardBoardFlag.CannotNo) == Enum_DesktopCardBoardFlag.None)
            {
                core.SelectedCards.Clear();
                core.IsYes = false;
            }
            else
            {
                List<Card> selections = new List<Card>();
                AITools.StepOptimizeAlgorithm(ctx, core.Controller, cardfilter, selections, card => 0.0d);
                core.SelectedCards.Clear();
                core.SelectedCards.AddRange(selections);
                core.IsYes = true;
            }
        }

        protected override void SelectList(Context ctx, ListBoardCore core)
        {
            core.SelectedItems.Clear();
            core.SelectedItems.Add(core.Items.FirstOrDefault());
            core.IsYes = true;
        }
    }
}
