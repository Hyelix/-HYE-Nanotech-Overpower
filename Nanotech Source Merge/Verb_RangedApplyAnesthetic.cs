using RimWorld;
using UnityEngine;
using Verse;

namespace Nanotech
{
    public class Verb_RangedApplyAnesthetic : Verb_Shoot
    {
        private static HediffDef anestheticDef;

        protected override bool TryCastShot()
        {
            if (!base.TryCastShot()) return false;

            if (anestheticDef == null)
            {
                anestheticDef = HediffDef.Named("Anesthetic");
                if (anestheticDef == null)
                {
                    Log.ErrorOnce("[Nanotech] HediffDef 'Anesthetic' not found!", 123988);
                    return true;
                }
            }

            Thing hitThing = currentTarget.Thing;
            Pawn pawn = hitThing as Pawn;

            if (pawn == null || pawn.Dead || (pawn.RaceProps != null && pawn.RaceProps.IsMechanoid))
                return true;

            Hediff existing = pawn.health.hediffSet.GetFirstHediffOfDef(anestheticDef);
            float initSeverity = anestheticDef.initialSeverity > 0f ? anestheticDef.initialSeverity : 1f;

            if (existing != null)
            {
                if (existing.Severity < initSeverity) existing.Severity = initSeverity;
            }
            else
            {
                Hediff h = HediffMaker.MakeHediff(anestheticDef, pawn);
                h.Severity = initSeverity;
                pawn.health.AddHediff(h);
            }

            if (pawn.Map != null)
                MoteMaker.ThrowText(pawn.Position.ToVector3Shifted(), pawn.Map, "zzz", -1f);

            return true;
        }
    }
}
