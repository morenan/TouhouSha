using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.AIs;
using TouhouSha.Koishi.Cards;
using TouhouSha.Koishi.Cards.Weapons;
using TouhouSha.Koishi.Cards.Armors;

namespace TouhouSha.AI
{

    /// <summary>
    /// 默认的价值评估器。
    /// </summary>
    public class WorthAccessment : IWorthAccessment
    {
        public WorthAccessment(Player _owner)
        {
            this.owner = _owner;
        }

        private Player owner;
        public Player Owner
        {
            get { return this.owner; }
        }

        private IAssGausser assgausser;
        public IAssGausser AssGausser
        {
            get { return this.assgausser; }
            set { this.assgausser = value; }
        }

        private ICardGausser cardgausser;
        public ICardGausser CardGausser
        {
            get { return this.cardgausser; }
            set { this.cardgausser = value; }
        }
        
        protected double GetWorthWithoutAss(Context ctx, Player controller, Card card)
        {
            switch (card.Zone?.KeyName)
            {
                case Zone.Hand:
                case Zone.Equips:
                    switch (card.KeyName)
                    {
                        case KillCard.Normal: return 0.8d;
                        case KillCard.Thunder: return 0.9d;
                        case KillCard.Fire: return 1.0d;
                        case MissCard.Normal: return 1.0d;
                        case PeachCard.Normal: return 1.5d;
                        case LiqureCard.Normal: return 0.8d;
                        case BirthCard.Normal: return 1.25d;
                        case PeachAllCard.Normal: return 0.8d;
                        case AttackAllCard.Normal: return 1.5d;
                        case ArrowAllCard.Normal: return 1.5d;
                        case SheepCard.Normal: return 1.5d;
                        case BridgeBoomCard.Normal: return 1.25d;
                        case BorrowKnifeCard.Normal: return 1.5d;
                        case FireCard.Normal: return 0.9d;
                        case ChainCard.Normal: return 1.25d;
                        case HappyCard.Normal: return 2.0d;
                        case HungerCard.Normal: return 1.5d;
                        case LightCard.Normal: return 0.0d;
                    }
                    return 1.0d;
                case Zone.Judge:
                    switch (card.KeyName)
                    {
                        case HappyCard.Normal:
                            return -GetWorthInPhase(ctx, card.Zone.Owner, card.Zone.Owner);
                        case HungerCard.Normal:
                            return -2.0d;
                        case LightCard.Normal:
                            return -(8.0d / (13 * 4)) * 6;
                    }
                    return 0.0d;
            }
            switch (card.Zone?.KeyName)
            {
                case Zone.Hand:
                    switch (card.CardType?.SubType?.E)
                    {
                        case Enum_CardSubType.Weapon: return 2.0d;
                        case Enum_CardSubType.Armor: return 2.0d;
                        case Enum_CardSubType.HorsePlus: return 2.0d;
                        case Enum_CardSubType.HorseMinus: return 1.5d;
                    }
                    return 1.0d;
                case Zone.Equips:
                    if (card is SelfWeapon)
                    {
                        SelfWeapon weapon0 = card.Zone.Cards.FirstOrDefault(_card => _card is SelfWeapon) as SelfWeapon;
                        SelfWeapon weapon1 = (SelfWeapon)card;
                        int range0 = weapon0?.GetWeaponRange() ?? 1;
                        int range1 = weapon1.GetWeaponRange();
                        return GetWorth(ctx, controller, weapon1, range1 - range0);
                    }
                    switch (card.CardType?.SubType?.E)
                    {
                        case Enum_CardSubType.Weapon: return 2.0d;
                        case Enum_CardSubType.Armor: return 3.0d;
                        case Enum_CardSubType.HorsePlus: return 2.0d;
                        case Enum_CardSubType.HorseMinus: return 1.5d;
                    }
                    break;
            }
            return 0.0d;
        }

