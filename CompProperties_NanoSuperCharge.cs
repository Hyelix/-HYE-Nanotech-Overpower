using Verse;

namespace Nanotech
{
    public class CompProperties_NanoSuperCharge : CompProperties
    {
        public float energyPerSecond = 300f;
        public bool onlyWhenChargingJob = false;
        public float scanRadius = 2.6f;

        public CompProperties_NanoSuperCharge()
        {
            compClass = typeof(CompNanoSuperCharge);
        }
    }
}
