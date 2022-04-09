using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.AIs;
using TouhouSha.Core.Events;

namespace TouhouSha.Koishi.Cards
{
    public class BorrowKnifeCard : Card
    {
        public const string Normal = "借刀杀人";

        public BorrowKnifeCard()
        {
            KeyName = Normal;
            UseCondition = new UseBorrowKnifeCondition();
            TargetFilter = new UseBorrowKnifeTargetFilter();
            CardType = new CardType(Enum_CardType.Spell, new CardSubType(Enum_CardSubType.Immediate));
        }

        public override Card Create()
        {
            return new BorrowKnifeCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "出牌阶段，选择除你以外装备区里有武器牌的一名角色使用。该角色需对其攻击范围内你指定的另一名角色使用一张【杀】，否则将武器牌交给你。",
                Image = ImageHelper.LoadCardImage("Cards", "Borrow")
            };
        }
        public override double GetWorthForTarget()
        {
            return -2;
        }

        public override CardTargetFilterAuto GetAutoAI()
        {
            return base.GetAutoAI();
        }
    }

    public class UseBorrowKnifeCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用借刀杀人的条件";

        public UseBorrowKnifeCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }

    public class UseBorrowKnifeTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用借刀杀人的目标";

        public UseBorrowKnifeTargetFilter()
        {
            KeyName = DefaultKeyName;
        }
        
        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            Player player = state.Owner;
            if (player == null) return false;
            if (selecteds.Count() == 0)
            {
                Zone equipzone = want.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Equips) == true);
                if (equipzone == null) return false;
                Card weapon = equipzone.Cards.FirstOrDefault(_card => _card.CardType?.E == Enum_CardType.Equip && _card.CardType.SubType?.E == Enum_CardSubType.Weapon);
                if (weapon == null) return false;
                if (player == want) return false;
                return true;
            }
            else if (selecteds.Count() == 1)
            {
                Player source = selecteds.FirstOrDefault();
                if (want == source) return false;
                if (!ctx.World.IsInDistance2Kill(ctx, source, want)) return false;
                return true;
            }
            return false;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() == 2;
        }
    }

    public class UseBorrowKnifeAutoAI : CardTargetFilterAuto
    {
        public UseBorrowKnifeAutoAI(Card _thiscard)
        {
            this.thiscard = _thiscard;
        }

        private Card thiscard;
        public Card ThisCard { get { return this.thiscard; } }

        public override double GetUseWorth(Context ctx, Card card, Player user)
        {
            List<Player> selections = new List<Player>();
            return GetSelectionAndWorth(ctx, card, user, selections);
        }

        public override List<Player> GetSelection(Context ctx, Card card, Player user)
        {
            List<Player> selections = new List<Player>();
            GetSelectionAndWorth(ctx, card, user, selections);
            return selections;
        }

        protected double GetSelectionAndWorth(Context ctx, Card card, Player user, List<Player> selections)
        {
            Card killcard = ctx.World.GetCardInstance(KillCard.Normal);
            double optworth = double.MinValue;
            PlayerFilter targetfilter = ctx.World.TryReplaceNewPlayerFilter(card.TargetFilter, null);
            killcard = killcard.Clone();
            killcard.Zone = user.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            foreach (Player target0 in ctx.World.GetAlivePlayers())
            {
                if (!targetfilter.CanSelect(ctx, new Player[] { }, target0)) continue;
                double killrate = AITools.CardGausser.GetProbablyOfCardKey(ctx, target0, "Kill");
                EquipZone equipzone = target0.Zones.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
                Card weapon = equipzone.Cards[equipzone.Cells.FirstOrDefault(_cell => _cell.E == Enum_CardSubType.Weapon).CardIndex];
                double worth = (1 - killrate) * -AITools.WorthAcc.GetWorth(ctx, user, weapon);
                Player optsubtarget = null;
                double optsubworth = double.MinValue;
                if (killrate < 0.1) killrate = 0.1;
                weapon.Zone = user.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                worth += (1 - killrate) * AITools.WorthAcc.GetWorth(ctx, user, weapon);
                weapon.Zone = equipzone;
                foreach (Player target1 in ctx.World.GetAlivePlayers())
                {
                    if (!targetfilter.CanSelect(ctx, new Player[] { target0 }, target1)) continue;
                    double subworth = killcard.GetWorthAI().GetWorth(ctx, killcard, user, new Player[] { }, target1);
                    if (subworth > optsubworth)
                    {
                        optsubworth = subworth;
                        optsubtarget = target1;
                    }
                }
                if (optsubtarget != null)
                {
                    worth += optsubworth * killrate;
                    if (worth >= optworth)
                    {
                        optworth = worth;
                        selections.Clear();
                        selections.Add(target0);
                        selections.Add(optsubtarget);
                    }
                }
            }
            killcard.Zone = null;
            return optworth;
        }
    }
}