        protected double GetWorth(Context ctx, Player controller, SelfWeapon weapon, int rangedelta)
        {
            double optworth = 0;
            double damagetime = 2;
            int killmax = ctx.World.CalculateValue(ctx, weapon.Zone.Owner, KillCard.KillMaxUse);
            switch (weapon.KeyName)
            {
                case AkCard.Normal:
                    killmax = 9999;
                    break;
                case LouKanCard.Normal:
                    killmax++;
                    break;
            }
            if (weapon.Zone.Owner == controller)
            {
                Zone handzone = controller.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                damagetime = Math.Min(killmax, handzone?.Cards?.Count(_card => KillCard.IsKill(_card)) ?? 0) + 1;
            }
            else
            {
                double[] probs = CardGausser.GetProbablyArrayOfCardKey(ctx, weapon.Zone.Owner, "Kill");
                for (int i = 0; i < probs.Count(); i++)
                {
                    if (i + 1 >= probs.Count())
                        damagetime += probs[i] * Math.Min(killmax, (i + 1));
                    else
                        damagetime += (probs[i] - probs[i + 1]) * Math.Min(killmax, (i + 1));
                }
                damagetime += 1;
            }
            foreach (Player target in ctx.World.GetAlivePlayers())
            {
                if (controller == target) continue;
                if (!ctx.World.IsInDistance2Kill(ctx, controller, target)) continue;
                double damageworth = -GetWorthPerHp(ctx, controller, target);
                double worth = damageworth * damagetime;
                EquipZone equipzone = target.Zones.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
                foreach (Card equipcard in equipzone.Cards)
                    switch (equipcard.KeyName)
                    {
                        case YinYangYuCard.DefaultKeyName:
                        case DarkBallCard.DefaultKeyName:
                            switch (weapon.KeyName)
                            {
                                case HisoCard.Normal:
                                    break;
                                default:
                                    worth /= 2;
                                    break;
                            }
                            break;
                        case MarisaHatCard.DefaultKeyName:
                            switch (weapon.KeyName)
                            {
                                case HisoCard.Normal:
                                    break;
                                default:
                                    {
                                        Zone handzone = target.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                                        if (handzone != null)
                                            worth -= damageworth * Math.Min(damagetime, handzone.Cards.Count() * 0.25);
                                    }
                                    break;
                            }
                            break;
                    }
                switch (weapon.KeyName)
                {
                    case BowCard.Normal:
                        if (equipzone != null)
                            foreach (Card equipcard in equipzone.Cards)
                                switch (equipcard.CardType?.SubType?.E)
                                {
                                    case Enum_CardSubType.HorsePlus:
                                    case Enum_CardSubType.HorseMinus:
                                        worth += -GetWorth(ctx, controller, equipcard);
                                        break;
                                }
                        break;
                    case LouKanCard.Normal:
                        worth += damagetime * 0.75;
                        break;
                    case GoheCard.Normal:
                        worth += damagetime;
                        break;
                }
                optworth = Math.Max(optworth, worth);
            }
            return optworth;
        }

        public double GetHatred(Context ctx, Player controller, Player target)
        {
            double hatred = 1.0d;
            double assfactor = 0.0d;
            if (target.Ass?.E == Enum_PlayerAss.Leader) hatred += 0.5;
            if (target.HP < 4) hatred += (4 - target.HP) * 0.1;
            if (AssGausser != null)
            {
                double ass0 = 1.0d;
                double ass1 = AssGausser.ProbablyOfSlave(ctx, target) - AssGausser.ProbablyOfAvenger(ctx, target);
                if (controller.Ass?.E == Enum_PlayerAss.Avenger) ass0 = -1.0d;
                assfactor = ass0 * ass1;
            }
            return hatred * assfactor;
        }

        public double GetWorthInPhase(Context ctx, Player controller, Player target)
        {
            double worth = 0;
            Zone handzone = target.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone != null) worth += (handzone.Cards.Count() + 2) * GetWorthExpected(ctx, controller, handzone);
            return worth;
        }

