using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;
using TouhouSha.Core.UIs.Events;

namespace TouhouSha.Core
{
    public class World
    {
        #region Resources
        
        public const string DistancePlus = "+距离";
        public const string DistanceMinus = "-距离";
        public const string KillRange = "攻击范围";

        #endregion

        #region Number

        private IPlayerRegister airegister;
        public IPlayerRegister AIRegister
        {
            get { return this.airegister; }
            set { this.airegister = value; }
        }

        private ObservableCollection<IPlayerRegister> playerregisters = new ObservableCollection<IPlayerRegister>();
        public ObservableCollection<IPlayerRegister> PlayerRegisters
        {
            get { return this.playerregisters; }
        }

        private ObservableCollection<Player> players = new ObservableCollection<Player>();
        public ObservableCollection<Player> Players
        {
            get { return this.players; }
        }

        private List<Zone> commonzones = new List<Zone>();
        public List<Zone> CommonZones
        {
            get { return this.commonzones; }
        }

        private List<Charactor> charactors = new List<Charactor>();
        public List<Charactor> Charactors
        {
            get { return this.charactors; }
        }

        private TriggerCollection globaltriggersbeforeplayers = new TriggerCollection();
        public TriggerCollection GlobalTriggerBeforePlayers
        {
            get { return this.globaltriggerafterplayers; }
        }

        private TriggerCollection globaltriggerafterplayers = new TriggerCollection();
        public TriggerCollection GlobalTriggerAfterPlayers
        {
            get { return this.globaltriggerafterplayers; }
        }

        private Enum_GameMode gamemode;
        public Enum_GameMode GameMode
        {
            get { return this.gamemode; }
            set { this.gamemode = value; }
        }

        private Dictionary<string, int> totalofcardnames = new Dictionary<string, int>();
        private int cardtotal;
        private int killtotal;
        private Thread thread;

        #endregion

        #region Game Start

        public void GameStart()
        {
            if (thread != null) return;
            thread = new Thread(GameStartAsync);
            thread.Start();
        }

