using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;
using TouhouSha.Reimu.Charactors.SelfCrafts;

namespace TouhouSha.Reimu.Charactors.Homos
{
    /// <summary>
    /// 角色【帕秋莉】
    /// </summary>
    /// <remarks>
    /// 势力：红魔 3勾玉
    /// 【贤石】：出牌阶段，你可以关联两种花色的组合和一个牌名（非装备）。每种花色组合和每个牌名整场游戏限选择一次。
    /// 【魔法】：你可以将两张牌视做一种牌来打出，这两张牌的花色组合和打出的牌的排名必须通过【贤石】关联过。
    /// 【哮喘】：锁定技，当你在出牌阶段以【魔法】连续打出牌时，你对自己造成一点伤害。当你受到一点伤害时，你摸两张牌。
    /// </remarks>
    public class Pachuli : Charactor
    {
        public const string UsedByMagic = "使用帕琪的组合魔法打出的";
        public const string MagicSequentlyNumber = "连续使用魔法打出的数量";

        public static readonly Enum_CardColor[] Color4s = new Enum_CardColor[]
        {
            Enum_CardColor.Club,
            Enum_CardColor.Diamond,
            Enum_CardColor.Heart,
            Enum_CardColor.Spade,
        };

        public Pachuli()
        {
            MaxHP = 3;
            HP = 3;
            Country = Remilia.CountryNameOfLeader;
            OtherCountries.Add(Marisa.CountryNameOfLeader);
            Skills.Add(new Pachuli_Skill_0());
            Skills.Add(new Pachuli_Skill_1());
            Skills.Add(new Pachuli_Skill_2());
        }

        public override Charactor Clone()
        {
            return new Pachuli();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "帕秋莉";
            info.AbilityRadar = new AbilityRadar() { Attack = 4, Defence = 4, Control = 4, Auxiliary = 4, LastStages = 4, Difficulty = 0 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Pachuli");
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }

        private Dictionary<KeyValuePair<Enum_CardColor, Enum_CardColor>, string> relations
            = new Dictionary<KeyValuePair<Enum_CardColor, Enum_CardColor>, string>();
        public Dictionary<KeyValuePair<Enum_CardColor, Enum_CardColor>, string> Relations
        {
            get { return this.relations; }
        }
        
        public List<Card> GetEnabledRemainCards(World world)
        {
            List<string> cardnames = new List<string>();
            cardnames.AddRange(world.GetBaseCardKeyNames());
            cardnames.AddRange(world.GetSpellCardKeyNames());
            return cardnames.Where(_name => !relations.ContainsValue(_name)).Select(_name => world.GetCardInstance(_name)).ToList();
        }
       
        public List<Enum_CardColor> GetEnabledCardColorPatternFirst()
        {
            List<Enum_CardColor> colors = new List<Enum_CardColor>();
            foreach (Enum_CardColor color0 in Color4s)
            {
                List<Enum_CardColor> secondlist = GetEnabledCardColorPatternSecond(color0);
                if (secondlist.Count() == 0) continue;
                colors.Add(color0);
            }
            return colors;
        }

        public List<Enum_CardColor> GetEnabledCardColorPatternSecond(Enum_CardColor colorfirst)
        {
            List<Enum_CardColor> colors = new List<Enum_CardColor>();
            foreach (Enum_CardColor color1 in Color4s)
            {
                KeyValuePair<Enum_CardColor, Enum_CardColor> key0 = new KeyValuePair<Enum_CardColor, Enum_CardColor>(colorfirst, color1);
                KeyValuePair<Enum_CardColor, Enum_CardColor> key1 = new KeyValuePair<Enum_CardColor, Enum_CardColor>(color1, colorfirst);
                if (relations.ContainsKey(key0)) continue;
                if (relations.ContainsKey(key1)) continue;
                colors.Add(color1);
            }
            return colors;
        }
        
    }
    
    public class Pachuli_Skill_0 : Skill, ISkillInitative
    {
        public Pachuli_Skill_0()
        {
            this.usecondition = new Pachuli_Skill_0_UseCondition(this);
            this.targetfilter = new Pachuli_Skill_0_TargetFilter(this);
            this.costfilter = new Pachuli_Skill_0_CostFilter(this);
        }

        private Pachuli_Skill_0_UseCondition usecondition;
        public ConditionFilter UseCondition { get => usecondition; }

        private Pachuli_Skill_0_TargetFilter targetfilter;
        public PlayerFilter TargetFilter { get => targetfilter; }

        private Pachuli_Skill_0_CostFilter costfilter;
        public CardFilter CostFilter { get => costfilter; }

