using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.AIs;
using TouhouSha.Koishi.Cards;

namespace TouhouSha.AI
{
    public class AssGausser : IAssGausser
    {
        public AssGausser(Player _owner)
        {
            this.owner = _owner;
        }

        /// <summary>
        /// 当前玩家。
        /// </summary>
        private Player owner;
        /// <summary>
        /// 当前玩家。
        /// </summary>
        public Player Owner { get { return this.owner; } }

        /// <summary>
        /// 收益评估器。
        /// </summary>
        private IWorthAccessment worthacc;
        /// <summary>
        /// 收益评估器。
        /// </summary>
        public IWorthAccessment WorthAcc
        {
            get { return this.worthacc; }
            set { this.worthacc = value; }
        }

        /// <summary>
        /// 贡献统计
        /// </summary>
        private Dictionary<Player, PlayerContribution> contributions
            = new Dictionary<Player, PlayerContribution>();

        /// <summary>
        /// 上一个回合的玩家.
        /// </summary>
        private Player lastphaseplayer;
        /// <summary>
        /// 上一个回合是否是乐的。
        /// </summary>
        private bool islastphasehappied;

        public PlayerContribution GetContribution(Player player)
        {
            PlayerContribution result = null;
            if (contributions.TryGetValue(player, out result)) return result;
            result = new PlayerContribution(player);
            contributions.Add(player, result);
            return result;
        }

        public List<PlayerContribution> GetContributionList(Context ctx, 
            out int slavescount, out int avengerscount, out int spiescount,
            out int deterslavescount, out int deteravengercount)
        {
            List<Player> alives = ctx.World.Players.Where(_player => _player.IsAlive).ToList();
            List<PlayerContribution> list = alives.Where(_player => _player.Ass?.E != Enum_PlayerAss.Leader)
                .Select(_player => GetContribution(_player)).ToList();
            slavescount = alives.Count(_player => _player.Ass?.E == Enum_PlayerAss.Slave);
            avengerscount = alives.Count(_player => _player.Ass?.E == Enum_PlayerAss.Avenger);
            spiescount = alives.Count(_player => _player.Ass?.E == Enum_PlayerAss.Spy);
            list.Sort((c0, c1) => (c0.LeaderContribution - c0.AvengerContribution).CompareTo(c1.LeaderContribution - c1.AvengerContribution));
            deterslavescount = Math.Min(slavescount, list.Count(c => c.LeaderContribution - c.AvengerContribution > 1));
            deteravengercount = Math.Min(avengerscount, list.Count(c => c.LeaderContribution - c.AvengerContribution < 1));
            return list;
        }