        protected void GameStartAsync()
        {
            #region 确定各种玩家数量
            int players_count = 5;
            int leaders_count = 1;
            int slaves_count = 1;
            int avengers_count = 2;
            int spys_count = 1;
            switch (gamemode)
            {
                case Enum_GameMode.StandardPlayers5: 
                    players_count = 5;
                    leaders_count = 1;
                    slaves_count = 1;
                    avengers_count = 2;
                    spys_count = 1;
                    if (Config.GameConfig.DoubleSpy)
                    {
                        spys_count++;
                        slaves_count--;
                    }
                    break;
                case Enum_GameMode.StandardPlayers8: 
                    players_count = 8;
                    leaders_count = 1;
                    slaves_count = 2;
                    avengers_count = 4;
                    spys_count = 1;
                    if (Config.GameConfig.DoubleSpy)
                    {
                        spys_count++;
                        slaves_count--;
                    }
                    break;
                case Enum_GameMode.KOF: 
                    players_count = 2;
                    leaders_count = 0;
                    slaves_count = 1;
                    avengers_count = 1;
                    spys_count = 0;
                    break;
                case Enum_GameMode.FightLandlord: 
                    players_count = 3;
                    leaders_count = 1;
                    slaves_count = 0;
                    avengers_count = 2;
                    spys_count = 0;
                    break;
                case Enum_GameMode.Players2v2: 
                    players_count = 4;
                    leaders_count = 0;
                    slaves_count = 2;
                    avengers_count = 2;
                    spys_count = 0;
                    break;
                case Enum_GameMode.Players3v3: 
                    players_count = 6;
                    leaders_count = 0;
                    slaves_count = 3;
                    avengers_count = 3;
                    spys_count = 0;
                    break;
            }
            #endregion
            #region 生成玩家
            Players.Clear();
            for (int i = 0; i < players_count; i++)
            {
                Player player = new Player();
                IPlayerRegister register = i < playerregisters.Count() ? playerregisters[i] : null;
                player.KeyName = String.Format("Player {0}", i + 1);
                player.TrusteeshipConsole = airegister.CreateConsole(player);
                player.Console = register?.CreateConsole(player) ?? player.TrusteeshipConsole;
                player.IsTrusteeship = false;
                Zone handzone = new Zone();
                handzone.KeyName = Zone.Hand;
                handzone.Owner = player;
                player.Zones.Add(handzone);
                EquipZone equipzone = new EquipZone();
                equipzone.KeyName = Zone.Equips;
                equipzone.Owner = player;
                player.Zones.Add(equipzone);
                Zone judgezone = new Zone();
                judgezone.KeyName = Zone.Judge;
                judgezone.Owner = player;
                player.Zones.Add(judgezone);
                Players.Add(player);
            }
            #endregion 
            #region 初始化公共区域
            CommonZones.Clear();
            Zone drawzone = new Zone();
            drawzone.KeyName = Zone.Draw;
            CommonZones.Add(drawzone);
            Zone desktopzone = new Zone();
            drawzone.KeyName = Zone.Desktop;
            CommonZones.Add(desktopzone);
            Zone discardzone = new Zone();
            discardzone.KeyName = Zone.Discard;
            CommonZones.Add(discardzone);
            #endregion
            #region 加载可用角色
            Charactors.Clear();
            foreach (IPackage package in Config.GameConfig.UsedPackages)
                Charactors.AddRange(package.GetCharactors());
            #endregion
            #region 分配身份
            switch (gamemode)
            {
                // 标准局身份随机
                case Enum_GameMode.StandardPlayers5:
                case Enum_GameMode.StandardPlayers8:
                    {
                        List<PlayerAss> asslist = new List<PlayerAss>();
                        for (int i = 0; i < leaders_count; i++)
                            asslist.Add(new PlayerAss(Enum_PlayerAss.Leader));
                        for (int i = 0; i < slaves_count; i++)
                            asslist.Add(new PlayerAss(Enum_PlayerAss.Slave));
                        for (int i = 0; i < avengers_count; i++)
                            asslist.Add(new PlayerAss(Enum_PlayerAss.Avenger));
                        for (int i = 0; i < spys_count; i++)
                            asslist.Add(new PlayerAss(Enum_PlayerAss.Spy));
                        Shuffle(asslist);
                        for (int i = 0; i < players_count; i++)
                            Players[i].Ass = asslist[i];
                    }
                    break;
            }
            #endregion
            #region 主公选将
            List<Charactor> char_leaders = new List<Charactor>();
            List<Charactor> char_others = new List<Charactor>();
            Player leader = Players.FirstOrDefault(_player => _player.Ass.E == Enum_PlayerAss.Leader);
            if (leader != null)
            {
                foreach (Charactor ch in Charactors)
                {
                    if (ch.Skills.FirstOrDefault(_skill => _skill.IsLeader) != null)
                        char_leaders.Add(ch);
                    else
                        char_others.Add(ch);
                }
                Shuffle(char_others);
                List<Charactor> charlist = new List<Charactor>();
                charlist.AddRange(char_leaders);
                for (int i = char_others.Count() - Config.GameConfig.LeaderCharactorsSelectNumber; i < char_others.Count(); i++)
                    charlist.Add(char_others[i]);
                char_others.RemoveRange(char_others.Count() - Config.GameConfig.LeaderCharactorsSelectNumber, Config.GameConfig.LeaderCharactorsSelectNumber);
                SelectCharactorBoardCore core = new SelectCharactorBoardCore();
                core.World = this;
                core.Message = String.Format("你的身份是【主公】，请选择一名角色。");
                core.Controller = leader;
                core.Charactors.Clear();
                core.Charactors.AddRange(charlist);
                leader.GetCurrentConsole().SelectCharactor(core);
                leader.Charactors.Clear();
                leader.Charactors.Add(core.SelectedCharactor);
                leader.Name = core.SelectedCharactor.GetInfo().Name;
                leader.MaxHP = core.SelectedCharactor.MaxHP + 1;
                leader.HP = core.SelectedCharactor.HP + 1;
                leader.Skills.Clear();
                foreach (Skill skill in core.SelectedCharactor.Skills)
                    leader.Skills.Add(skill.Clone());
                charlist.Remove(core.SelectedCharactor);
                char_others.AddRange(charlist);
            }
            else
            {
                char_others.AddRange(char_leaders);
            }
            #endregion
            #region 其他角色选将
            Shuffle(char_others);
            List<Player> notleaders = Players.Where(_player => _player.Ass.E != Enum_PlayerAss.Leader).ToList();
            List<List<Charactor>> charlists = new List<List<Charactor>>();
            List<Charactor> char_remains = new List<Charactor>();
            for (int i = 0; i <notleaders.Count(); i++)
                charlists.Add(char_others.GetRange(i * Config.GameConfig.CharactorsSelectNumber, Config.GameConfig.CharactorsSelectNumber));
            if (notleaders.Count() * Config.GameConfig.CharactorsSelectNumber < char_others.Count())
                char_remains.AddRange(char_others.GetRange(notleaders.Count() * Config.GameConfig.CharactorsSelectNumber, 
                    char_others.Count() - notleaders.Count() * Config.GameConfig.CharactorsSelectNumber));
            ParallelLoopResult result = Parallel.For(0, notleaders.Count(), (i) =>
            {
                Player player = notleaders[i];
                List<Charactor> charlist = charlists[i];
                SelectCharactorBoardCore core = new SelectCharactorBoardCore();
                core.World = this;
                core.Message = String.Format("你的身份是【{0}】，请选择一名角色。", player.Ass);
                core.Controller = player;
                core.Charactors.Clear();
                core.Charactors.AddRange(charlist);
                player.GetCurrentConsole().SelectCharactor(core);
                player.Charactors.Clear();
                player.Charactors.Add(core.SelectedCharactor);
                player.Name = core.SelectedCharactor.GetInfo().Name;
                player.MaxHP = core.SelectedCharactor.MaxHP;
                player.HP = core.SelectedCharactor.HP;
                player.Skills.Clear();
                foreach (Skill skill in core.SelectedCharactor.Skills)
                    player.Skills.Add(skill.Clone());
                charlist.Remove(core.SelectedCharactor);
                charlists[i] = charlist;
            });
            while (!result.IsCompleted) Thread.Sleep(100);
            Charactors.Clear();
            foreach (List<Charactor> charlist in charlists)
                Charactors.AddRange(charlist);
            Charactors.AddRange(char_remains);
            #endregion
            #region 加载卡牌实例
            basecards.Clear();
            equipcards.Clear();
            spellcards.Clear();
            foreach (IPackage package in Config.GameConfig.UsedPackages)
                foreach (Card card in package.GetCardInstances())
                {
                    switch (card.CardType?.E)
                    {
                        case Enum_CardType.Base: basecards.Add(card); break;
                        case Enum_CardType.Equip: equipcards.Add(card); break;
                        case Enum_CardType.Spell: spellcards.Add(card); break;
                    }
                }
            #endregion 
            #region 加载卡堆
            foreach (IPackage package in Config.GameConfig.UsedPackages)
                drawzone.AddRange(package.GetCards());
            foreach (Card card in drawzone.Cards)
            {
                int keytotal = 0;
                cardtotal++;
                switch (card.KeyName)
                {
                    case "杀":
                    case "火杀":
                    case "雷杀":
                        killtotal++;
                        break;
                }
                if (String.IsNullOrEmpty(card.KeyName)) continue;
                if (!totalofcardnames.TryGetValue(card.KeyName, out keytotal))
                    totalofcardnames.Add(card.KeyName, 1);
                else
                    totalofcardnames[card.KeyName] = keytotal + 1;
            }
            #endregion
            #region 发牌
            Shuffle(drawzone.Cards);
            foreach (Player player in players)
                DrawCard(player, 4, null);
            #endregion
            #region 加载全局触发器
            GlobalTriggerBeforePlayers.Clear();
            GlobalTriggerAfterPlayers.Clear();
            foreach (IPackage package in Config.GameConfig.UsedPackages)
            {
                foreach (Trigger trigger in package.GetGlobalTriggersBeforePlayer())
                    GlobalTriggerBeforePlayers.Add(trigger);
                foreach (Trigger trigger in package.GetGlobalTriggersAfterPlayer())
                    GlobalTriggerAfterPlayers.Add(trigger);
            }
            #endregion
            #region 进入游戏开始阶段
            State gamestartstate = new State();
            gamestartstate.KeyName = State.GameStart;
            gamestartstate.Step = 0;
            EnterState(gamestartstate);
            #endregion
            #region 进入第一名玩家的回合开始阶段
            PhaseEvent ev_phase = new PhaseEvent();
            State state = new State();
            state.Ev = ev_phase;
            state.KeyName = State.Begin;
            state.Owner = leader;
            state.Step = 0;
            EnterState(state);
            #endregion 
        }

        #endregion 

        #region Stack System

        private List<StackHandler> stack = new List<StackHandler>();
        public List<StackHandler> Stack
        {
            get { return this.stack; }
        }

        private Dictionary<StackHandler, List<StackHandler>> stackafters = new Dictionary<StackHandler, List<StackHandler>>();
        public Dictionary<StackHandler, List<StackHandler>> StackAfters
        {
            get { return stackafters; }
        }

