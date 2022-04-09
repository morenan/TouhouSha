using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Filters
{
    public class UseCardStateCardFilter : CardFilter, ICardFilterRequiredCardTypes
    {
        public const string DefaultKeyName = "出牌阶段选择使用的牌";

        public UseCardStateCardFilter(Context _ctx)
        {
            this.ctx = _ctx;
            State state = ctx.World.GetPlayerState();
            this.controller = state?.Owner;
        }

        private Context ctx;
        public Context Context { get { return this.ctx; } }

        private Player controller;
        public Player Controller { get { return this.controller; } }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() >= 1) return false;
            if (want == null) return false;
            if (want.Zone?.KeyName?.Equals(Zone.Hand) != true) return false;
            if (want.UseCondition?.Accept(ctx) != true) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() >= 1;
        }

        IEnumerable<string> ICardFilterRequiredCardTypes.RequiredCardTypes
        {
            get
            {
                Zone handzone = controller.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                if (handzone != null)
                {
                    foreach (string cardname in ctx.World.GetBaseCardKeyNames()
                        .Concat(ctx.World.GetEquipCardKeyNames())
                        .Concat(ctx.World.GetSpellCardKeyNames()))
                    {
                        Card card = ctx.World.GetCardInstance(cardname);
                        card.Zone = handzone;
                        if (card.UseCondition?.Accept(ctx) == true)
                            yield return cardname;
                        card.Zone = null;
                    }
                }
            }
        }
    }
}
