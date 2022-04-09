using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core
{
    public class Calculator : ShaPriorityObject
    {
        public override string ToString()
        {
            return String.Format("Calculator:{0}", KeyName);
        }

        public virtual int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            return oldvalue;
        }
    }

    public class CardCalculator : ShaPriorityObject
    {
        public override string ToString()
        {
            return String.Format("CardCalculator:{0}", KeyName);
        }
        public virtual Card GetValue(Context ctx, Card oldvalue)
        {
            return oldvalue;
        }

        public virtual Card GetCombine(Context ctx, IEnumerable<Card> oldvalue)
        {
            return null;
        }
    }

    public class CalculatorFromSkill : Calculator, ICalculatorFromSkill
    {
        public CalculatorFromSkill(Skill _skill)
        {
            this.skill = _skill;
        }

        protected Skill skill;
        public Skill Skill { get { return this.skill; } }
    }

    public class CalculatorFromCard : Calculator, ICalculatorFromCard
    {
        public CalculatorFromCard(Card _card)
        {
            this.card = _card;
        }

        protected Card card;
        public Card Card { get { return this.card; } }
    }

    public class CardCalculatorFromSkill : CardCalculator, ICalculatorFromSkill
    {
        public CardCalculatorFromSkill(Skill _skill)
        {
            this.skill = _skill;
        }

        protected Skill skill;
        public Skill Skill { get { return this.skill; } }
    }

    public class CardCalculatorFromCard : CardCalculator, ICalculatorFromCard
    {
        public CardCalculatorFromCard(Card _card)
        {
            this.card = _card;
        }

        protected Card card;
        public Card Card { get { return this.card; } }
    }

    public class OverrideCalculator : Calculator
    {
        private Calculator oldcalculator;
        public Calculator OldCalculator
        {
            get { return this.oldcalculator; }
            set { this.oldcalculator = value; }
        }
    }

    public class OverrideCardCalculator : CardCalculator
    {
        private CardCalculator oldcalculator;
        public CardCalculator OldCalculator
        {
            get { return this.oldcalculator; }
            set { this.oldcalculator = value; }
        }
    }
    
    public class CalculatorCollection
    {
        private Dictionary<string, SortedDictionary<int, List<Calculator>>> calcofprops
            = new Dictionary<string, SortedDictionary<int, List<Calculator>>>();
        private SortedDictionary<int, List<Calculator>> othercalcs
            = new SortedDictionary<int, List<Calculator>>();

        public void Clear()
        {
            calcofprops.Clear();
            othercalcs.Clear();
        }
      
        public void Add(Calculator calc)
        {
            if (calc is ICalculatorProperty)
            {
                ICalculatorProperty calcprop = (ICalculatorProperty)calc;
                string propname = calcprop.PropertyName;
                if (String.IsNullOrEmpty(propname)) propname = String.Empty;
                SortedDictionary<int, List<Calculator>> sort = null;
                List<Calculator> list = null;
                if (!calcofprops.TryGetValue(propname, out sort))
                {
                    sort = new SortedDictionary<int, List<Calculator>>();
                    calcofprops.Add(propname, sort);
                }
                if (!sort.TryGetValue(calc.Priority, out list))
                {
                    list = new List<Calculator>();
                    sort.Add(calc.Priority, list);
                }
                list.Add(calc);
            }
            else
            {
                List<Calculator> list = null;
                if (!othercalcs.TryGetValue(calc.Priority, out list))
                {
                    list = new List<Calculator>();
                    othercalcs.Add(calc.Priority, list);
                }
                list.Add(calc);
            }
        }

        public bool Remove(Calculator calc)
        {
            if (calc is ICalculatorProperty)
            {
                ICalculatorProperty calcprop = (ICalculatorProperty)calc;
                string propname = calcprop.PropertyName;
                if (String.IsNullOrEmpty(propname)) propname = String.Empty;
                SortedDictionary<int, List<Calculator>> sort = null;
                List<Calculator> list = null;
                if (calcofprops.TryGetValue(propname, out sort)
                 && sort.TryGetValue(calc.Priority, out list))
                    return list.Remove(calc);
            }
            else
            {
                List<Calculator> list = null;
                if (othercalcs.TryGetValue(calc.Priority, out list))
                    return list.Remove(calc);
            }
            return false;
        }

        public int GetValueFollowPriority(Context ctx, ShaObject obj, string propertyname, int oldvalue,
            SortedDictionary<int, List<Calculator>> sort0)
        {
            foreach (List<Calculator> list in sort0.Values.Reverse())
                foreach (Calculator calc in list)
                {
                    Calculator newcalc = ctx.World.TryReplaceNewCalculator(calc, ctx.Ev);
                    if (newcalc == null) continue;
                    oldvalue = newcalc.GetValue(ctx, obj, propertyname, oldvalue);
                }
            return oldvalue;
        }

        protected int GetValueFollowPriority(Context ctx, ShaObject obj, string propertyname, int oldvalue,
            SortedDictionary<int, List<Calculator>> sort0,
            SortedDictionary<int, List<Calculator>> sort1)
        {
            List<Calculator> list = new List<Calculator>();
            SortedDictionary<int, List<Calculator>>.Enumerator enum0 = sort0.GetEnumerator();
            SortedDictionary<int, List<Calculator>>.Enumerator enum1 = sort1.GetEnumerator();
            while (enum0.Current.Value != null && enum1.Current.Value != null)
            {
                if (enum1.Current.Value == null || enum1.Current.Key > enum0.Current.Key)
                    list.AddRange(enum0.Current.Value);
                else
                    list.AddRange(enum1.Current.Value);
            }
            foreach (Calculator calc in list.Reverse<Calculator>())
                oldvalue = calc.GetValue(ctx, obj, propertyname, oldvalue);
            return oldvalue;
        }

        public int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            SortedDictionary<int, List<Calculator>> sort0 = null;
            SortedDictionary<int, List<Calculator>> sort1 = othercalcs;
            if (!calcofprops.TryGetValue(propertyname, out sort0))
                return GetValueFollowPriority(ctx, obj, propertyname, oldvalue, sort1);
            return GetValueFollowPriority(ctx, obj, propertyname, oldvalue, sort0, sort1);
        }
    }

    public interface ICalculatorFromSkill
    {
        Skill Skill { get; }
    }

    public interface ICalculatorFromCard
    {
        Card Card { get; }
    }

    public interface ICalculatorProperty
    {
        string PropertyName { get; }
    }
}