        public void Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
        {
            Pachuli pachi = skilluser.Charactors.FirstOrDefault(_char => _char is Pachuli) as Pachuli;
            if (pachi == null) return;
            List<Card> cards = pachi.GetEnabledRemainCards(ctx.World);
            if (cards.Count() == 0) return;
            List<Enum_CardColor> colorfirsts = pachi.GetEnabledCardColorPatternFirst();
            if (colorfirsts.Count() == 0) return;
            ctx.World.ShowList(KeyName, "请选择一种牌。", skilluser,
                cards, 1, true, 15, (selecteds) =>
                {
                    Card selectedcard = selecteds.FirstOrDefault() as Card;
                    if (selectedcard == null) return;
                    ctx.World.ShowList(KeyName, "请选择第一个花色。", skilluser,
                       colorfirsts.Cast<object>(), 1, true, 15, (selected1s) =>
                       {
                           Enum_CardColor colorfirst = (Enum_CardColor)selected1s.FirstOrDefault();
                           List<Enum_CardColor> colorseconds = pachi.GetEnabledCardColorPatternSecond(colorfirst);
                           ctx.World.ShowList(KeyName, "请选择第二个花色", skilluser,
                               colorseconds.Cast<object>(), 1, true, 15, (selected2s) =>
                               {
                                   Enum_CardColor colorsecond = (Enum_CardColor)selected2s.FirstOrDefault();
                                   KeyValuePair<Enum_CardColor, Enum_CardColor> key0 = new KeyValuePair<Enum_CardColor, Enum_CardColor>(colorfirst, colorsecond);
                                   KeyValuePair<Enum_CardColor, Enum_CardColor> key1 = new KeyValuePair<Enum_CardColor, Enum_CardColor>(colorsecond, colorfirst);
                                   if (!pachi.Relations.ContainsKey(key0)) pachi.Relations.Add(key0, selectedcard.KeyName);
                                   if (!pachi.Relations.ContainsKey(key1)) pachi.Relations.Add(key1, selectedcard.KeyName);
                               }, null);
                       }, null);
                }, null);
        }

        public override Skill Clone()
        {
            return new Pachuli_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Pachuli_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "贤石",
                Description = "出牌阶段，你可以关联两种花色的组合和一个牌名（非装备）。每种花色组合和每个牌名整场游戏限选择一次。"
            };
        }
    }
    