        private State laststate;

        public Event GetCurrentEvent()
        {
            return ((EventHandler)(Stack.LastOrDefault(_handler => _handler is EventHandler)))?.Event;
        }

        public State GetCurrentState()
        {
            return ((StateHandler)(Stack.LastOrDefault(_handler => _handler is StateHandler)))?.State;
        }

        public State GetPlayerState()
        {
            return ((StateHandler)(Stack.LastOrDefault(_handler =>
            {
                if (!(_handler is StateHandler)) return false;
                StateHandler statehandler = (StateHandler)_handler;
                switch (statehandler.State.KeyName)
                {
                    case State.Begin:
                    case State.Judge:
                    case State.Draw:
                    case State.UseCard:
                    case State.Discard:
                    case State.End:
                        return true;
                }
                return false;
            })))?.State;
        }
        
        public void InvokeEvent(Event ev)
        {
            Handle(new EventHandler(this, ev));
        }
        
        public void EnterState(State newstate)
        {
            Handle(new StateHandler(this, newstate));
        }

        public Trigger TryReplaceNewTrigger(Trigger trigger, Event reason)
        {
            Trigger newtrigger = trigger;
            UseTriggerEvent ev_trigger = new UseTriggerEvent();
            ev_trigger.Reason = reason;
            ev_trigger.Source = GetAlivePlayers().FirstOrDefault();
            ev_trigger.OldTrigger = trigger;
            ev_trigger.NewTrigger = newtrigger;
            do
            {
                InvokeEvent(ev_trigger);
                if (ev_trigger.NewTrigger == newtrigger) break;
                newtrigger = ev_trigger.NewTrigger;
            }
            while (true);
            return newtrigger;
        }

        public ConditionFilter TryReplaceNewCondition(ConditionFilter condition, Event reason)
        {
            ConditionFilter newcondition = condition;
            UseFilterEvent ev_filter = new UseFilterEvent();
            ev_filter.Reason = reason;
            ev_filter.Source = GetAlivePlayers().FirstOrDefault();
            ev_filter.OldFilter = condition;
            ev_filter.NewFilter = newcondition;
            do
            {
                InvokeEvent(ev_filter);
                if (ev_filter.NewFilter == newcondition) break;
                newcondition = ev_filter.NewFilter as ConditionFilter;
            }
            while (true);
            return newcondition;
        }

        public CardFilter TryReplaceNewCardFilter(CardFilter cardfilter, Event reason)
        {
            CardFilter newcardfilter = cardfilter;
            UseFilterEvent ev_filter = new UseFilterEvent();
            ev_filter.Reason = reason;
            ev_filter.Source = GetAlivePlayers().FirstOrDefault();
            ev_filter.OldFilter = cardfilter;
            ev_filter.NewFilter = newcardfilter;
            do
            {
                InvokeEvent(ev_filter);
                if (ev_filter.NewFilter == newcardfilter) break;
                newcardfilter = ev_filter.NewFilter as CardFilter;
            }
            while (true);
            return cardfilter;
        }

        public PlayerFilter TryReplaceNewPlayerFilter(PlayerFilter playerfilter, Event reason)
        {
            PlayerFilter newplayerfilter = playerfilter;
            UseFilterEvent ev_filter = new UseFilterEvent();
            ev_filter.Reason = reason;
            ev_filter.Source = GetAlivePlayers().FirstOrDefault();
            ev_filter.OldFilter = playerfilter;
            ev_filter.NewFilter = newplayerfilter;
            do
            {
                InvokeEvent(ev_filter);
                if (ev_filter.NewFilter == newplayerfilter) break;
                newplayerfilter = ev_filter.NewFilter as PlayerFilter;
            }
            while (true);
            return playerfilter;
        }

        public Calculator TryReplaceNewCalculator(Calculator calc, Event reason)
        {
            Calculator newcalc = calc;
            UseCalculatorEvent ev_calc = new UseCalculatorEvent();
            ev_calc.Reason = reason;
            ev_calc.Source = GetAlivePlayers().FirstOrDefault();
            ev_calc.OldCalculator = calc;
            ev_calc.NewCalculator = newcalc;
            do
            {
                InvokeEvent(ev_calc);
                if (ev_calc.NewCalculator == newcalc) break;
                newcalc = ev_calc.NewCalculator;
            }
            while (true);
            return newcalc;
        }

        public CardCalculator TryReplaceNewCardCalculator(CardCalculator cardcalc, Event reason)
        {
            CardCalculator newcardcalc = cardcalc;
            UseCardCalculatorEvent ev_cardcalc = new UseCardCalculatorEvent();
            ev_cardcalc.Reason = reason;
            ev_cardcalc.Source = GetAlivePlayers().FirstOrDefault();
            ev_cardcalc.OldCalculator = cardcalc;
            ev_cardcalc.NewCalculator = newcardcalc;
            do
            {
                InvokeEvent(ev_cardcalc);
                if (ev_cardcalc.NewCalculator == newcardcalc) break;
                newcardcalc = ev_cardcalc.NewCalculator;
            }
            while (true);
            return newcardcalc;
        }

        protected void Handle(StackHandler handler)
        {
            List<List<StackHandler>> afterstack = new List<List<StackHandler>>();
            List<int> afterindics = new List<int>();
            List<StackHandler> afters = null;
            int index = 0;
            afterstack.Add(new List<StackHandler>() { handler });
            afterindics.Add(0);
            while (afterstack.Count() > 0)
            {
                afters = afterstack.LastOrDefault();
                index = afterindics.LastOrDefault();
                if (index >= afters.Count())
                {
                    afterstack.RemoveAt(afterstack.Count() - 1);
                    afterindics.RemoveAt(afterindics.Count() - 1);
                }
                else
                {
                    afterindics[afterindics.Count() - 1]++;
                    while (afterstack.Count() > 0
                        && afterindics.LastOrDefault() >= afterstack.LastOrDefault().Count())
                    {
                        afterstack.RemoveAt(afterstack.Count() - 1);
                        afterindics.RemoveAt(afterindics.Count() - 1);
                    }
                    Stack.Add(handler);
                    if (handler is StateHandler)
                    {
                        State state = ((StateHandler)handler).State;
                        State _laststate = laststate;
                        if (_laststate != null && _laststate != state)
                        {
                            StateChangeEvent stateevent = new StateChangeEvent();
                            stateevent.OldState = _laststate;
                            stateevent.NewState = state;
                            InvokeEvent(stateevent);
                        }
                    }
                    handler.Handle();
                    if (handler is EventHandler)
                    {
                        Event ev = ((EventHandler)handler).Event;
                        if (!ev.IsStopHandle() && ev.NoticeUI)
                        {
                            UIEventFromLogical uiev = new UIEventFromLogical();
                            uiev.LogicalEvent = ev;
                            UIEvent?.Invoke(this, uiev);
                        }
                    }
                    Stack.RemoveAt(Stack.Count() - 1);
                    if (stackafters.TryGetValue(handler, out afters))
                    {
                        afters = afters.ToList();
                        stackafters.Remove(handler);
                        afterstack.Add(afters);
                        afterindics.Add(0);
                    }
                }
            }    
        }
        
