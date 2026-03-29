using Verse;

namespace Nanotech
{
    public class Projectile_ApplyAnesthetic : Bullet
    {
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing, blockedByShield);
            AnestheticUtil.TryApply(hitThing as Pawn, "zzz");
        }
    }
}
