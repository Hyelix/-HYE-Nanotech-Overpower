using System.Linq;
using UnityEngine;
using Verse;

namespace Nanotech
{
    public class PlaceWorker_NanoRegenerator : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            var map = Find.CurrentMap;
            if (map == null) return;

            float radius = 10f;
            var props = def.comps?.FirstOrDefault(c => c is CompProperties_NanoRegenerator) as CompProperties_NanoRegenerator;
            if (props != null) radius = props.radius;

            var cells = GenRadial.RadialCellsAround(center, radius, true)
                .Where(c => c.InBounds(map)).ToList();
            GenDraw.DrawFieldEdges(cells);
        }
    }
}
