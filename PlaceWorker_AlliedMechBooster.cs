using UnityEngine;
using Verse;

namespace Nanotech
{
    public class PlaceWorker_AlliedMechBooster : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            var props = def.GetCompProperties<CompProperties_AlliedMechBooster>();
            if (props != null)
                GenDraw.DrawRadiusRing(center, props.range);
        }
    }
}
