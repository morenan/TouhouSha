using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Koishi.Cards.Armors
{
    /// <summary>
    /// 防具【魔女帽】
    /// </summary>
    /// <remarks>
    /// 你可以将任意两张牌当作【闪】使用或打出。
    /// </remarks>
    public class MarisaHatCard : SelfArmor
    {
        public const string DefaultKeyName = "魔女帽";

        public MarisaHatCard()
        {
            KeyName = DefaultKeyName;
            Skills.Add(new MarisaHat_Skill(this));
        }
        public override Card Create()
        {
            return new MarisaHatCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "你可以将任意两张牌当作【闪】使用或打出。",
                Image = ImageHelper.LoadCardImage("Cards", "MarisaHat")
            };
        }
        public override double GetWorthForTarget()
        {
            return 3;
        }
    }

    public class MarisaHat_Skill : Skill, ISkillCardConverter, ISkillCardMultiConverter2
    {
        public MarisaHat_Skill(Card _armor)
        {
            this.armor = _armor;
            this.usecondition = new MarisaHat_Skill_UseCondition(armor, this);
            this.cardfilter = new MarisaHat_Skill_CardFilter(armor);
            this.cardconverter = new MarisaHat_Skill_CardConverter(armor);
        }

        private Card armor;
        public Card Armor { get { return this.armor; } }

        private MarisaHat_Skill_UseCondition usecondition;
        ConditionFilter ISkillCardConverter.UseCondition => usecondition;

        private MarisaHat_Skill_CardFilter cardfilter;
        CardFilter ISkillCardConverter.CardFilter => cardfilter;

        private MarisaHat_Skill_CardConverter cardconverter;
        CardCalculator ISkillCardConverter.CardConverter => cardconverter;

        private List<string> enabledcardtypes;
        IEnumerable<string> ISkillCardMultiConverter2.EnabledCardTypes => enabledcardtypes ?? new List<string> { MissCard.Normal };
        
        void ISkillCardMultiConverter2.CancelEnabledCardTypes(Context ctx)
        {
            this.enabledcardtypes = null;
        }

        void ISkillCardMultiConverter2.SetEnabledCardTypes(Context ctx, IEnumerable<string> cardtypes)
        {
            if (cardtypes.Contains(MissCard.Normal))
                this.enabledcardtypes = new List<string> { MissCard.Normal };
            else
                this.enabledcardtypes = null;
        }
        public override Skill Clone()
        {
            return new MarisaHat_Skill(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new MarisaHat_Skill(newcard);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }
    }

    public class MarisaHat_Skill_UseCondition : ConditionFilterFromCard
    {
        public MarisaHat_Skill_UseCondition(Card _card, Skill _cardskill) : base(_card)
        {
            this.cardskill = _cardskill;
        }

        private Skill cardskill;
        public Skill CardSkill { get { return this.cardskill; } }

        public override bool Accept(Context ctx)
        {
            if (card.Owner == null) return false;
            if (card.Zone?.KeyName?.Equals(Zone.Equips) != true) return false;
            if (!(cardskill is ISkillCardMultiConverter2)) return false;
            ISkillCardMultiConverter2 multiconv = (ISkillCardMultiConverter2)cardskill;
            if (multiconv.EnabledCardTypes == null) return false;
            if (multiconv.EnabledCardTypes.Count() == 0) return false;
            return true;
        }
    }

    public class MarisaHat_Skill_CardFilter : CardFilterFromCard
    {
        public MarisaHat_Skill_CardFilter(Card _card) : base(_card)
        {

        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() >= 2) return false;
            if (want.Zone?.KeyName?.Equals(Zone.Hand) != true) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() >= 2;
        }
    }

    public class MarisaHat_Skill_CardConverter : CardCalculatorFromCard
    {
        public MarisaHat_Skill_CardConverter(Card _card) : base(_card)
        {

        }
        
        public override Card GetCombine(Context ctx, IEnumerable<Card> oldvalue)
        {
            Card misscard = ctx.World.GetCardInstance(MissCard.Normal);
            if (oldvalue.Count() < 1)
            {
                misscard = misscard.Clone(oldvalue.FirstOrDefault());
                return misscard;
            }
            Card oldcard0 = oldvalue.FirstOrDefault();
            Card oldcard1 = oldvalue.LastOrDefault();
            if (oldcard0.CardColor?.E == oldcard1.CardColor?.E)
            {
                misscard = misscard.Clone(oldcard0);
                misscard.OriginCards.Add(oldcard1);
                misscard.CardPoint = -1;
                return misscard;
            }
            if (oldcard0.CardColor?.SeemAs(Enum_CardColor.Red) == true
             && oldcard1.CardColor?.SeemAs(Enum_CardColor.Red) == true)
            {
                misscard = misscard.Clone(oldcard0);
                misscard.OriginCards.Add(oldcard1);
                misscard.CardColor = new CardColor(Enum_CardColor.Red);
                misscard.CardPoint = -1;
                return misscard;
            }
            if (oldcard0.CardColor?.SeemAs(Enum_CardColor.Black) == true
             && oldcard1.CardColor?.SeemAs(Enum_CardColor.Black) == true)
            {
                misscard = misscard.Clone(oldcard0);
                misscard.OriginCards.Add(oldcard1);
                misscard.CardColor = new CardColor(Enum_CardColor.Black);
                misscard.CardPoint = -1;
                return misscard;
            }
            misscard = misscard.Clone(oldcard0);
            misscard.OriginCards.Add(oldcard1);
            misscard.CardColor = new CardColor(Enum_CardColor.None);
            misscard.CardPoint = -1;
            return misscard;
        }
    }


}