        public void InvokeEventAfterState(Event newevent, State oldstate)
        {
            StackHandler oldhandler = Stack.FirstOrDefault(_handler => _handler is StateHandler && ((StateHandler)_handler).State == oldstate);
            if (oldhandler == null) return;
            InvokeEventAfterHandler(newevent, oldhandler);
        }

        public void InvokeEventAfterEvent(Event newevent, Event oldevent)
        {
            StackHandler oldhandler = Stack.FirstOrDefault(_handler => _handler is EventHandler && ((EventHandler)_handler).Event == oldevent);
            if (oldhandler == null) return;
            InvokeEventAfterHandler(newevent, oldhandler);
        }

        public void EnterStateAfterState(State newstate, State oldstate)
        {
            StackHandler oldhandler = Stack.FirstOrDefault(_handler => _handler is StateHandler && ((StateHandler)_handler).State == oldstate);
            if (oldhandler == null) return;
            EnterStateAfterHandler(newstate, oldhandler);
        }

        public void EnterStateAfterEvent(State newstate, Event oldevent)
        {
            StackHandler oldhandler = Stack.FirstOrDefault(_handler => _handler is EventHandler && ((EventHandler)_handler).Event == oldevent);
            if (oldhandler == null) return;
            EnterStateAfterHandler(newstate, oldhandler);
        }
        
        protected void EnterStateAfterHandler(State newstate, StackHandler oldhandler)
        {
            EnterHandlerAfterHandler(new StateHandler(this, newstate), oldhandler);
        }

        protected void InvokeEventAfterHandler(Event newevent, StackHandler oldhandler)
        {
            EnterHandlerAfterHandler(new EventHandler(this, newevent), oldhandler);
        }

        protected void EnterHandlerAfterHandler(StackHandler newhandler, StackHandler oldhandler)
        {
            List<StackHandler> list = null;
            if (!stackafters.TryGetValue(oldhandler, out list))
            {
                list = new List<StackHandler>();
                stackafters.Add(oldhandler, list);
            }
            list.Add(newhandler);
        }

        #endregion
        
        #region Get Infomations

        public int CalculateValue(Context ctx, ShaObject obj, string propertyname)
        {
            int oldvalue = obj.GetValue(propertyname);
            foreach (Player player in GetAlivePlayers())
                oldvalue = player.Calculators.GetValue(ctx, obj, propertyname, oldvalue);
            return oldvalue;
        }

        public Card CalculateCard(Context ctx, Card card)
        {
            Card newcard = card;
            foreach (Player player in GetAlivePlayers())
            {
                foreach (Skill skill in player.Skills)
                {
                    if (!(skill is ISkillCardConverter)) continue;
                    if (!skill.IsLocked) continue;
                    ISkillCardConverter conv = (ISkillCardConverter)skill;
                    if (conv.UseCondition?.Accept(ctx) != true) continue;
                    if (conv.CardFilter?.CanSelect(ctx, new Card[] { }, card) != true) continue;
                    newcard = conv.CardConverter?.GetValue(ctx, card) ?? card;
                    if (newcard != card) return newcard;
                }
            }
            return newcard;
        }

        public IEnumerable<Player> GetAlivePlayers()
        {
            return players.Where(_player => _player.IsAlive);
        }

        public Player GetPreviousAlivePlayer(Player here)
        {
            if (here == null) return players.FirstOrDefault(_player => _player.IsAlive);
            int i = players.IndexOf(here);
            int j = i + 1;
            for (; j != i; j++)
            {
                if (j >= players.Count()) j -= players.Count();
                if (j == i) break;
                if (players[j].IsAlive) return players[j];
            }
            if (here.IsAlive) return here;
            return null;
        }

        public Player GetNextAlivePlayer(Player here)
        {
            if (here == null) return players.LastOrDefault(_player => _player.IsAlive);
            int i = players.IndexOf(here);
            int j = i - 1;
            for (; j != i; j++)
            {
                if (j < 0) j += players.Count();
                if (j == i) break;
                if (players[j].IsAlive) return players[j];
            }
            if (here.IsAlive) return here;
            return null;
        }

        public IEnumerable<Player> GetAlivePlayersStartHere(Player here)
        {
            if (here.IsAlive) yield return here;
            int i = players.IndexOf(here);
            int j = i + 1;
            for (; j != i; j++)
            {
                if (j >= players.Count()) j -= players.Count();
                if (j == i) break;
                if (players[j].IsAlive) yield return players[j];
            }
        }

        public bool IsInDistance2Kill(Context ctx, Player source, Player target)
        {
            return IsInDistance2Kill(ctx, source, target, 0);
        }

        public bool IsInDistance2Kill(Context ctx, Player source, Player target, int rangedelta)
        {
            int i0 = players.IndexOf(source);
            int i1 = players.IndexOf(target);
            if (i0 < 0) return false;
            if (i1 < 0) return false;
            int d0 = CalculateValue(ctx, source, DistanceMinus);
            int d1 = CalculateValue(ctx, source, KillRange) + rangedelta;
            int d2 = CalculateValue(ctx, target, DistancePlus);
            int d3 = 0;
            d0 += CalculateValue(ctx, source, String.Format("{0}_{1}", target.KeyName, DistanceMinus));
            d1 += CalculateValue(ctx, source, String.Format("{0}_{1}", target.KeyName, KillRange));
            d2 += CalculateValue(ctx, target, String.Format("{0}_{1}", source.KeyName, DistancePlus));
            for (int i = i0 + 1; i != i1; i++)
            {
                if (i >= players.Count()) i -= players.Count();
                if (!players[i].IsAlive) continue;
                d3++;
            }
            if (d3 <= d0 + d1 - d2) return true;
            d3 = 0;
            for (int i = i0 - 1; i != i1; i--)
            {
                if (i < 0) i += players.Count();
                if (!players[i].IsAlive) continue;
                d3++;
            }
            if (d3 <= d0 + d1 - d2) return true;
            return false;
        }