        public double ProbablyOfSlave(Context ctx, Player player)
        {
            if (player.Ass?.E == Enum_PlayerAss.Leader) return 1.0d;
            int slavescount = 0;
            int avengerscount = 0;
            int spiescount = 0;
            int deterslavescount = 0;
            int deteravengerscount = 0;
            List<PlayerContribution> list = GetContributionList(ctx, 
                out slavescount, out avengerscount, out spiescount, 
                out deterslavescount, out deteravengerscount);
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].Owner != player) continue;
                if (i >= list.Count() - deterslavescount) return 1.0d;
                if (i < deteravengerscount) return 0.0d;
                int n0 = slavescount - deterslavescount;
                int n1 = n0 + avengerscount - deteravengerscount;
                n0 -= spiescount;
                if (n1 == 0) return 0.0d;
                return (double)n0 / n1;
            }
            return 0.0d;
        }

        public double ProbablyOfAvenger(Context ctx, Player player)
        {
            if (player.Ass?.E == Enum_PlayerAss.Leader) return 0.0d;
            int slavescount = 0;
            int avengerscount = 0;
            int spiescount = 0;
            int deterslavescount = 0;
            int deteravengerscount = 0;
            List<PlayerContribution> list = GetContributionList(ctx,
                out slavescount, out avengerscount, out spiescount,
                out deterslavescount, out deteravengerscount);
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].Owner != player) continue;
                if (i >= list.Count() - deterslavescount) return 0.0d;
                if (i < deteravengerscount) return 1.0d;
                int n0 = avengerscount - deteravengerscount;
                int n1 = n0 + slavescount - deterslavescount;
                n0 -= spiescount;
                if (n1 == 0) return 0.0d;
                return (double)n0 / n1;
            }
            return 0.0d;
        }

        public double ProbablyOfSpy(Context ctx, Player player)
        {
            if (player.Ass?.E == Enum_PlayerAss.Leader) return 0.0d;
            int slavescount = 0;
            int avengerscount = 0;
            int spiescount = 0;
            int deterslavescount = 0;
            int deteravengerscount = 0;
            List<PlayerContribution> list = GetContributionList(ctx,
                out slavescount, out avengerscount, out spiescount,
                out deterslavescount, out deteravengerscount);
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].Owner != player) continue;
                if (i >= list.Count() - deterslavescount) return 0.0d;
                if (i < deteravengerscount) return 0.0d;
                int n0 = spiescount;
                int n1 = avengerscount + slavescount - deteravengerscount - deterslavescount;
                if (n1 == 0) return 0.0d;
                return (double)n0 / n1;
            }
            return 0.0d;
        }

        void IAssGausser.LetKnow(Context ctx, Event ev)
        {
            if (ev is DamageEvent)
                LetKnow(ctx, (DamageEvent)ev);
            if (ev is HealEvent)
                LetKnow(ctx, (HealEvent)ev);
            if (ev is MoveCardDoneEvent)
                LetKnow(ctx, (MoveCardDoneEvent)ev);
            if (ev is PhaseEvent)
                LetKnow(ctx, (PhaseEvent)ev);
            if (ev is GiveExtraPhaseEvent)
                LetKnow(ctx, (GiveExtraPhaseEvent)ev);
        }

        public void LetKnow(Context ctx, HealEvent ev)
        {
            PlayerContribution con = GetContribution(ev.Source);
            double p0 = ProbablyOfSlave(ctx, ev.Target);
            double p1 = ProbablyOfAvenger(ctx, ev.Target);
            con.LeaderContribution += p0 * WorthAcc.GetWorthPerHp(ctx, ev.Target, ev.Target) * ev.HealValue;
            con.AvengerContribution += p1 * WorthAcc.GetWorthPerHp(ctx, ev.Target, ev.Target) * ev.HealValue;
        }

        public void LetKnow(Context ctx, DamageEvent ev)
        {
            PlayerContribution con = GetContribution(ev.Source);
            double p0 = ProbablyOfSlave(ctx, ev.Target);
            double p1 = ProbablyOfAvenger(ctx, ev.Target);
            con.LeaderContribution -= p0 * WorthAcc.GetWorthPerHp(ctx, ev.Target, ev.Target) * ev.DamageValue;
            con.AvengerContribution -= p1 * WorthAcc.GetWorthPerHp(ctx, ev.Target, ev.Target) * ev.DamageValue;
        }

        public void LetKnow(Context ctx, MoveCardDoneEvent ev)
        {
            Player source = ev.Controller;
            Event reason = ev.Reason;
            while (reason != null)
            {
                if (reason is SkillEvent)
                {
                    SkillEvent er = (SkillEvent)(reason);
                    source = er.Source;
                    break;
                }
                if (reason is CardSkillEvent)
                {
                    CardSkillEvent er = (CardSkillEvent)(reason);
                    source = er.Source;
                    break;
                }
                if (reason is CardEvent)
                {
                    CardEvent er = (CardEvent)(reason);
                    source = er.Source;
                    break;
                }
                reason = reason.Reason;
            }
            if (source == null) return;
            PlayerContribution con = GetContribution(source);
            if (ev.OldZone?.Owner != null && ev.OldZone.Owner != source)
            {
                double p0 = ProbablyOfSlave(ctx, ev.OldZone.Owner);
                double p1 = ProbablyOfAvenger(ctx, ev.OldZone.Owner);
                foreach (Card card in ev.MovedCards)
                {
                    Zone oldzone = card.Zone;
                    card.Zone = ev.OldZone;
                    con.LeaderContribution -= p0 * WorthAcc.GetWorth(ctx, ev.OldZone.Owner, card);
                    con.AvengerContribution -= p1 * WorthAcc.GetWorth(ctx, ev.OldZone.Owner, card);
                    card.Zone = oldzone;
                }
            }
            if (ev.NewZone?.Owner != null && ev.NewZone.Owner != source)
            {
                double p0 = ProbablyOfSlave(ctx, ev.NewZone.Owner);
                double p1 = ProbablyOfAvenger(ctx, ev.NewZone.Owner);
                foreach (Card card in ev.MovedCards)
                {
                    Zone oldzone = card.Zone;
                    card.Zone = ev.NewZone;
                    con.LeaderContribution += p0 * WorthAcc.GetWorth(ctx, ev.NewZone.Owner, card);
                    con.AvengerContribution += p1 * WorthAcc.GetWorth(ctx, ev.NewZone.Owner, card);
                    card.Zone = oldzone;
                }
            }
        }

        public void LetKnow(Context ctx, PhaseEvent ev)
        {
            AccessLastPhasePlayer(ctx);
            RecordThisPhasePlayer(ctx, ev.StartState?.Owner);
        }

        public void LetKnow(Context ctx, GiveExtraPhaseEvent ev)
        {
            AccessLastPhasePlayer(ctx);
            RecordThisPhasePlayer(ctx, ev.StartState?.Owner);
        }

        /// <summary>
        /// 估计上一个回合的玩家。
        /// </summary>
        protected void AccessLastPhasePlayer(Context ctx)
        {
            Player player = lastphaseplayer;
            // 上一个回合的玩家不存在。
            if (lastphaseplayer == null) return;
            // 如果这个玩家：
            // 1. 没有被贴乐，否则默认认为不能作任何有价值的活动。
            if (islastphasehappied) return;
            // 2. 手牌数量小于2，认为没有能力造成收益。
            Zone hand = player.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (hand.Cards.Count() < 2) return;
            // 2. 旁边两格只能确定一个阵营，并且没有对任何阵营造成显著收益，那么就视作是同伙。
            #region 同伙判定
            // 自己无需猜测，明身份的玩家无需猜测。
            if (player != Owner && !player.IsAssVisibled)
            {
                Player previous = ctx.World.GetPreviousAlivePlayer(player);
                Player next = ctx.World.GetNextAlivePlayer(player);
                double previous_prob_slaves = ProbablyOfSlave(ctx, previous);
                double previous_prob_avenger = ProbablyOfAvenger(ctx, previous);
                double next_prob_slaves = ProbablyOfSlave(ctx, next);
                double next_prob_avenger = ProbablyOfAvenger(ctx, next);
                bool previous_almost_slaves = previous_prob_slaves - previous_prob_avenger >= 0.6;
                bool previous_almost_avenger = previous_prob_avenger - previous_prob_slaves >= 0.6;
                bool next_almost_slaves = next_prob_slaves - next_prob_avenger >= 0.6;
                bool next_almost_avenger = next_prob_avenger - next_prob_slaves >= 0.6;
                PlayerContribution contribution = GetContribution(player);
                if (Math.Abs(contribution.LeaderContribution) <= 1 || Math.Abs(contribution.AvengerContribution) <= 1)
                {
                    // 确定的是主忠阵营。
                    if (previous_almost_slaves || next_almost_slaves)
                        if (!previous_almost_avenger && !next_almost_avenger)
                            contribution.LeaderContribution += hand.Cards.Count() - 1;
                    // 确定的是反贼阵营。
                    if (previous_almost_avenger || next_almost_avenger)
                        if (!previous_almost_slaves && !next_almost_slaves)
                            contribution.AvengerContribution += hand.Cards.Count() - 1;
                }
            }
            #endregion
            // 3. 旁边两格正好是反阵营的玩家，并且没有对任何阵营造成显著收益，那么就视作是内奸。
        }

        /// <summary>
        /// 记录当前回合的玩家。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="player"></param>
        protected void RecordThisPhasePlayer(Context ctx, Player player)
        {
            Zone judgezone = player?.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
            lastphaseplayer = player;
            islastphasehappied = judgezone?.Cards?.FirstOrDefault(_card => _card.KeyName?.Equals(HappyCard.Normal) == true) != null;
        }
    }

    public class PlayerContribution
    {
        public PlayerContribution(Player _owner)
        {
            this.owner = _owner;
        }

        private Player owner;
        public Player Owner
        {
            get { return this.owner; }
        }

        private double leadercontribution;
        public double LeaderContribution
        {
            get { return this.leadercontribution; }
            set { this.leadercontribution = value; }
        }

        private double avengercontribution;
        public double AvengerContribution
        {
            get { return this.avengercontribution; }
            set { this.avengercontribution = value; }
        }

    }
}
