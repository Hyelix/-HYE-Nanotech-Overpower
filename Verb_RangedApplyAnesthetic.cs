using RimWorld;
using Verse;

namespace Nanotech
{
    public class Verb_RangedApplyAnesthetic : Verb_Shoot
    {
        protected override bool TryCastShot()
        {
            if (!base.TryCastShot()) return false;
            AnestheticUtil.TryApply(currentTarget.Thing as Pawn, "zzz");
            return true;
        }
    }
}
