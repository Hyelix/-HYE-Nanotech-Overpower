using RimWorld;
using Verse;

namespace Nanotech
{
    public class Verb_MeleeApplyAnesthetic : Verb_MeleeAttack
    {
        protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
        {
            AnestheticUtil.TryApply(target.Thing as Pawn, "rekt");
            return new DamageWorker.DamageResult();
        }
    }
}
