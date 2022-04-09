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
    public class ChainCard : Card
    {
        public const string Normal = "铁索连环";

        public ChainCard()
        {
            KeyName = Normal;
            UseCondition = new UseChainCondition();
            TargetFilter = new UseChainTargetFilter();
            CardType = new CardType(Enum_CardType.Spell, new CardSubType(Enum_CardSubType.Immediate));
        }
        public override Card Create()
        {
            return new ChainCard();
        }
        
        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "这张卡的①②效果在出牌阶段发动。\n" + 
                "①：选择一至两名角色为对象发动，横置或者重置选择的对象。横置的角色受到属性伤害时重置，其他横置角色受到同来源的等量伤害并重置。\n" +
                "②：丢弃这张卡，并摸一张牌。",
                Image = ImageHelper.LoadCardImage("Cards", "Chain")
            };
        }
        public override double GetWorthForTarget()
        {
            return -2;
        }

        public override CardTargetFilterAuto GetAutoAI()
        {
            return new UseChainAutoAI(this);
        }
    }

    public class UseChainCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用铁索连环的条件";

        public UseChainCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }

    public class UseChainTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用铁索连环的目标";

        public UseChainTargetFilter()
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
                return true;
            }
            else if (selecteds.Count() == 1)
            {
                Player source = selecteds.FirstOrDefault();
                if (want == source) return false;
                return true;
            }
            return false;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() > 0;
        }
    }

    public class UseChainAutoAI : CardTargetFilterAuto
    {
        public UseChainAutoAI(Card _thiscard)
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
            PlayerFilter targetfilter = ctx.World.TryReplaceNewPlayerFilter(card.TargetFilter, null);
            List<KeyValuePair<double, Player>> hatreds = ctx.World.GetAlivePlayers()
                .Where(_player => targetfilter.CanSelect(ctx, new Player[] { }, _player))
                .Select(_player => new KeyValuePair<double, Player>(AITools.WorthAcc.GetHatred(ctx, user, _player), _player))
                .ToList();
            List<KeyValuePair<double, Player>> worthsorts = hatreds
                .Select(_kvp => new KeyValuePair<double, Player>(_kvp.Key * (_kvp.Value.IsChained ? 1 : -1), _kvp.Value))
                .ToList();
            double worthall = 0.0d;
            selections.Clear();
            worthsorts.Sort((_kvp0, _kvp1) => -_kvp0.Key.CompareTo(_kvp1.Key));
            foreach (KeyValuePair<double, Player> kvp in worthsorts)
            {
                if (targetfilter.Fulfill(ctx, selections)) break;
                if (!targetfilter.CanSelect(ctx, selections, kvp.Value)) continue;
                selections.Add(kvp.Value);
                worthall += kvp.Key;
            }
            return worthall;
        }
    }
}