        public bool IsInDistance(Context ctx, Player source, Player target)
        {
            int i0 = players.IndexOf(source);
            int i1 = players.IndexOf(target);
            if (i0 < 0) return false;
            if (i1 < 0) return false;
            int d0 = CalculateValue(ctx, source, DistanceMinus);
            int d1 = 0;
            int d2 = CalculateValue(ctx, target, DistancePlus);
            int d3 = 0;
            d0 += CalculateValue(ctx, source, String.Format("{0}_{1}", target.KeyName, DistanceMinus));
            //d1 += CalculateValue(ctx, source, String.Format("{0}_{1}", target.KeyName, KillRange));
            d2 += CalculateValue(ctx, target, String.Format("{0}_{1}", source.KeyName, DistancePlus));
            for (int i = i0 + 1; i != i1; i++)
            {
                if (i >= players.Count()) i -= players.Count();
                if (!players[i].IsAlive) continue;
                d3++;
            }
            if (d3 <= d0 + d1 - d2) return true;
            d3 = 0;
            for (int i = i0 - 1; i != i1; i--)
            {
                if (i < 0) i += players.Count();
                if (!players[i].IsAlive) continue;
                d3++;
            }
            if (d3 <= d0 + d1 - d2) return true;
            return false;
        }

        public Context GetContext()
        {
            return new Context(this, GetCurrentEvent());
        }

        public double GetCardProbably(string cardkeyname)
        {
            int n0 = cardtotal;
            int n1 = 0;
            if (cardkeyname.Equals("Kill")) n1 = killtotal;
            else if (!totalofcardnames.TryGetValue(cardkeyname, out n1)) return 0.0d;
            if (n0 == 0) return 0.0d;
            return (double)n1 / n0;
        }

        public double GetColorProbably(Enum_CardColor cardcolor)
        {
            switch (cardcolor)
            {
                case Enum_CardColor.None:
                    return 0.0d;
                case Enum_CardColor.Red:
                case Enum_CardColor.Black:
                    return 0.5d;
                default:
                    return 0.25d;
            }
        }

        public double GetPointNotLessProperty(int cardpoint)
        {
            if (cardpoint < 1 || cardpoint > 13) return 0.0d;
            return (14 - cardpoint) / 13.0d;
        }

        #endregion

        #region Cards Library

        private List<Card> basecards = new List<Card>();

        public IEnumerable<string> GetBaseCardKeyNames()
        {
            return basecards.Select(_card => _card.KeyName);
        }
        
        private List<Card> spellcards = new List<Card>();
        
        public IEnumerable<string> GetSpellCardKeyNames()
        {
            return spellcards.Select(_card => _card.KeyName);
        }

        private List<Card> equipcards = new List<Card>();

        public IEnumerable<string> GetEquipCardKeyNames()
        {
            return equipcards.Select(_card => _card.KeyName);
        }

        public Card GetCardInstance(string keyname)
        {
            Card card = basecards.FirstOrDefault(_card => _card.KeyName?.Equals(keyname) == true);
            if (card != null) return card;
            card = spellcards.FirstOrDefault(_card => _card.KeyName?.Equals(keyname) == true);
            if (card != null) return card;
            card = equipcards.FirstOrDefault(_card => _card.KeyName?.Equals(keyname) == true);
            if (card != null) return card;
            return null;
        }

        #endregion

        #region User Interface
        
        public Event QuestEventInUseCardState(Player owner)
        {
            Context ctx = GetContext();
            IPlayerConsole console = owner.GetCurrentConsole();
            return console.QuestEventInUseCardState(ctx);
        }

        public SelectCardBoardCore RequireCard(string keyname, string message, Player player, CardFilter filter, bool cancancel, int timeout, Action<IEnumerable<Card>> yes, Action no)
        {
            Context ctx = GetContext();
            SelectCardBoardCore core = new SelectCardBoardCore();
            core.KeyName = keyname;
            core.World = this;
            core.Message = message;
            core.Controller = player;
            core.CardFilter = filter;
            core.CanCancel = cancancel;
            core.Timeout = timeout;
            if (!player.IsAlive) return null;
            IPlayerConsole console = player.GetCurrentConsole();
            console.SelectCards(core);
            if (core.IsYes)
                yes?.Invoke(core.SelectedCards);
            else
                no?.Invoke();
            return core;
        }
       
        public ParallelSelectCardBoardCore RequireCardParallel(string keyname, string message, IEnumerable<Player> players, 
            CardFilter filter, bool cancancel, int timeout, Action<IEnumerable<Card>> yes, Action no)
        {
            ParallelSelectCardBoardCore core = new ParallelSelectCardBoardCore();
            core.KeyName = keyname;
            core.World = this;
            core.Message = message;
            core.Controllers.Clear();
            core.Controllers.AddRange(players.Where(_player => _player.IsAlive));
            core.CanCancel = cancancel;
            core.Timeout = timeout;
            if (core.Controllers.Count() == 0) return core;
            ParallelSelectCardBoardOpen?.Invoke(this, new ParallelSelectCardBoardOpenEventArgs(core));
            if (core.IsYes)
                yes?.Invoke(core.SelectedCards);
            else
                no?.Invoke();
            return core;
        }

        public SelectPlayerBoardCore SelectPlayer(string keyname, string message, Player controller, PlayerFilter filter, bool cancancel, int timeout, Action<IEnumerable<Player>> yes, Action no)
        {
            SelectPlayerBoardCore core = new SelectPlayerBoardCore();
            core.KeyName = keyname;
            core.World = this;
            core.Message = message;
            core.Controller = controller;
            core.PlayerFilter = filter;
            core.CanCancel = cancancel;
            core.Timeout = timeout;
            if (!controller.IsAlive) return core;
            IPlayerConsole console = controller.GetCurrentConsole();
            console.SelectPlayers(core);
            if (core.IsYes)
                yes?.Invoke(core.SelectedPlayers);
            else
                no?.Invoke();
            return core;
        }

        public DesktopCardBoardCore ShowDesktop(Player player, DesktopCardBoardCore core, IList<IList<Card>> cards, 
            bool move2desktopzone, Event movereason)
        {
            if (!player.IsAlive) return core;
            for (int i = 0; i < cards.Count(); i++)
            {
                IList<Card> sublist = cards[i];
                DesktopCardBoardZone zone = core.Zones[i];
                if (move2desktopzone)
                {
                    core.Flag |= Enum_DesktopCardBoardFlag.CardInBoard;
                    for (int j = 0; j < sublist.Count(); j++)
                        MoveCard(player, sublist[j], zone, movereason);
                }
                else
                {
                    core.Flag &= ~Enum_DesktopCardBoardFlag.CardInBoard;
                    for (int j = 0; j < sublist.Count(); j++)
                        zone.Cards.Add(sublist[j]);
                }
            }
            if (core.IsAsync)
            {
                foreach (Player alive in GetAlivePlayersStartHere(player))
                {
                    IPlayerConsole console = alive.GetCurrentConsole();
                    console.SelectDesktop(core);
                }
            }
            else
            {
                IPlayerConsole console = player.GetCurrentConsole();
                console.SelectDesktop(core);
            }
            return core;
        }

