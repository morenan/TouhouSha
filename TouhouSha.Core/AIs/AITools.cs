using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.AIs
{
    public class AITools
    {
        static public double ForceAll(Context ctx, PlayerFilter targetfilter, List<Player> targets, Func<Player, double> getworth)
        {
            double worth = 0;
            foreach (Player target in ctx.World.GetAlivePlayers())
            {
                if (!targetfilter.CanSelect(ctx, targets, target)) continue;
                worth += getworth(target);
                targets.Add(target);
            }
            return worth;
        }
        static public double ForceAll(Context ctx, Player owner, CardFilter cardfilter, List<Card> selections, Func<Card, double> getworth)
        {
            double worth = 0;
            foreach (Zone zone in owner.Zones)
                foreach (Card card in zone.Cards)
                {
                    Card seemas = ctx.World.CalculateCard(ctx, card);
                    if (!cardfilter.CanSelect(ctx, selections, seemas)) continue;
                    worth = getworth(card);
                    selections.Add(seemas);
                }
            return worth;
        }

        static public double StepOptimizeAlgorithm(Context ctx, PlayerFilter targetfilter, List<Player> targets, Func<Player, double> getworth)
        {
            double worth = 0;
            if ((targetfilter.GetFlag(ctx) & Enum_PlayerFilterFlag.ForceAll) != Enum_PlayerFilterFlag.None)
                return ForceAll(ctx, targetfilter, targets, getworth);
            while (true)
            {
                double optworth = double.MinValue;
                Player opttarget = null;
                foreach (Player target in ctx.World.GetAlivePlayers())
                {
                    if (targets.Contains(target)) continue;
                    if (!targetfilter.CanSelect(ctx, targets, target)) continue;
                    double subworth = getworth(target);
                    if (subworth > optworth)
                    {
                        optworth = subworth;
                        opttarget = target;
                    }
                }
                if (opttarget == null) break;
                if (optworth <= 0 && targetfilter.Fulfill(ctx, targets)) break;
                targets.Add(opttarget);
                worth += optworth;
            }
            return worth;
        }

        static public double StepOptimizeAlgorithm(Context ctx, Player owner, CardFilter cardfilter, List<Card> selections, Func<Card, double> getworth)
        {
            HashSet<Card> handles = new HashSet<Card>();
            double worth = 0;
            if ((cardfilter.GetFlag(ctx) & Enum_CardFilterFlag.ForceAll) != Enum_CardFilterFlag.None)
                return ForceAll(ctx, owner, cardfilter, selections, getworth);
            while (true)
            {
                double optworth = double.MinValue;
                Card optcard = null;
                Card optseemas = null;
                foreach (Zone zone in owner.Zones)
                    foreach (Card card in zone.Cards)
                    {
                        if (handles.Contains(card)) continue;
                        Card seemas = ctx.World.CalculateCard(ctx, card);
                        if (!cardfilter.CanSelect(ctx, selections, seemas)) continue;
                        double subworth = getworth(card);
                        if (subworth > optworth)
                        {
                            optworth = subworth;
                            optcard = card;
                            optseemas = seemas;
                        }
                    }
                if (optcard == null) break;
                if (optworth <= 0 && cardfilter.Fulfill(ctx, selections)) break;
                handles.Add(optcard);
                selections.Add(optseemas);
                worth += optworth;
            }
            return worth;
        }

        public AITools(IWorthAccessment _worthacc, ICardGausser _cardgausser, IAssGausser _assgausser)
        {
            this.worthacc = _worthacc;
            this.cardgausser = _cardgausser;
            this.assgausser = _assgausser;
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

        
    }
}
