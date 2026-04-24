using RimWorld;
using Verse;
using Verse.Sound;

namespace Nanotech
{
    public class CompProperties_AlliedMechBooster : CompProperties
    {
        public float range = 12f;
        public HediffDef hediff;
        public SoundDef activeSound;
        public int scanIntervalTicks = 10;
        public bool alsoAffectNonHostileAllies = false;
        public int lingerTicks = 0;

        public CompProperties_AlliedMechBooster()
        {
            compClass = typeof(CompAlliedMechBooster);
        }
    }
}