        public void ControlDesktop(Player controller, DesktopCardBoardCore core)
        {
            if (!controller.IsAlive) return;
            IPlayerConsole console = controller.GetCurrentConsole();
            console.SelectDesktop(core);
        }

        public void CloseDesktop(DesktopCardBoardCore core)
        {
            foreach (Player alive in GetAlivePlayers())
            {
                IPlayerConsole console = alive.GetCurrentConsole();
                console.SelectDesktop(core);
            }
        }

        public ListBoardCore ShowList(string keyname, string message, Player controller, IEnumerable<object> items, int selectmax, bool cancancel, int timeout, Action<IEnumerable<object>> yes, Action no)
        {
            ListBoardCore core = new ListBoardCore();
            core.KeyName = keyname;
            core.Message = message;
            core.Controller = controller;
            core.World = this;
            core.Items.Clear();
            core.Items.AddRange(items);
            core.CanCancel = cancancel;
            core.Timeout = timeout;
            core.SelectMax = selectmax;
            if (!controller.IsAlive) return core;
            IPlayerConsole console = controller.GetCurrentConsole();
            console.SelectList(core);
            if (core.IsYes)
                yes?.Invoke(core.SelectedItems);
            else
                no?.Invoke();
            return core;
        }
        
        #endregion

        #region Operations
        
        public void Shuffle(IList list)
        {
            Random random = new Random();
            for (int i = 0; i < list.Count; i++)
            {
                int i1 = random.Next() % list.Count;
                object temp = list[i];
                list[i] = list[i1];
                list[i1] = temp;
            }
        }