    public class Pachuli_Skill_0_UseCondition : ConditionFilterFromSkill
    {
        public Pachuli_Skill_0_UseCondition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            Pachuli pachi = skill.Owner.Charactors.FirstOrDefault(_char => _char is Pachuli) as Pachuli;
            if (pachi == null) return false;
            List<Card> cards = pachi.GetEnabledRemainCards(ctx.World);
            if (cards.Count() == 0) return false;
            List<Enum_CardColor> colors = pachi.GetEnabledCardColorPatternFirst();
            if (colors.Count() == 0) return false;
            return true;
        }
    }

    public class Pachuli_Skill_0_TargetFilter : PlayerFilterFromSkill
    {
        public Pachuli_Skill_0_TargetFilter(Skill _skill) : base(_skill)
        {

        }

        public override Enum_PlayerFilterFlag GetFlag(Context ctx)
        {
            return Enum_PlayerFilterFlag.ForceAll;
        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            return want == skill.Owner;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() >= 1;
        }

    }

    public class Pachuli_Skill_0_CostFilter : CardFilterFromSkill
    {
        public Pachuli_Skill_0_CostFilter(Skill _skill) : base(_skill)
        {

        }

        public override Enum_CardFilterFlag GetFlag(Context ctx)
        {
            return Enum_CardFilterFlag.ForceAll;
        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            return false;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return true;
        }
    }

    public class Pachuli_Skill_1 : Skill, ISkillCardMultiConverter, ISkillCardMultiConverter2
    {
        public Pachuli_Skill_1()
        {
            this.condition = new Pachuli_Skill_1_Condition(this);
            this.cardfilter = new Pachuli_Skill_1_CardFilter(this);
            this.cardconverter = new Pachuli_Skill_1_CardConverter(this);
            
        }

        private Pachuli_Skill_1_Condition condition;
        public ConditionFilter UseCondition { get { return this.condition; } }

        private Pachuli_Skill_1_CardFilter cardfilter;
        public CardFilter CardFilter { get { return this.cardfilter; } }

        private Pachuli_Skill_1_CardConverter cardconverter;
        public CardCalculator CardConverter { get { return this.cardconverter; } }
      
        
        public IEnumerable<string> GetCardTypes(Context ctx)
        {
            if (enabledcardtypes != null) return enabledcardtypes;
            Pachuli pachi = Owner.Charactors.FirstOrDefault(_char => _char is Pachuli) as Pachuli;
            if (pachi == null) return new List<string>();
            HashSet<string> cardnames = new HashSet<string>();
            foreach (string cardname in pachi.Relations.Values)
            {
                if (cardnames.Contains(cardname)) continue;
                cardnames.Add(cardname);
            }
            return cardnames;
        }

        public void SetSelectedCardType(Context ctx, string cardtype)
        {
            Enum_CardColor firstcolor = Enum_CardColor.None;
            Enum_CardColor secondcolor = Enum_CardColor.None;
            Card cardinst = ctx.World.GetCardInstance(cardtype);
            Pachuli pachi = Owner.Charactors.FirstOrDefault(_char => _char is Pachuli) as Pachuli;
            if (pachi == null) return;
            foreach (KeyValuePair<KeyValuePair<Enum_CardColor, Enum_CardColor>, string> kvp in pachi.Relations)
            {
                if (kvp.Value.Equals(cardtype) != true) continue;
                firstcolor = kvp.Key.Key;
                secondcolor = kvp.Key.Value;
            }
            cardfilter.FirstColor = firstcolor;
            cardfilter.SecondColor = secondcolor;
            cardconverter.SeemAs = cardinst;
        }

        private string selectedcardtype;
        public string SelectedCardType
        {
            get { return this.selectedcardtype; }
        }

        public void SetEnabledCardTypes(Context ctx, IEnumerable<string> cardtypes)
        {
            Pachuli pachi = Owner.Charactors.FirstOrDefault(_char => _char is Pachuli) as Pachuli;
            if (pachi == null) return;
            HashSet<string> cardnames = new HashSet<string>();
            foreach (string cardname in pachi.Relations.Values)
            {
                if (cardnames.Contains(cardname)) continue;
                cardnames.Add(cardname);
            }
            this.enabledcardtypes = new List<string>();
            foreach (string cardname in cardtypes)
            {
                if (!cardnames.Contains(cardname)) continue;
                enabledcardtypes.Add(cardname);
            }
        }

        public void CancelEnabledCardTypes(Context ctx)
        {
            this.enabledcardtypes = null;
        }

        private List<string> enabledcardtypes;
        public IEnumerable<string> EnabledCardTypes
        {
            get { return enabledcardtypes; }
        }

        public override Skill Clone()
        {
            return new Pachuli_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Pachuli_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "魔法",
                Description = "你可以将两张牌视做一种牌来打出，这两张牌的花色组合和打出的牌的排名必须通过【贤石】关联过。"
            };
        }
    }
   
    public class Pachuli_Skill_1_Condition : ConditionFilterFromSkill
    {
        public Pachuli_Skill_1_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            if (!(Skill is ISkillCardMultiConverter)) return false;
            ISkillCardMultiConverter multiconv = (ISkillCardMultiConverter)Skill;
            if (multiconv.GetCardTypes(ctx).Count() == 0) return false;
            Zone handzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return false;
            if (handzone.Cards.Count() == 0) return false;
            return true;
        }
    }
    
    public class Pachuli_Skill_1_CardFilter : CardFilterFromSkill
    {
        public Pachuli_Skill_1_CardFilter(Skill _skill) : base(_skill)
        {

        }

        private Enum_CardColor firstcolor;
        public Enum_CardColor FirstColor
        {
            get { return this.firstcolor; }
            set { this.firstcolor = value; }
        }

        private Enum_CardColor secondcolor;
        public Enum_CardColor SecondColor
        {
            get { return this.secondcolor; }
            set { this.secondcolor = value; }
        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (want.Zone?.KeyName?.Equals(Zone.Hand) != true) return false; 
            if (selecteds.Count() == 0)
            {
                if (want.CardColor.E == firstcolor) return true;
                if (want.CardColor.E == secondcolor) return true;
                return false;
            }
            if (selecteds.Count() == 1)
            {
                Card firstcard = selecteds.FirstOrDefault();
                if (firstcard.CardColor.E == firstcolor) return want.CardColor.E == secondcolor;
                if (firstcard.CardColor.E == secondcolor) return want.CardColor.E == firstcolor;
                return false;
            }
            return false;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() >= 2;
        }

    }

    public class Pachuli_Skill_1_CardConverter : CardCalculatorFromSkill
    {
        public Pachuli_Skill_1_CardConverter(Skill _skill) : base(_skill)
        {
            
        }

        private Card seemas;
        public Card SeemAs
        {
            get { return this.seemas; }
            set { this.seemas = value; }
        }

        public override Card GetCombine(Context ctx, IEnumerable<Card> oldvalue)
        {
            if (seemas == null) return oldvalue.FirstOrDefault();
            Card card0 = oldvalue.FirstOrDefault();
            Card card1 = oldvalue.LastOrDefault();
            if (card0.CardColor?.E == card1.CardColor?.E)
            {
                seemas = seemas.Clone(card0);
                seemas.CardPoint = -1;
                seemas.SetValue(Pachuli.UsedByMagic, 1);
                return seemas;
            }
            if (card0.CardColor?.SeemAs(Enum_CardColor.Red) == true
             && card1.CardColor?.SeemAs(Enum_CardColor.Red) == true)
            {
                seemas = seemas.Clone(card0);
                seemas.CardColor = new CardColor(Enum_CardColor.Red);
                seemas.CardPoint = -1;
                seemas.SetValue(Pachuli.UsedByMagic, 1);
                return seemas;
            }
            if (card0.CardColor?.SeemAs(Enum_CardColor.Black) == true
             && card1.CardColor?.SeemAs(Enum_CardColor.Black) == true)
            {
                seemas = seemas.Clone(card0);
                seemas.CardColor = new CardColor(Enum_CardColor.Black);
                seemas.CardPoint = -1;
                seemas.SetValue(Pachuli.UsedByMagic, 1);
                return seemas;
            }
            seemas = seemas.Clone(card0);
            seemas.CardColor = new CardColor(Enum_CardColor.None);
            seemas.CardPoint = -1;
            seemas.SetValue(Pachuli.UsedByMagic, 1);
            return seemas;
        }
    }
    
    public class Pachuli_Skill_2 : Skill
    {
        public Pachuli_Skill_2()
        {
            Triggers.Add(new Pachuli_Skill_2_Trigger_0(this));
            Triggers.Add(new Pachuli_Skill_2_Trigger_1(this));
        }

        public override Skill Clone()
        {
            return new Pachuli_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new Pachuli_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "哮喘",
                Description = "锁定技，当你在出牌阶段以【魔法】连续打出两张牌时，你对自己造成一点伤害。当你受到一点伤害时，你摸两张牌。"
            };
        }
    }

    public class Pachuli_Skill_2_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public Pachuli_Skill_2_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Pachuli_Skill_2_Trigger_0_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => CardEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(ctx.Ev);
            int number = skill.Owner.GetValue(Pachuli.MagicSequentlyNumber);
            if (ev.Card.GetValue(Pachuli.UsedByMagic) == 1)
            {
                if (++number >= 2)
                {
                    SkillEvent ev_skill = new SkillEvent();
                    ev_skill.Reason = ev;
                    ev_skill.Skill = skill;
                    ev_skill.Source = skill.Owner;
                    ev_skill.Targets.Clear();
                    ev_skill.Targets.Add(skill.Owner);
                    ctx.World.InvokeEvent(ev);
                    if (!ev.Cancel)
                        ctx.World.Damage(skill.Owner, skill.Owner, 1, DamageEvent.Normal, ev);
                }
                skill.Owner.SetValue(Pachuli.MagicSequentlyNumber, number);
            }
            else
            {
                skill.Owner.SetValue(Pachuli.MagicSequentlyNumber, 0);
            }
        }
    }
    
    public class Pachuli_Skill_2_Trigger_0_Condition : ConditionFilterFromSkill
    { 
        public Pachuli_Skill_2_Trigger_0_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(ctx.Ev);
            if (ev.Source != skill.Owner) return false;
            return true;
        }
    }

    public class Pachuli_Skill_2_Trigger_1 : SkillTrigger, ITriggerInState
    {
        public Pachuli_Skill_2_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Pachuli_Skill_2_Trigger_1_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Damaged; }
       
        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (!(state.Ev is DamageEvent)) return;
            DamageEvent ev = (DamageEvent)(state.Ev);
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            for (int i = 0; i < ev.DamageValue; i++)
                ctx.World.DrawCard(skill.Owner, 2, ev);
        }
    }
    
    public class Pachuli_Skill_2_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Pachuli_Skill_2_Trigger_1_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            if (!(state.Ev is DamageEvent)) return false;
            DamageEvent ev = (DamageEvent)(state.Ev);
            if (ev.DamageValue <= 0) return false;
            if (ev.DamageType?.Equals(DamageEvent.Lost) == true) return false;
            return true;
        }
    }
}
