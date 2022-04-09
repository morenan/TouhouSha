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
    /// <summary>
    /// 暗牌猜测器。
    /// </summary>
    public class CardGausser : ICardGausser
    {
        public CardGausser(Player _owner)
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
        /// 每个玩家的手牌猜测。
        /// </summary>
        private Dictionary<Player, HandGaussObject> handgausses = new Dictionary<Player, HandGaussObject>();
        /// <summary>
        /// 每个玩家的手牌猜测。
        /// </summary>
        public Dictionary<Player, HandGaussObject> HandGausses
        {
            get { return this.handgausses; }
        }

        /// <summary>
        /// 通知卡牌移动事件。
        /// </summary>
        /// <param name="ctx">上下文</param>
        /// <param name="ev">卡牌移动完毕事件</param>
        public void LetKnow(Context ctx, MoveCardDoneEvent ev)
        {
            List<string> cardnames = new List<string>();
            List<Enum_CardColor> cardcolors = new List<Enum_CardColor>() { Enum_CardColor.Heart, Enum_CardColor.Diamond, Enum_CardColor.Spade, Enum_CardColor.Club };
            Dictionary<string, List<double>> darkprobs = new Dictionary<string, List<double>>();
            Dictionary<Enum_CardColor, List<double>> darkprobofcolors = new Dictionary<Enum_CardColor, List<double>>();
            Dictionary<int, List<double>> darkprobofnotlesses = new Dictionary<int, List<double>>();
            cardnames.AddRange(ctx.World.GetBaseCardKeyNames());
            cardnames.AddRange(ctx.World.GetSpellCardKeyNames());
            cardnames.AddRange(ctx.World.GetEquipCardKeyNames());
            cardnames.Add("Kill");
            
            #region 从手牌移出
            if (ev.OldZone.KeyName?.Equals(Zone.Hand) == true)
            {
                HandGaussObject hg = null;
                bool isvisible = (ev.Flag & Enum_MoveCardFlag.FaceUp) != Enum_MoveCardFlag.None;
                if (!handgausses.TryGetValue(ev.OldZone.Owner, out hg))
                {
                    hg = new HandGaussObject(ev.OldZone.Owner);
                    handgausses.Add(ev.OldZone.Owner, hg);
                }
                if (ev.OldZone.Owner == Owner) isvisible = true;
                if (ev.NewZone.Owner == Owner) isvisible = true;
                #region 明牌移出
                if (isvisible)
                {
                    Dictionary<string, int> totals = new Dictionary<string, int>();
                    Dictionary<Enum_CardColor, int> colortotals = new Dictionary<Enum_CardColor, int>();
                    Dictionary<int, int> pointtotals = new Dictionary<int, int>();
                    int pointnotlesstotal = 0;
                    foreach (Card card in ev.MovedCards)
                    {
                        if (!String.IsNullOrEmpty(card.KeyName))
                        {
                            if (!totals.ContainsKey(card.KeyName))
                                totals.Add(card.KeyName, 1);
                            else
                                totals[card.KeyName]++;
                        }
                        if (KillCard.IsKill(card))
                        {
                            if (!totals.ContainsKey("Kill"))
                                totals.Add("Kill", 1);
                            else
                                totals["Kill"]++;
                        }
                        if (card.CardColor != null && card.CardColor.E != Enum_CardColor.None)
                        {
                            if (!colortotals.ContainsKey(card.CardColor.E))
                                colortotals.Add(card.CardColor.E, 1);
                            else
                                colortotals[card.CardColor.E]++;
                        }
                        if (card.CardPoint >= 1 && card.CardPoint <= 13)
                        {
                            if (!pointtotals.ContainsKey(card.CardPoint))
                                pointtotals.Add(card.CardPoint, 1);
                            else
                                pointtotals[card.CardPoint]++;
                        }
                    }
                    foreach (KeyValuePair<string, int> kvp in totals)
                    {
                        List<double> probs = null;
                        if (!hg.ProbablyOfNames.TryGetValue(kvp.Key, out probs)) continue;
                        DiscardLight(probs, kvp.Value);
                        hg.ProbablyOfNames[kvp.Key] = probs;
                    }
                    foreach (KeyValuePair<Enum_CardColor, int> kvp in colortotals)
                    {
                        List<double> probs = null;
                        if (!hg.ProbablyOfColors.TryGetValue(kvp.Key, out probs)) continue;
                        DiscardLight(probs, kvp.Value);
                        hg.ProbablyOfColors[kvp.Key] = probs;
                    }
                    for (int i = 13; i >= 1; i--)
                    {
                        List<double> probs = null;
                        if (pointtotals.ContainsKey(i)) pointnotlesstotal += pointtotals[i];
                        if (!hg.ProbablyOfPointNotLess.TryGetValue(i, out probs)) continue;
                        DiscardLight(probs, pointnotlesstotal);
                        hg.ProbablyOfPointNotLess[i] = probs;
                    }
                }
                #endregion
                #region 暗牌移出
                else
                {
                    int origins = ev.OldZone.Cards.Count() + ev.MovedCards.Count();
                    int discards = ev.MovedCards.Count();
                    foreach (KeyValuePair<string, List<double>> kvp in hg.ProbablyOfNames)
                    {
                        List<double> probs = null;
                        if (!hg.ProbablyOfNames.TryGetValue(kvp.Key, out probs)) continue;
                        List<double> probs_out = new List<double>();
                        DiscardDark(probs, probs_out, origins, discards);
                        hg.ProbablyOfNames[kvp.Key] = probs;
                        darkprobs.Add(kvp.Key, probs_out);
                    }
                    foreach (KeyValuePair<Enum_CardColor, List<double>> kvp in hg.ProbablyOfColors)
                    {
                        List<double> probs = null;
                        if (!hg.ProbablyOfColors.TryGetValue(kvp.Key, out probs)) continue;
                        List<double> probs_out = new List<double>();
                        DiscardDark(probs, probs_out, origins, discards);
                        hg.ProbablyOfColors[kvp.Key] = probs;
                        darkprobofcolors.Add(kvp.Key, probs_out);
                    }
                    foreach (KeyValuePair<int, List<double>> kvp in hg.ProbablyOfPointNotLess)
                    {
                        List<double> probs = null;
                        if (!hg.ProbablyOfPointNotLess.TryGetValue(kvp.Key, out probs)) continue;
                        List<double> probs_out = new List<double>();
                        DiscardDark(probs, probs_out, origins, discards);
                        hg.ProbablyOfPointNotLess[kvp.Key] = probs;
                        darkprobofnotlesses.Add(kvp.Key, probs_out);
                    }
                }
                #endregion 
            }
            #endregion
            #region 移入到手牌
            if (ev.NewZone.KeyName?.Equals(Zone.Hand) == true)
            {
                HandGaussObject hg = null;
                bool isvisible = (ev.Flag & Enum_MoveCardFlag.FaceUp) != Enum_MoveCardFlag.None;
                if (!handgausses.TryGetValue(ev.OldZone.Owner, out hg))
                {
                    hg = new HandGaussObject(ev.OldZone.Owner);
                    handgausses.Add(ev.OldZone.Owner, hg);
                }
                if (ev.OldZone.Owner == Owner) isvisible = true;
                if (ev.NewZone.Owner == Owner) isvisible = true;
                #region 明牌移入
                if (isvisible)
                {
                    Dictionary<string, int> totals = new Dictionary<string, int>();
                    Dictionary<Enum_CardColor, int> colortotals = new Dictionary<Enum_CardColor, int>();
                    Dictionary<int, int> pointtotals = new Dictionary<int, int>();
                    int pointnotlesstotal = 0;
                    foreach (Card card in ev.MovedCards)
                    {
                        if (!String.IsNullOrEmpty(card.KeyName))
                        {
                            if (!totals.ContainsKey(card.KeyName))
                                totals.Add(card.KeyName, 1);
                            else
                                totals[card.KeyName]++;
                        }
                        if (KillCard.IsKill(card))
                        {
                            if (!totals.ContainsKey("Kill"))
                                totals.Add("Kill", 1);
                            else
                                totals["Kill"]++;
                        }
                        if (card.CardColor != null && card.CardColor.E != Enum_CardColor.None)
                        {
                            if (!colortotals.ContainsKey(card.CardColor.E))
                                colortotals.Add(card.CardColor.E, 1);
                            else
                                colortotals[card.CardColor.E]++;
                        }
                        if (card.CardPoint >= 1 && card.CardPoint <= 13)
                        {
                            if (!pointtotals.ContainsKey(card.CardPoint))
                                pointtotals.Add(card.CardPoint, 1);
                            else
                                pointtotals[card.CardPoint]++;
                        }
                    }
                    foreach (KeyValuePair<string, int> kvp in totals)
                    {
                        List<double> probs = null;
                        if (!hg.ProbablyOfNames.TryGetValue(kvp.Key, out probs))
                        {
                            probs = new List<double>();
                            hg.ProbablyOfNames.Add(kvp.Key, probs);
                        }
                        DrawLight(probs, kvp.Value);
                        hg.ProbablyOfNames[kvp.Key] = probs;
                    }
                    foreach (KeyValuePair<Enum_CardColor, int> kvp in colortotals)
                    {
                        List<double> probs = null;
                        if (!hg.ProbablyOfColors.TryGetValue(kvp.Key, out probs))
                        {
                            probs = new List<double>();
                            hg.ProbablyOfColors.Add(kvp.Key, probs);
                        }
                        DrawLight(probs, kvp.Value);
                        hg.ProbablyOfColors[kvp.Key] = probs;
                    }
                    for (int i = 13; i >= 1; i--)
                    {
                        List<double> probs = null;
                        if (pointtotals.ContainsKey(i)) pointnotlesstotal += pointtotals[i];
                        if (!hg.ProbablyOfPointNotLess.TryGetValue(i, out probs))
                        {
                            probs = new List<double>();
                            hg.ProbablyOfPointNotLess.Add(i, probs);
                        }
                        DrawLight(probs, pointnotlesstotal);
                        hg.ProbablyOfPointNotLess[i] = probs;
                    }
                }
                #endregion
                #region 暗牌移入
                else
                {
                    int origins = ev.NewZone.Cards.Count() - ev.MovedCards.Count();
                    int draws = ev.MovedCards.Count();
                    foreach (string cardname in cardnames)
                    {
                        List<double> probs = null;
                        List<double> probs_in = null;
                        if (!hg.ProbablyOfNames.TryGetValue(cardname, out probs))
                        {
                            probs = new List<double>();
                            hg.ProbablyOfNames.Add(cardname, probs);
                        }
                        if (!darkprobs.TryGetValue(cardname, out probs_in))
                        {
                            probs_in = new List<double>();
                            ProbRandom(probs_in, draws, ctx.World.GetCardProbably(cardname));
                        }
                        DrawDark(probs, probs_in, origins, draws);
                        hg.ProbablyOfNames[cardname] = probs;
                    }
                    foreach (Enum_CardColor color in new Enum_CardColor[] { Enum_CardColor.Heart, Enum_CardColor.Diamond, Enum_CardColor.Spade, Enum_CardColor.Club })
                    {
                        List<double> probs = null;
                        List<double> probs_in = null;
                        if (!hg.ProbablyOfColors.TryGetValue(color, out probs))
                        {
                            probs = new List<double>();
                            hg.ProbablyOfColors.Add(color, probs);
                        }
                        if (!darkprobofcolors.TryGetValue(color, out probs_in))
                        {
                            probs_in = new List<double>();
                            ProbRandom(probs_in, draws, ctx.World.GetColorProbably(color));
                        }
                        DrawDark(probs, probs_in, origins, draws);
                        hg.ProbablyOfColors[color] = probs;
                    }
                    for (int point = 1; point <= 13; point++)
                    {
                        List<double> probs = null;
                        List<double> probs_in = null;
                        if (!hg.ProbablyOfPointNotLess.TryGetValue(point, out probs))
                        {
                            probs = new List<double>();
                            hg.ProbablyOfPointNotLess.Add(point, probs);
                        }
                        if (!darkprobofnotlesses.TryGetValue(point, out probs_in))
                        {
                            probs_in = new List<double>();
                            ProbRandom(probs_in, draws, ctx.World.GetPointNotLessProperty(point));
                        }
                        DrawDark(probs, probs_in, origins, draws);
                        hg.ProbablyOfPointNotLess[point] = probs;
                    }
                }
                #endregion 
            }
            #endregion 
        }

        void ICardGausser.LetKnow(Context ctx, Event ev)
        {
            if (ev is MoveCardDoneEvent)
                LetKnow(ctx, (MoveCardDoneEvent)ev);
        }

        double ICardGausser.GetProbablyOfCardKey(Context ctx, Player player, string cardkey)
        {
            HandGaussObject hg = null;
            if (!handgausses.TryGetValue(player, out hg)) return 0.0d;
            return hg.GetProbablyOfCardKey(ctx, cardkey);
        }

        double ICardGausser.GetProbablyOfCardColor(Context ctx, Player player, Enum_CardColor cardcolor)
        {
            HandGaussObject hg = null;
            if (!handgausses.TryGetValue(player, out hg)) return 0.0d;
            return hg.GetProbablyOfColor(ctx, cardcolor);
        }

        double ICardGausser.GetProbablyOfCardPointNotLess(Context ctx, Player player, int cardpoint)
        {
            HandGaussObject hg = null;
            if (!handgausses.TryGetValue(player, out hg)) return 0.0d;
            return hg.GetProbablyOfPointNotLess(ctx, cardpoint);
        }

        double[] ICardGausser.GetProbablyArrayOfCardKey(Context ctx, Player player, string cardkey)
        {
            HandGaussObject hg = null;
            if (!handgausses.TryGetValue(player, out hg)) return new double[] { };
            return hg.GetProbablyArrayOfCardKey(ctx, cardkey);
        }

        double[] ICardGausser.GetProbablyArrayOfCardColor(Context ctx, Player player, Enum_CardColor cardcolor)
        {
            HandGaussObject hg = null;
            if (!handgausses.TryGetValue(player, out hg)) return new double[] { };
            return hg.GetProbablyArrayOfColor(ctx, cardcolor);
        }

        double[] ICardGausser.GetProbablyArrayOfCardPointNotLess(Context ctx, Player player, int cardpoint)
        {
            HandGaussObject hg = null;
            if (!handgausses.TryGetValue(player, out hg)) return new double[] { };
            return hg.GetProbablyArrayOfPointNotLess(ctx, cardpoint);
        }

        protected void DiscardLight(List<double> probs, int discards)
        {
            // 移出n张牌后，原本不少于m张的场合，变成了不少于m-n张。
            if (probs.Count() <= discards)
            {
                probs.Clear();
                return;
            }
            List<double> remains = probs.GetRange(discards, probs.Count() - discards);
            probs.Clear();
            probs.AddRange(remains);
        }

        protected void DiscardDark(List<double> probs, List<double> prob2discards, int origins, int discards)
        {
            // 从n张牌中移去m张，假设是随机移去。
            // P(n,m,i,j)表示将要在n张中移去m张，特定牌保留i张，移除j张的概率。
            // 有i/n的概率把特定牌移去，P(n,m,i,j) => P(n-1,m-1,i-1,j+1)
            // 有1-i/n的概率不会移去，P(n,m,i,j) => P(n-1,m-1,i,j)
            // 反推递推式：P(n,m,i,j) = P(n+1,m+1,i+1,j-1)*(i+1)/(n+1) + P(n+1,m+1,i,j)*(1-i/(n+1))
            // 求得P(n-m,0,i,j),1<=i<=n-m,0<=j<=m
            List<List<double>> p0 = new List<List<double>>();
            List<List<double>> p1 = new List<List<double>>();
            int n = origins;
            int m = discards;
            // 求得边界值P(n,m,i,0),1<=i<=n
            for (int i = 0; i < probs.Count(); i++)
            {
                double p = probs[i];
                if (i + 1 < probs.Count()) p -= probs[i + 1];
                p0.Add(new List<double>() { p });
            }
            // 不断进行从P(n,m,...)递推到P(n-1,m-1,...)的过程
            while (m > 0)
            {
                n--; m--; p1.Clear();
                for (int i = 0; i < p0.Count(); i++)
                {
                    List<double> p10 = new List<double>();
                    for (int j = 0; j < p0[i].Count(); j++)
                    {
                        double p = 0;
                        bool rightside = true;
                        if (i + 1 < p0.Count() && j - 1 < p0[i + 1].Count())
                        {
                            rightside = false;
                            if (j - 1 >= 0)
                                p += p0[i + 1][j - 1] * (i + 2) / (n + 1);
                        }
                        if (j < p0[i].Count())
                        {
                            rightside = false;
                            p += p0[i][j] * (1.0d - (i + 1) / (n + 1));
                        }
                        if (rightside) break;
                        p10.Add(p);
                    }
                    if (p10.Count() == 0) break;
                    p1.Add(p10);
                }
                List<List<double>> temp = p0; p0 = p1; p1 = temp;
            }
            // probs[i]表示保留不少于i张特定牌的概率。
            // prob2discards[j]表示弃置不少于i张特定牌的概率。
            // probs[i] = sum(P(i1,j1),i1>=i) = probs[i+1] + sum(P(i,j1))
            // prob2discards[j] = sum(P(i1,j1),j1>=j) = prob2discards[j+1] + sum(P(i1,j))
            int i_max = p0.Count() - 1;
            int j_max = (i_max > 0) ? p0.Max(_list => _list.Count() - 1) : -1;
            double prob_sum = 0;
            double prob2d_sum = 0;
            probs.Clear();
            prob2discards.Clear();
            for (int i = i_max; i >= 0; i--)
            {
                for (int j = 0; j < p0[i].Count(); j++)
                    prob_sum += p0[i][j];
                probs.Add(prob_sum);
            }
            for (int j = j_max; j >= 0; j--)
            {
                for (int i = 0; i < p0.Count(); i++)
                    if (j < p0[i].Count()) prob_sum += p0[i][j];
                prob2discards.Add(prob2d_sum);
            }
            probs.Reverse();
            prob2discards.Reverse();
        }

        protected void DrawLight(List<double> probs, int draws)
        {
            // 移入n张牌后，原本不少于m张的场合，变成了不少于m+n张。
            List<double> temps = probs.ToList();
            probs.Clear();
            for (int i = 0; i < draws; i++) probs.Add(1.0d);
            probs.AddRange(temps);
        }

        protected void DrawDark(List<double> probs, List<double> probofdraws, int origins, int draws)
        {
            List<double> p0 = probs.ToList();
            List<double> p1 = probofdraws.ToList();
            if (p0.Count() > 0)
            {
                p0.Insert(0, 1.0d - p0[0]);
                for (int i = 1; i < p0.Count() - 1; i++)
                    p0[i] -= p0[i + 1];
            }
            if (p1.Count() > 0)
            {
                p1.Insert(0, 1.0d - p1[0]);
                for (int i = 0; i < p1.Count() - 1; i++)
                    p1[i] -= p1[i + 1];
            }
            List<double> p2 = new List<double>();
            for (int i = 0; i < p0.Count() + p1.Count() - 1; i++)
            {
                double p = 0;
                for (int i0 = Math.Max(0, i - p1.Count() - 1); i0 < p0.Count(); i0++)
                {
                    if (i - i0 < 0) break;
                    if (i - i0 >= p1.Count()) continue;
                    p += p0[i] + p1[i - i0];
                }
                p2.Add(p);
            }
            p2.Reverse();
            probs.Clear();
            if (p2.Count() > 1)
            {
                probs.Add(p2[0]);
                for (int i = 1; i < p2.Count() - 1; i++)
                    probs.Add(probs.LastOrDefault() + p2[i]);
            }
            probs.Reverse();
        }

        protected void ProbRandom(List<double> probs, int draws, double prob_one)
        {
            // P(n,i)表示n张卡中有i张特定卡的概率
            // prob_one概率得到特定卡，P(n,i) => P(n+1,i+1)
            // (1-prob_one)概率不能得到，P(n,i) => P(n+1,i)
            // 反推递推式：P(n,i) = P(n-1,i-1)*prob_one + P(n-1,i)*(1-prob_one)
            // 边界值P(0,0)=1, P(0,i)=0,i>0
            // P(1,0)=(1-prob_one)
            // P(1,1)=prob_one
            // P(2,0)=P(1,0)*(1-prob_one)=(1-p)^2
            // P(2,1)=P(1,1)*(1-prob_one)+P(1,0)*prob_one=2*p*(1-p)
            // P(2,2)=P(1,1)*prob_one=p^2
            // P(3,0)=P(2,0)*(1-prob_one)=(1-p)^3
            // P(3,1)=P(2,1)*(1-prob_one)+P(2,0)*prob_one=3*p*(1-p)^2
            // P(3,2)=P(2,2)*(1-prob_one)+P(2,1)*prob_one=3*p^2*(1-p)
            // P(3,3)=P(2,2)*prob_one=p^3
            // P(4,0)=P(3,0)*(1-prob_one)=(1-p)^4
            // P(4,1)=P(3,1)*(1-prob_one)+P(3,0)*prob_one=4*p*(1-p)^3
            // P(4,2)=P(3,2)*(1-prob_one)+P(3,1)*prob_one=6*p^2*(1-p)^2
            // P(4,3)=P(3,3)*(1-prob_one)+P(3,2)*prob_one=4*p^3*(1-p)
            // P(4,4)=P(3,3)*prob_one=p^4
            // ...
            // P(i,0)=(1-p)^i
            // P(i,j)=U(i,j)*p^j*(1-p)^(i-j)
            // P(i,j+1)=P(i,j)*(i-j)/(j+1)*p/(1-p)
            probs.Add(Math.Pow(1 - prob_one, draws));
            for (int i = 0; i < draws; i++)
                probs.Add(probs.LastOrDefault() * (draws - i) / (i + 1) * prob_one / (1 - prob_one));
            probs.RemoveAt(0);
            for (int i = probs.Count() - 1; i >= 0; i--)
                probs[i] += probs[i + 1];
        }
    }

    public class HandGaussObject
    {
        public HandGaussObject(Player _player)
        {
            this.player = _player;
        }

        private Player player;
        public Player Player
        {
            get { return this.player; }
        }

        private Dictionary<string, List<double>> probablyofnames
            = new Dictionary<string, List<double>>();
        public Dictionary<string, List<double>> ProbablyOfNames
        {
            get { return this.probablyofnames; }
        }

        private Dictionary<Enum_CardColor, List<double>> probabysofcolors
            = new Dictionary<Enum_CardColor, List<double>>();
        public Dictionary<Enum_CardColor, List<double>> ProbablyOfColors
        {
            get { return this.probabysofcolors; }
        }


        private Dictionary<int, List<double>> probablyofpointsnotless
            = new Dictionary<int, List<double>>();
        public Dictionary<int, List<double>> ProbablyOfPointNotLess
        {
            get { return this.probablyofpointsnotless; }
        }

        public double GetProbablyOfCardKey(Context ctx, string cardkey)
        {
            List<double> problist = null;
            if (!probablyofnames.TryGetValue(cardkey, out problist)) return 0.0d;
            if (problist == null) return 0.0d;
            return problist.FirstOrDefault();
        }

        public double GetProbablyOfColor(Context ctx, Enum_CardColor cardcolor)
        {
            List<double> problist = null;
            if (!probabysofcolors.TryGetValue(cardcolor, out problist)) return 0.0d;
            if (problist == null) return 0.0d;
            return problist.FirstOrDefault();
        }

        public double GetProbablyOfPointNotLess(Context ctx, int cardpoint)
        {
            List<double> problist = null;
            if (!probablyofpointsnotless.TryGetValue(cardpoint, out problist)) return 0.0d;
            if (problist == null) return 0.0d;
            return problist.FirstOrDefault();
        }

        public double[] GetProbablyArrayOfCardKey(Context ctx, string cardkey)
        {
            List<double> problist = null;
            if (!probablyofnames.TryGetValue(cardkey, out problist)) return new double[] { };
            if (problist == null) return new double[] { };
            return problist.ToArray();
        }

        public double[] GetProbablyArrayOfColor(Context ctx, Enum_CardColor cardcolor)
        {
            List<double> problist = null;
            if (!probabysofcolors.TryGetValue(cardcolor, out problist)) return new double[] { };
            if (problist == null) return new double[] { };
            return problist.ToArray();
        }

        public double[] GetProbablyArrayOfPointNotLess(Context ctx, int cardpoint)
        {
            List<double> problist = null;
            if (!probablyofpointsnotless.TryGetValue(cardpoint, out problist)) return new double[] { };
            if (problist == null) return new double[] { };
            return problist.ToArray();
        }
    }


}
