using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Koishi.Cards.Horses
{
    /// <summary>
    /// 马匹【-1马】
    /// </summary>
    public class MinusHorseCard : Card
    {
        public string DefaultKeyName = "攻击UFO";

        public MinusHorseCard()
        {
            KeyName = DefaultKeyName;
            UseCondition = new MinusHorseUseCondition(this);
            TargetFilter = new MinusHorseTargetFilter(this);
            CardType = new CardType(Enum_CardType.Equip, new CardSubType(Enum_CardSubType.Armor));
            Skills.Add(new MinusHorseSkill(this));
        }
        public override Card Create()
        {
            return new MinusHorseCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "你计算与其他角色的距离时-1。",
                Image = ImageHelper.LoadCardImage("Cards", "AttackHorse")
            };
        }
        public override double GetWorthForTarget()
        {
            return 2;
        }
    }

    public class MinusHorseSkill : Skill
    {
        public MinusHorseSkill(Card _horse)
        {
            this.horse = _horse;
            IsLocked = true;
            Calculators.Add(new MinusDistanceCalculator(horse));
        }

        private Card horse;
        public Card Horse { get { return this.horse; } }

        public override Skill Clone()
        {
            return new MinusHorseSkill(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new MinusHorseSkill(newcard);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }
    }
    
    public class MinusHorseUseCondition : ConditionFilterFromCard
    {
        public MinusHorseUseCondition(Card _card) : base(_card)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner == null) return false;
            EquipZone equipzone = state.Owner.Zones.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
            if (equipzone == null) return false;
            EquipCell armorcell = equipzone.Cells.FirstOrDefault(_cell => _cell.E == Enum_CardSubType.HorseMinus);
            if (armorcell == null) return false;
            if (!armorcell.IsEnabled) return false;
            return true;
        }
    }

    public class MinusHorseTargetFilter : PlayerFilterFromCard
    {
        public MinusHorseTargetFilter(Card _card) : base(_card)
        {

        }

        public override Enum_PlayerFilterFlag GetFlag(Context ctx)
        {
            return Enum_PlayerFilterFlag.ForceAll;
        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            Player player = state.Owner;
            if (player == null) return false;
            return player == want;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() > 0;
        }
    }
   
    public class MinusDistanceCalculator : CalculatorFromCard, ICalculatorProperty
    {
        public MinusDistanceCalculator(Card _card) : base(_card) { }

        string ICalculatorProperty.PropertyName { get => World.DistanceMinus; }

        public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            if (!(obj is Player)) return oldvalue;
            if (!propertyname.Equals(World.DistanceMinus)) return oldvalue;
            Player player = (Player)obj;
            if (player != card.Owner) return oldvalue;
            if (card.Zone?.KeyName?.Equals(Zone.Equips) != true) return oldvalue;
            return oldvalue + 1;
        }
    }
}
