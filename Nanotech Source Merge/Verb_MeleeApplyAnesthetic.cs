using RimWorld;
using UnityEngine;
using Verse;

namespace Nanotech
{
    public class Verb_MeleeApplyAnesthetic : Verb_MeleeAttack
    {
        private static HediffDef anestheticDef;

        protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
        {
            if (anestheticDef == null)
            {
                anestheticDef = HediffDef.Named("Anesthetic");
                if (anestheticDef == null)
                    Log.ErrorOnce("[Nanotech] HediffDef 'Anesthetic' not found!", 123987);
            }

            Thing hitThing = target.Thing;
            Pawn pawn = hitThing as Pawn;

            if (anestheticDef != null && pawn != null && !pawn.Dead &&
                (pawn.RaceProps == null || !pawn.RaceProps.IsMechanoid))
            {
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
                    MoteMaker.ThrowText(pawn.Position.ToVector3Shifted(), pawn.Map, "rekt", -1f);
            }

            return new DamageWorker.DamageResult();
        }
    }
}
