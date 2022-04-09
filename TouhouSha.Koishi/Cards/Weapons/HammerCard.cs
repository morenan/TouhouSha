using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Calculators;

namespace TouhouSha.Koishi.Cards.Weapons
{
    /// <summary>
    /// 武器【万宝锤】
    /// </summary>
    /// <remarks>
    /// 武器范围：4
    /// 出牌阶段限一次，你可以丢弃X张牌，并摸取X-1张牌。
    /// </remarks>
    public class HammerCard : SelfWeapon
    {
        public const string Normal = "万宝锤";
        
        public HammerCard()
        {
            KeyName = Normal;
            Skills.Add(new SkillHammer(this));
        }
        public override Card Create()
        {
            return new HammerCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "攻击距离4。出牌阶段限一次，你可以丢弃X张牌，并摸取X-1张牌。",
                Image = ImageHelper.LoadCardImage("Cards", "Hammer")
            };
        }
        public override double GetWorthForTarget()
        {
            return 3;
        }
    }
    
    public class SkillHammer : Skill, ISkillInitative
    {
        public SkillHammer(Card _weapon)
        {
            this.weapon = _weapon;
            this.usecondition = new SkillHammerCondition(weapon);
            this.targetfilter = new SkillHammerTargetFilter(weapon);
            this.costfilter = new SkillHammerCostFilter(weapon);
            Calculators.Add(new WeaponKillRangePlusCalculator(weapon, 3));
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }
        
        private ConditionFilter usecondition;
        public ConditionFilter UseCondition { get { return this.usecondition; } }

        private PlayerFilter targetfilter;
        public PlayerFilter TargetFilter { get { return this.targetfilter; } }

        private CardFilter costfilter;
        public CardFilter CostFilter { get { return this.costfilter; } }

        public void Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
        {
            CardSkillEvent ev = new CardSkillEvent();
            int costnumber = costs.Count();
            ev.Reason = null;
            ev.Skill = this;
            ev.Card = weapon;
            ev.Source = skilluser;
            ev.Targets.Clear();
            ev.Targets.AddRange(targets);
            Zone discardzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
            if (discardzone == null) return;
            ctx.World.MoveCards(skilluser, costs, discardzone, ev);
            ctx.World.DrawCard(skilluser, costnumber - 1, ev);
        }

        public override Skill Clone()
        {
            return new SkillHammer(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new SkillHammer(newcard);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }
    }

    public class SkillHammerCondition : ConditionFilterFromCard
    {
        //public const string Defined = "对万宝槌使用情况的注册";
        public const string Used = "已经使用过万宝槌的次数";
        //public const string Maximum = "最多使用万宝槌的次数";

        public SkillHammerCondition(Card _weapon) : base(_weapon)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state?.KeyName?.Equals(State.UseCard) != true) return false;
            return state.GetValue(Used) < 1;
        }
    }

    public class SkillHammerTargetFilter : PlayerFilterFromCard
    {
        public SkillHammerTargetFilter(Card _weapon) : base(_weapon)
        {

        }

        public override Enum_PlayerFilterFlag GetFlag(Context ctx)
        {
            return Enum_PlayerFilterFlag.ForceAll;
        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            return want == card.Zone?.Owner;

        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() == 1;
        }

    }

    public class SkillHammerCostFilter : CardFilterFromCard
    {
        public SkillHammerCostFilter(Card _weapon) : base(_weapon)
        {

        }
        
        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (want.Zone == null) return false;
            if (String.IsNullOrEmpty(want.Zone.KeyName)) return false;
            if (want == card) return false;
            if (want.Zone.KeyName.Equals(Zone.Hand)) return true;
            if (want.Zone.KeyName.Equals(Zone.Equips)) return true;
            return false;
        }
    }


}
