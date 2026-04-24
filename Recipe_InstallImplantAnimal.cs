using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Nanotech
{
    public class Recipe_InstallImplantAnimal : Recipe_InstallImplant
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            if (recipe.addsHediff != null && pawn.health.hediffSet.HasHediff(recipe.addsHediff))
                return Enumerable.Empty<BodyPartRecord>();

            return pawn.health.hediffSet.GetNotMissingParts()
                .Where(p => recipe.appliedOnFixedBodyParts.Contains(p.def));
        }

        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            return thing is Pawn;
        }
    }
}