        public void RecycleDiscards()
        {
            Zone drawzone = CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Draw) == true);
            if (drawzone == null) return;
            Zone discardzone = CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == false);
            if (discardzone == null) return;
            drawzone.AddRange(discardzone.Cards.ToArray());
            drawzone.Shuffle();
        }

        public bool MakeSureDrawCount(int drawcount)
        {
            Zone drawzone = CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Draw) == true);
            if (drawzone == null) return false;
            if (drawzone.Cards.Count() >= drawcount) return false;
            RecycleDiscards();
            return drawzone.Cards.Count() >= drawcount;
        }

        public List<Card> GetDrawTops(int drawcount)
        {
            Zone drawzone = CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Draw) == true);
            if (drawzone == null) return new List<Card>();
            if (!MakeSureDrawCount(drawcount)) return new List<Card>();
            List<Card> cards = new List<Card>();
            for (int i = drawzone.Cards.Count() - drawcount; i < drawzone.Cards.Count(); i++)
                cards.Add(drawzone.Cards[i]);
            return cards;
        }

        public void MoveCard(Player source, Card card, Zone newzone, Event reason, Enum_MoveCardFlag flag = Enum_MoveCardFlag.None)
        {
            if (newzone.Owner != null && !newzone.Owner.IsAlive) return;
            if (reason?.IsStopHandle() == true) return;
            Zone oldzone = card.Zone;
            MoveCardEvent ev = new MoveCardEvent(new List<Card> { card }, source, oldzone, newzone);
            ev.Reason = reason;
            ev.Flag = flag;
            if ((oldzone.Flag & Enum_ZoneFlag.CardUnvisibled) == Enum_ZoneFlag.None
             || (newzone.Flag & Enum_ZoneFlag.CardUnvisibled) == Enum_ZoneFlag.None)
                ev.Flag |= Enum_MoveCardFlag.FaceUp;
            InvokeEvent(ev);
            if (ev.Cancel) return;
            if (ev.NewZone == null) return;
            ev.NewZone.AddRange(ev.MovedCards, flag);
            MoveCardDoneEvent ev_done = new MoveCardDoneEvent(new List<Card> { card }, source, oldzone, newzone);
            ev_done.Reason = ev.Reason;
            ev_done.Flag = ev.Flag;
            InvokeEvent(ev_done);
        }

        public void MoveCards(Player source, IEnumerable<Card> cards, Zone newzone, Event reason, Enum_MoveCardFlag flag = Enum_MoveCardFlag.None)
        {
            if (newzone.Owner != null && !newzone.Owner.IsAlive) return;
            if (reason?.IsStopHandle() == true) return;
            Dictionary<Zone, List<Card>> zonedict = new Dictionary<Zone, List<Card>>();
            foreach (Card card in cards)
            {
                Zone oldzone = card.Zone;
                List<Card> list = null;
                if (oldzone == null) continue;
                if (!zonedict.TryGetValue(oldzone, out list))
                {
                    list = new List<Card>();
                    zonedict.Add(oldzone, list);
                }
                list.AddRange(card.GetInitialCards());
            }
            List<MoveCardEvent> evs = new List<MoveCardEvent>();
            foreach (KeyValuePair<Zone, List<Card>> kvp in zonedict)
            {
                MoveCardEvent ev = new MoveCardEvent(kvp.Value, source, kvp.Key, newzone);
                ev.Reason = reason;
                ev.Flag = flag;
                if ((ev.OldZone.Flag & Enum_ZoneFlag.CardUnvisibled) == Enum_ZoneFlag.None
                 || (ev.NewZone.Flag & Enum_ZoneFlag.CardUnvisibled) == Enum_ZoneFlag.None)
                    ev.Flag |= Enum_MoveCardFlag.FaceUp;
                InvokeEvent(ev);
                if (ev.Cancel) continue;
                if (ev.NewZone == null) continue;
                evs.Add(ev);
                ev.NewZone.AddRange(ev.MovedCards, flag);
            }
            if (evs.Count() == 1)
            {
                MoveCardEvent ev = evs.FirstOrDefault();
                MoveCardDoneEvent ev_done = new MoveCardDoneEvent(ev.MovedCards, source, ev.OldZone, ev.NewZone);
                ev_done.Reason = ev.Reason;
                ev_done.Flag = ev.Flag;
                InvokeEvent(ev_done);
            }
            else if (evs.Count() > 1)
            {
                UIEventGroup uieg = new UIEventGroup();
                foreach (MoveCardEvent ev in evs)
                {
                    UIEventFromLogical evfl = new UIEventFromLogical();
                    evfl.LogicalEvent = ev;
                    uieg.Items.Add(evfl);
                }
                UIEvent?.Invoke(this, uieg);
                foreach (MoveCardEvent ev in evs)
                {
                    MoveCardDoneEvent ev_done = new MoveCardDoneEvent(ev.MovedCards, source, ev.OldZone, ev.NewZone);
                    ev_done.Reason = ev.Reason;
                    ev_done.Flag = ev.Flag;
                    ev_done.NoticeUI = false;
                    InvokeEvent(ev_done);
                }
            }
        }
        
        public void UseCard(Player source, Player target, Card card, Event reason)
        {
            if (!source.IsAlive) return;
            if (!target.IsAlive) return;
            if (reason?.IsStopHandle() == true) return;
            Zone desktopzone = CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Desktop) == true);
            if (desktopzone == null) return;
            MoveCard(source, card, desktopzone, reason);
            CardEvent ev = new CardEvent();
            ev.Reason = reason;
            ev.Card = card;
            ev.Source = source;
            ev.Targets.Clear();
            ev.Targets.Add(target);
            InvokeEvent(ev);
        }
        
        public void UseCard(Player source, IEnumerable<Player> targets, Card card, Event reason)
        {
            if (!source.IsAlive) return;
            if (targets.FirstOrDefault(_target => _target.IsAlive) == null) return;
            if (reason?.IsStopHandle() == true) return;
            Zone desktopzone = CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Desktop) == true);
            if (desktopzone == null) return;
            MoveCard(source, card, desktopzone, reason);
            CardEvent ev = new CardEvent();
            ev.Reason = reason;
            ev.Card = card;
            ev.Source = source;
            ev.Targets.Clear();
            ev.Targets.AddRange(targets.Where(_target => _target.IsAlive));
            InvokeEvent(ev);
        }

        public void DrawCard(Player source, int number, Event reason)
        {
            if (!source.IsAlive) return;
            if (reason?.IsStopHandle() == true) return;
            Zone drawzone = CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Draw) == true);
            if (drawzone == null) return;
            Zone handzone = source.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return;
            if (!MakeSureDrawCount(number)) return;
            List<Card> cards = GetDrawTops(number);
            MoveCards(source, cards, handzone, reason);
        }

        public bool CanDiscardCardFulfillNumber(Player source, int number, Event reason, bool allow_equiped)
        {
            if (!source.IsAlive) return false;
            if (reason?.IsStopHandle() == true) return false;
            int candiscards = 0;
            Zone discardzone = CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
            if (discardzone == null) return false;
            Zone handzone = source.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone != null) candiscards += handzone.Cards.Count();
            Zone equipzone = source.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Equips) == true);
            if (equipzone != null && allow_equiped) candiscards += equipzone.Cards.Count();
            return candiscards >= number;
        }
       
        public int DiscardCard(Player source, int number, Event reason, bool allow_equiped)
        {
            if (!source.IsAlive) return 0;
            if (reason?.IsStopHandle() == true) return 0;
            int result = 0;
            int candiscards = 0;
            Zone discardzone = CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
            if (discardzone == null) return 0;
            Zone handzone = source.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone != null) candiscards += handzone.Cards.Count();
            Zone equipzone = source.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Equips) == true);
            if (equipzone != null && allow_equiped) candiscards += equipzone.Cards.Count();
            if (candiscards <= number)
            {
                List<Card> cards = new List<Card>();
                if (handzone != null) cards.AddRange(handzone.Cards);
                if (equipzone != null && allow_equiped) cards.AddRange(equipzone.Cards);
                if (cards.Count() == 0) return 0;
                MoveCards(source, cards, discardzone, reason);
                return cards.Count();
            }
            RequireCard(reason?.KeyName ?? String.Empty,
                String.Format("请丢弃{0}张卡。", number),
                source,
                new FulfillNumberCardFilter(number, number)
                {
                    Allow_Equiped = allow_equiped,
                    Allow_Judging = false,
                },
                false, 15,
                (cards) => { if (cards.Count() == 0) return; MoveCards(source, cards, discardzone, reason); result += cards.Count(); },
                () => { });
            return result;
        }
        
        public void DiscardCardCanCancel(Player source, int number, Event reason, bool allow_equiped, Action<IEnumerable<Card>> yes, Action no)
        {
            if (!source.IsAlive) return;
            if (reason?.IsStopHandle() == true) return;
            int candiscards = 0;
            Zone discardzone = CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
            if (discardzone == null) return;
            Zone handzone = source.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone != null) candiscards += handzone.Cards.Count();
            Zone equipzone = source.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Equips) == true);
            if (equipzone != null && allow_equiped) candiscards += equipzone.Cards.Count();
            if (candiscards <= number)
            {
                List<Card> cards = new List<Card>();
                if (handzone != null) cards.AddRange(handzone.Cards);
                if (equipzone != null && allow_equiped) cards.AddRange(equipzone.Cards);
                MoveCards(source, cards, discardzone, reason);
                yes?.Invoke(cards);
                return;
            }
            RequireCard(reason?.KeyName ?? String.Empty,
                String.Format("请丢弃{0}张卡。", number),
                source,
                new FulfillNumberCardFilter(number, number)
                {
                    Allow_Equiped = allow_equiped,
                    Allow_Judging = false,
                },
                true, 15,
                (cards) => { MoveCards(source, cards, discardzone, reason); yes?.Invoke(cards); },
                no);
        }
        
        public void SelectTargetCard(Player source, IEnumerable<Zone> zones, int number, Action<IEnumerable<Card>> yes, Action no)
        {
            if (!source.IsAlive) return;
            if (zones == null || zones.Count() == 0)
            {
                no?.Invoke();
                return;
            }
            int candiscards = zones.Sum(_zone => _zone.Cards.Count());
            if (candiscards == 0)
            {
                no?.Invoke();
                return;
            }
            if (candiscards <= number)
            {
                List<Card> cards = new List<Card>();
                foreach (Zone zone in zones) cards.AddRange(zone.Cards);
                yes?.Invoke(cards);
                return;
            }
            DesktopCardBoardCore desktop_core = new DesktopCardBoardCore();
            List<IList<Card>> cardalls = new List<IList<Card>>();
            FulfillNumberCardFilter cardfilter = new FulfillNumberCardFilter(number, number);
            cardfilter.Allow_Hand = false;
            cardfilter.Allow_Equiped = false;
            cardfilter.Allow_Judging = false;
            cardfilter.Allow_OtherZones.Clear();
            foreach (Zone zone in zones)
            {
                switch (zone.KeyName)
                {
                    case Zone.Hand: cardfilter.Allow_Hand = true; break;
                    case Zone.Equips: cardfilter.Allow_Equiped = true; break;
                    case Zone.Judge: cardfilter.Allow_Judging = true; break;
                    default: cardfilter.Allow_OtherZones.Add(zone); break;
                }
            }
            desktop_core.Controller = source;
            desktop_core.IsAsync = false;
            desktop_core.CardFilter = cardfilter;
            desktop_core.Flag = Enum_DesktopCardBoardFlag.None;
            if (number == 1)
                desktop_core.Flag |= Enum_DesktopCardBoardFlag.SelectCardAndYes;
            desktop_core.Flag |= Enum_DesktopCardBoardFlag.CannotNo;
            foreach (Zone zone in zones)
            {
                DesktopCardBoardZone desktop_zone = new DesktopCardBoardZone(desktop_core);
                desktop_zone.KeyName = zone.KeyName;
                if ((zone.Flag & Enum_ZoneFlag.CardUnvisibled) != Enum_ZoneFlag.None)
                    desktop_zone.Flag |= Enum_DesktopZoneFlag.FaceDown;
                desktop_core.Zones.Add(desktop_zone);
                cardalls.Add(zone.Cards.ToList());
            }
            ShowDesktop(source, desktop_core, cardalls, false, null);
            if (desktop_core.IsYes)
                yes?.Invoke(desktop_core.SelectedCards);
            else
                no?.Invoke();

        }

        public void SelectTargetCard(Player source, Player target, int number, bool allow_equiped, bool allow_judging, Action<IEnumerable<Card>> yes, Action no)
        {
            if (!source.IsAlive) return;
            if (!target.IsAlive) return;
            List<Zone> zones = new List<Zone>();
            Zone handzone = target.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone != null) zones.Add(handzone);
            Zone equipzone = target.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Equips) == true);
            if (equipzone != null && allow_equiped) zones.Add(equipzone);
            Zone judgezone = target.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
            if (judgezone != null && allow_judging) zones.Add(judgezone);
            SelectTargetCard(source, zones, number, yes, no);
        }

        public int StealTargetCard(Player source, Player target, int number, Event reason, bool allow_equiped, bool allow_judging)
        {
            if (!source.IsAlive) return 0;
            if (!target.IsAlive) return 0;
            if (reason?.IsStopHandle() == true) return 0;
            int result = 0;
            Zone handzone = source.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return 0;
            SelectTargetCard(source, target, number, allow_equiped, allow_judging,
                (cards) => { if (cards.Count() == 0) return; MoveCards(target, cards, handzone, reason); result += cards.Count(); }, null);
            return result;
        }

        public int DiscardTargetCard(Player source, Player target, int number, Event reason, bool allow_equiped, bool allow_judging)
        {
            if (!source.IsAlive) return 0;
            if (!target.IsAlive) return 0;
            if (reason?.IsStopHandle() == true) return 0;
            int result = 0;
            Zone discardzone = CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
            if (discardzone == null) return 0;
            SelectTargetCard(source, target, number, allow_equiped, allow_judging,
                (cards) => { if (cards.Count() == 0) return; MoveCards(target, cards, discardzone, reason); result += cards.Count(); }, null);
            return result;
        }

        public DamageEvent GetDamageEvent(Player source, Player target, int damagevalue, string damagetype, Event ev)
        {
            DamageEvent damageevent = new DamageEvent();
            damageevent.Source = source;
            damageevent.Target = target;
            damageevent.Reason = ev;
            damageevent.DamageType = damagetype;
            damageevent.DamageValue = damagevalue;
            return damageevent;
        }
        
        public void Damage(Player source, Player target, int damagevalue, string damagetype, Event ev)
        {
            if (!source.IsAlive) return;
            if (!target.IsAlive) return;
            if (ev?.IsStopHandle() == true) return;
            InvokeEvent(GetDamageEvent(source, target, damagevalue, damagetype, ev));
        }

        public HealEvent GetHealEvent(Player source, Player target, int healvalue, string healtype, Event ev)
        {
            HealEvent healevent = new HealEvent();
            healevent.Source = source;
            healevent.Target = target;
            healevent.Reason = ev;
            healevent.HealType = healtype;
            healevent.HealValue = healvalue;
            return healevent;
        }

        public void Heal(Player source, Player target, int healvalue, string healtype, Event ev)
        {
            if (!source.IsAlive) return;
            if (!target.IsAlive) return;
            if (ev?.IsStopHandle() == true) return;
            InvokeEvent(GetHealEvent(source, target, healvalue, healtype, ev));
        }

        public void ChangeMaxHp(Player source, int delta, Event reason)
        {
            if (!source.IsAlive) return;
            if (reason?.IsStopHandle() == true) return;
            PropChangeEvent ev = new PropChangeEvent();
            ev.Source = source;
            ev.PropertyName = "MaxHP";
            ev.OldValue = source.MaxHP;
            source.MaxHP += delta;
            ev.NewValue = source.MaxHP;
            InvokeEvent(ev);
        }

        public void Die(Player player, Event ev)
        {
            if (!player.IsAlive) return;
            if (ev?.IsStopHandle() == true) return;
            player.IsAlive = false;
            DieEvent dieevent = new DieEvent();
            dieevent.Reason = ev;
            dieevent.Source = player;
            InvokeEvent(dieevent);
        }

        public void FaceClip(Player source, Player target, Event ev)
        {
            if (!source.IsAlive) return;
            if (!target.IsAlive) return;
            if (ev?.IsStopHandle() == true) return;
            FaceClipEvent clipev = new FaceClipEvent();
            clipev.Reason = ev;
            clipev.Source = source;
            clipev.Target = target;
            InvokeEvent(clipev);
            if (clipev.Cancel) return;
            target.SetValue("IsFacedDown", target.IsFacedDown ? 0 : 1, clipev);
        }

        public void ShowHand(Player target)
        {
            if (!target.IsAlive) return;
        }

        public void ShowHand(Player target, IEnumerable<Card> cards)
        {
            if (!target.IsAlive) return;
        }

        public void HideHand(Player target)
        {
            if (!target.IsAlive) return;
        }

        public bool Ask(Player asked, string keyname, string message)
        {
            if (!asked.IsAlive) return false;
            Context ctx = GetContext();
            IPlayerConsole console = asked.GetCurrentConsole();
            return console.Ask(ctx, keyname, message, Config.GameConfig.Timeout_Ask);
        }

        #endregion

        #region Event Handler

        public event ParallelSelectCardBoardOpenEventHandler ParallelSelectCardBoardOpen;

        public event UIEventHandler UIEvent;

        #endregion
    }
    
    public enum Enum_GameMode
    {
        StandardPlayers5,
        StandardPlayers8,
        KOF,
        FightLandlord,
        Players2v2,
        Players3v3,
    }
}
