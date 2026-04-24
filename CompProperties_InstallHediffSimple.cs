using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Nanotech
{
    public class CompProperties_InstallHediffSimple : CompProperties_UseEffect
    {
        public HediffDef hediff;
        public BodyPartDef targetPart;
        public List<BodyPartDef> targetParts;
        public bool humanlikesOnly = true;
        public bool failIfAlreadyInstalled = true;
        public bool allowInstallOnMissing = true;

        public CompProperties_InstallHediffSimple()
        {
            compClass = typeof(CompUseEffect_InstallHediffSimple);
        }
    }
}