        public double GetWorth(Context ctx, Player controller, Player target)
        {
            double worth = 0;
            foreach (Zone zone in target.Zones)
            {
                if (controller != target && zone.KeyName?.Equals(Zone.Hand) == true)
                {
                    worth += GetWorthExpected(ctx, controller, zone) * zone.Cards.Count();
                    continue;
                }
                foreach (Card card in zone.Cards)
                    worth += GetWorth(ctx, controller, card);
            }
            worth += GetWorthPerHp(ctx, controller, target) * target.HP;
            worth += GetWorthPerMaxHp(ctx, controller, target) * target.MaxHP;
            if (target.IsFacedDown) worth -= 2.88d * GetHatred(ctx, controller, target);
            return worth;
        }

        public double GetWorth(Context ctx, Player controller, Card card)
        {
            return GetWorth(ctx, controller, card, Enum_CardBehavior.Lost);
        }

        public double GetWorth(Context ctx, Player controller, Card card, Enum_CardBehavior behavior)
        {
            if (card.Zone?.Owner == null) return 0.0d;
            double worth = GetWorthWithoutAss(ctx, controller, card);
            worth *= GetHatred(ctx, controller, card.Zone.Owner);
            return worth;
        }

        public double GetWorth(Context ctx, Player controller, Card card, int indexofzone)
        {
            return GetWorth(ctx, controller, card);
        }

        public double GetWorthAsk(Context ctx, Player controller, string keyname)
        {
            return 1.0d;
        }

        public double GetWorthExpected(Context ctx, Player controller, Zone zone)
        {
            if (zone.Owner == null) return 0.0d;
            double worth = 0.9d;
            worth *= GetHatred(ctx, controller, zone.Owner);
            return worth;
        }

        public double GetWorthHandle(Context ctx, Player controller, Card handledcard)
        {
            return 1.0d;
        }

        public double GetWorthPerHp(Context ctx, Player controller, Player target)
        {
            return GetWorthPerHp(ctx, controller, target, Enum_HpBehavior.Damage);
        }

        public double GetWorthPerHp(Context ctx, Player controller, Player target, Enum_HpBehavior behavior)
        {
            double worth = 2.0d;
            worth *= GetHatred(ctx, controller, target);
            return worth;
        }

        public double GetWorthPerMaxHp(Context ctx, Player controller, Player target)
        {
            double worth = 4.0d;
            worth *= GetHatred(ctx, controller, target);
            return worth;
        }

        public double GetWorthSelect(Context ctx, Player controller, string keyname, Card card, IEnumerable<Card> selecteds)
        {
            return -GetWorth(ctx, controller, card);
        }

        public double GetWorthSelect(Context ctx, Player controller, string keyname, Player target, IEnumerable<Player> selecteds)
        {
            return -GetHatred(ctx, controller, target);
        }

        public double GetWorthSelect(Context ctx, Player controller, Skill skill, Card card, IEnumerable<Card> selecteds)
        {
            return -GetWorth(ctx, controller, card);
        }

        public double GetWorthSelect(Context ctx, Player controller, Skill skill, Player target, IEnumerable<Player> selecteds)
        {
            return GetHatred(ctx, controller, target);
        }

        public double GetWorthUse(Context ctx, Player controller, Card usecard, Player target, IEnumerable<Player> selecteds)
        {
            return -GetWorth(ctx, controller, usecard) + GetHatred(ctx, controller, target) * usecard.GetWorthForTarget();
        }

        public double GetWorthPointBattleWin(Context ctx, Player controller, Player target, Event reason)
        {
            return -GetWorthExpected(ctx, controller, target.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true));
        }

        public double GetWorthPointBattleLose(Context ctx, Player controller, Player target, Event reason)
        {
            return -GetWorthExpected(ctx, controller, target.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true));
        }

    }
}
