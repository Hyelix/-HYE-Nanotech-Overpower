using RimWorld;
using UnityEngine;
using Verse;

namespace Nanotech
{
    public class Projectile_ApplyAnesthetic : Bullet
    {
        private static HediffDef anestheticDef;

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing, blockedByShield);

            Pawn pawn = hitThing as Pawn;
            if (pawn == null || pawn.Dead || (pawn.RaceProps != null && pawn.RaceProps.IsMechanoid))
                return;

            if (anestheticDef == null)
            {
                anestheticDef = HediffDef.Named("Anesthetic");
                if (anestheticDef == null)
                {
                    Log.ErrorOnce("[Nanotech] HediffDef 'Anesthetic' not found!", 123989);
                    return;
                }
            }

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
        }
    }
}
