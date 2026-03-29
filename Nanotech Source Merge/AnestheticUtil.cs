using RimWorld;
using Verse;

namespace Nanotech
{
    internal static class AnestheticUtil
    {
        private static HediffDef _def;

        private static HediffDef Def
        {
            get
            {
                if (_def == null)
                {
                    _def = HediffDef.Named("Anesthetic");
                    if (_def == null)
                        Log.ErrorOnce("[Nanotech] HediffDef 'Anesthetic' not found!", 123987);
                }
                return _def;
            }
        }

        internal static void TryApply(Pawn pawn, string moteText)
        {
            if (pawn == null || pawn.Dead) return;
            if (pawn.RaceProps != null && pawn.RaceProps.IsMechanoid) return;

            var def = Def;
            if (def == null) return;

            Hediff existing = pawn.health.hediffSet.GetFirstHediffOfDef(def);
            float initSeverity = def.initialSeverity > 0f ? def.initialSeverity : 1f;

            if (existing != null)
            {
                if (existing.Severity < initSeverity) existing.Severity = initSeverity;
            }
            else
            {
                Hediff h = HediffMaker.MakeHediff(def, pawn);
                h.Severity = initSeverity;
                pawn.health.AddHediff(h);
            }

            if (pawn.Map != null)
                MoteMaker.ThrowText(pawn.Position.ToVector3Shifted(), pawn.Map, moteText, -1f);
        }
    }
}
